using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BigfootController : NetworkBehaviour
{

    // Mouvment Var
    private PathFinder pathfinder;
    private DissolveEffect dissolve;
    public PerlinNoiseMap map;
    private Path m_Path;
    [SyncVar] public float speed = 5f;
    private Vector3 MouvmentVector;

    public bool isDead = false;

    // Health var
    public int maxHealth = 50;
    [SyncVar] public int currentHealth;

    // Other var
    public NetworkAnimator Animator;

    public AudioSource generalAudio;

    [SerializeField] private Renderer[] renderers;

    public Transform feets;
    public GameObject kart;

    public void Init()
    {
        currentHealth = maxHealth;
        pathfinder = GetComponentInChildren<PathFinder>();
        pathfinder.setMap(map);
        pathfinder.setSpeed(speed);
        transform.position = map.getBigFootSpawn();
        GameGUI.instance.bfHealth.SetHealth(currentHealth, maxHealth);
    }


    // Update is called once per frame
    void Update()
    {

        if (!isServer) return;

        MouvmentVector = pathfinder.getMouvmentVector(transform.position);

        // Todo use force and debug colision  And a Gizmo editor is available

        Quaternion toRotate = Quaternion.LookRotation(Vector3.forward, MouvmentVector);
        feets.rotation = Quaternion.RotateTowards(feets.rotation, toRotate, 720 * Time.deltaTime);
        transform.position = MouvmentVector + transform.position;

        // Todo Rotation

    }

    void Start()
    {
        if (Random.Range(0, 100) <= 15)
        {
            kart.SetActive(true);
        }
        GameGUI.instance.bfHealth.SetHealth(currentHealth, maxHealth);
        Animator.animator.SetBool("moving", true);
    }


    [Command(requiresAuthority = false)]
    public void TakeDamage(int dammage)
    {
        RpcAddSound("BigfootHurt");
        currentHealth -= dammage;

        if (speed < 10)
        {
            speed += 0.5f;
        }

        pathfinder.setSpeed(speed);

        RCPUpdateHealthBar();
        if (currentHealth <= 0)
        {
            RpcTriggerEnd();
        }
    }

    [ClientRpc]
    public void RpcTriggerEnd()
    {
        StartCoroutine(WaitForEnd());

    }

    IEnumerator WaitForEnd()
    {
        float dissolveAmount = 0;
        while (dissolveAmount < 1)
        {
            dissolveAmount = Mathf.Clamp01(dissolveAmount + 0.3f * Time.deltaTime);
            print("Dissolve " + dissolveAmount);
            foreach (Renderer rd in renderers)
                rd.material.SetFloat("_DissolveAmmount", dissolveAmount);
            yield return new WaitForEndOfFrame();
        }

        GameGUI.instance.ShowEndScreen();
    }

    public void HesDead()
    {

        if (currentHealth <= 0)
        {
            isDead = true;
            print("le bigfoot est mort");
            NetworkServer.Destroy(gameObject);
        }
    }

    [ClientRpc]
    public void RCPUpdateHealthBar()
    {
        GameGUI.instance.bfHealth.SetHealth(currentHealth, maxHealth);
    }




    [Command]
    public void CmdAddSound(string filename)
    {
        RpcAddSound(filename);
    }

    [ClientRpc]
    public void RpcAddSound(string filename)
    {
        AudioClip clip = Resources.Load<AudioClip>("Audio/SFX/" + filename);
        if (clip != null)
            generalAudio.PlayOneShot(clip);
    }

}