using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BigfootController : NetworkBehaviour {
    
    // Mouvment Var
    private PathFinder pathfinder;
    public Transform Spawn;
    public PerlinNoiseMap map;
    private Path m_Path;
    [SerializeField] private float speed;
    private Vector3 MouvmentVector;
    
    // Health var
    public int maxHealth = 50;
    [SyncVar] public int currentHealth;
    
    // Other var
    public NetworkAnimator Animator;
    
    public override void OnStartServer()
    {
        currentHealth = maxHealth;
        transform.position = Spawn.transform.position;
        pathfinder = GetComponentInChildren<PathFinder>();
        pathfinder.setMap(map);
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