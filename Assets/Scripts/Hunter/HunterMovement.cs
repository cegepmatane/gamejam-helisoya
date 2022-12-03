using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Discovery;
using Unity.VisualScripting;

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

    public static HunterMovement localPlayer;

    public override void OnStartClient()
    {
        if (isLocalPlayer)
        {
            localPlayer = this;
        }
        currentAmmo = maxAmmo;
        totalAmmo = 18;
    }


    void Update()
    {
        if (GameGUI.instance.paused) return;

        move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        transform.position += move * speed * Time.deltaTime;



        if (move != Vector3.zero)
        {
            Quaternion toRotate = Quaternion.LookRotation(Vector3.forward, move);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotate, 720 * Time.deltaTime);
        }

        feetAnimator.animator.SetBool("moving", move != Vector3.zero);
        bodyAnimator.animator.SetBool("moving", move != Vector3.zero);


        if (Time.time - lastFire >= currentTimeToWait)
        {
            if (Input.GetKeyDown(KeyCode.R) && totalAmmo > 0 && maxAmmo != currentAmmo)
            {
                lastFire = Time.time;
                currentTimeToWait = reloadTime;
                bodyAnimator.SetTrigger("reload");
                ChangeMagazine();
                GameGUI.instance.UpdateAmmoText(currentAmmo, totalAmmo);
            }
            else if (Input.GetMouseButtonDown(0) && currentAmmo > 0)
            {
                lastFire = Time.time;
                currentTimeToWait = fireCooldownTime;
                bodyAnimator.SetTrigger("shoot");

                currentAmmo--;
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pos.z = 0;
                SpawnBullet(pos);
                GameGUI.instance.UpdateAmmoText(currentAmmo, totalAmmo);
            }
        }

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

        bullet.GetComponent<Bullet>().Init(vec);
    }

    

    private void OnTriggerEnter2D(Collider2D collision)
    {

    }


    public void AddAmmoFromBox(AmmoBox box)
    {
        totalAmmo += box.GetAmmo();
        GameGUI.instance.UpdateAmmoText(currentAmmo, totalAmmo);
        NetworkServer.Destroy(box.gameObject);
    }
}
