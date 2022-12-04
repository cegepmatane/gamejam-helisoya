using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BigfootController : NetworkBehaviour
{


   // public Grid Grid;
    public OldPathFinder pathfinder;
    //public Transform[] Objectives;

    public Transform Spawn;
    public Transform Objective;

    public PerlinNoiseMap map;
    

    private Path m_Path;

    [SerializeField] Bullet bullet;

    [SerializeField] private float speed;

    [SerializeField] private float rotationSpeed;

    public NetworkAnimator Animator;

    public int maxHealth = 50;
    [SyncVar] public int currentHealth;

    public float Speed = 10f;

    private Vector3 chqngem;



    public override void OnStartServer()
    {
        currentHealth = maxHealth;

        transform.position = Spawn.transform.position;
    }

    public float amplitude = 10f;          //Set in Inspector 
    private Vector3 tempPos;

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.P))
        {
            TakeDamage(10);
            print(currentHealth);
        }


        tempPos.x = 7;
        tempPos.y = amplitude * Mathf.Sin(speed * Time.time);
        transform.localPosition = tempPos;


    }

    void Start()
    {
        GameGUI.instance.bfHealth.SetHealth(currentHealth, maxHealth);
        Animator.animator.SetBool("moving", true);
    }


    [Command(requiresAuthority = false)]
    public void TakeDamage(int dammage)
    {
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

}
