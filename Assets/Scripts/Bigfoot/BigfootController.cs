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
    public PerlinNoiseMap map;
    private Path m_Path;
    [SerializeField] private float speed = 5f;
    private Vector3 MouvmentVector;

    // Health var
    public int maxHealth = 50;
    [SyncVar] public int currentHealth;

    // Other var
    public NetworkAnimator Animator;

    public AudioSource generalAudio;

    public override void OnStartServer()
    {
        currentHealth = maxHealth;
        transform.position = map.getBigFootSpawn();
        pathfinder = GetComponentInChildren<PathFinder>();
        pathfinder.setMap(map);
        pathfinder.setSpeed(speed);
    }


    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.P))
        {
            TakeDamage(10);
            print(currentHealth);
        }

        MouvmentVector = pathfinder.getMouvmentVector(transform.position);

        transform.position = MouvmentVector + transform.position;

    }

    void Start()
    {
        GameGUI.instance.bfHealth.SetHealth(currentHealth, maxHealth);
        Animator.animator.SetBool("moving", true);
    }


    [Command(requiresAuthority = false)]
    public void TakeDamage(int dammage)
    {
        RpcAddSound("BigfootHurt");
        currentHealth -= dammage;


        RCPUpdateHealthBar();
        if (currentHealth <= 0)
        {
            RpcTriggerEnd();
        }
    }

    [ClientRpc]
    public void RpcTriggerEnd()
    {
        GameGUI.instance.ShowEndScreen();
    }

    public void HesDead()
    {
        if (currentHealth <= 0)
        {
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