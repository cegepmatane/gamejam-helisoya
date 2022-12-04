using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Discovery;
using Unity.VisualScripting;
using TMPro;

public class HunterMovement : NetworkBehaviour
{

    [Header("Shooting")]
    public int currentAmmo;
    [SerializeField] private int maxAmmo;

    public int totalAmmo;
    [SerializeField] private float fireCooldownTime;
    private float lastFire;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform barrel;
    private float currentTimeToWait;

    [Header("Reloading")]
    [SerializeField] private float reloadTime;

    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private Transform feets;
    [SerializeField] private NetworkAnimator feetAnimator;
    [SerializeField] private NetworkAnimator bodyAnimator;


    Vector3 move;

    [Header("Stun")]
    [SerializeField] private float maxStunTime;
    [HideInInspector] public bool stuned;
    [SerializeField] private SpriteRenderer bodyRenderer;

    public static HunterMovement localPlayer;

    [Header("Stats")]
    [SyncVar] public int badShot;
    [SyncVar] public int goodShot;
    [SyncVar] public int friendlyShot;

    [Header("Audio")]
    [SerializeField] private AudioSource walkSound;
    [SerializeField] private AudioSource generalSound;



    [Header("Multiplayer")]
    [SyncVar] public string playerName;
    [SerializeField] private TextMeshPro playerNameText;

    public override void OnStartClient()
    {
        if (isLocalPlayer)
        {
            localPlayer = this;
            CmdSetPlayerName(PlayerPrefs.GetString("playerName"));
        }
        else
        {
            playerNameText.text = playerName;
        }
        currentAmmo = maxAmmo;
        totalAmmo = 18;
        stuned = false;
    }


    [Command(requiresAuthority = false)]
    public void CmdSetPlayerName(string newName)
    {
        print("server : " + newName);
        playerName = newName;
        CmdRefreshNameText(newName);
    }

    [ClientRpc]
    public void CmdRefreshNameText(string value)
    {
        print("client : " + value);
        if (playerName != value) playerName = value;
        playerNameText.text = value;
    }


    void Start()
    {
        if (isServer)
        {
            transform.position = FindObjectOfType<NetworkStartPosition>().transform.position;
        }
        playerNameText.text = playerName;
    }

    void Update()
    {
        if (GameGUI.instance.paused) return;

        if (stuned)
        {
            transform.position += move * speed * Time.deltaTime;
            if (Time.time - lastFire >= currentTimeToWait)
            {
                stuned = false;
                move = Vector3.zero;
                CmdColor(Color.white);
            }
            return;
        }



        move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        transform.position += move * speed * Time.deltaTime;



        if (move != Vector3.zero)
        {
            Quaternion toRotate = Quaternion.LookRotation(Vector3.forward, move);
            feets.rotation = Quaternion.RotateTowards(feets.rotation, toRotate, 720 * Time.deltaTime);
        }

        feetAnimator.animator.SetBool("moving", move != Vector3.zero);
        bodyAnimator.animator.SetBool("moving", move != Vector3.zero);
        CmdSetWalkSound(move != Vector3.zero);


        if (Time.time - lastFire >= currentTimeToWait)
        {
            if ((Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.RightShift)) && totalAmmo > 0 && maxAmmo != currentAmmo)
            {
                lastFire = Time.time;
                currentTimeToWait = reloadTime;
                bodyAnimator.SetTrigger("reload");
                CmdAddSound("reload");
                ChangeMagazine();
                GameGUI.instance.UpdateAmmoText(currentAmmo, totalAmmo);
            }
            else if (Input.GetMouseButtonDown(0) && currentAmmo > 0)
            {
                lastFire = Time.time;
                currentTimeToWait = fireCooldownTime;
                bodyAnimator.SetTrigger("shoot");

                CmdAddSound("Gunshot");
                currentAmmo--;
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pos.z = 0;
                SpawnBullet(pos);
                GameGUI.instance.UpdateAmmoText(currentAmmo, totalAmmo);
            }
        }


        if (Input.GetKeyDown(KeyCode.M))
        {
            Stun(Vector3.right);
        }

    }




    [Command(requiresAuthority = false)]
    public void CmdColor(Color col)
    {
        RpcColor(col);
    }

    [ClientRpc]
    public void RpcColor(Color col)
    {
        bodyRenderer.color = col;
    }


    [Command(requiresAuthority = false)]
    public void Stun(Vector3 newMove)
    {
        RpcStun(newMove);
    }

    [ClientRpc]
    public void RpcStun(Vector3 newMove)
    {
        if (stuned) return;
        CmdAddSound("PlayerHurt");

        bodyRenderer.color = Color.red;
        if (!isLocalPlayer) return;

        move = newMove;
        stuned = true;
        lastFire = Time.time;
        currentTimeToWait = maxStunTime;
        bodyAnimator.animator.SetBool("moving", false);
        feetAnimator.animator.SetBool("moving", false);

    }

    public void ChangeMagazine()
    {
        while (currentAmmo < maxAmmo && totalAmmo > 0)
        {
            currentAmmo++;
            totalAmmo--;
        }
    }


    [Command]
    public void SpawnBullet(Vector3 pos)
    {
        RPCSpawnBullet(pos);
    }


    [ClientRpc]
    public void RPCSpawnBullet(Vector3 pos)
    {
        GameObject bullet = Instantiate(bulletPrefab, barrel.position, Quaternion.identity);

        Vector3 vec = pos - bullet.transform.position;
        vec.Normalize();

        bullet.GetComponent<Bullet>().Init(vec, this);
    }

    [Command(requiresAuthority = false)]
    public void CmdSetWalkSound(bool value)
    {
        RpcSetWalkSound(value);
    }

    [ClientRpc]
    public void RpcSetWalkSound(bool value)
    {
        walkSound.enabled = value;
    }


    [Command(requiresAuthority = false)]
    public void CmdAddSound(string filename)
    {
        RpcAddSound(filename);
    }

    [ClientRpc]
    public void RpcAddSound(string filename)
    {
        AudioClip clip = Resources.Load<AudioClip>("Audio/SFX/" + filename);
        if (clip != null)
            generalSound.PlayOneShot(clip);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("BigFoot"))
        {
            Vector3 vec = transform.position - collision.transform.position;
            vec.Normalize();
            Stun(vec);
        }
    }


    public void AddAmmoFromBox(AmmoBox box)
    {
        totalAmmo += box.GetAmmo();
        GameGUI.instance.UpdateAmmoText(currentAmmo, totalAmmo);
        CmdAddSound("ammoPickup");
        NetworkServer.Destroy(box.gameObject);
    }



    [Command(requiresAuthority = false)]
    public void AddFriendlyShoot()
    {
        friendlyShot++;
        RefreshValues(goodShot, badShot, friendlyShot);
    }

    [Command(requiresAuthority = false)]
    public void AddMissedShot()
    {
        badShot++;
        RefreshValues(goodShot, badShot, friendlyShot);
    }

    [Command(requiresAuthority = false)]
    public void AddGoodShot()
    {
        goodShot++;
        RefreshValues(goodShot, badShot, friendlyShot);
    }


    [ClientRpc]
    public void RefreshValues(int good, int bad, int friendly)
    {
        friendlyShot = friendly;
        goodShot = good;
        badShot = bad;
    }
}
