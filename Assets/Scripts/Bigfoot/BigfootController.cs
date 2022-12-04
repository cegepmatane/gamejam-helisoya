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