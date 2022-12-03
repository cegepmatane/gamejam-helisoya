using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BigfootController : NetworkBehaviour
{

    [SerializeField] Bullet bullet;

    [SerializeField] private float speed;

    [SerializeField] private float rotationSpeed;

    public NetworkAnimator Animator; 

    public int maxHealth = 50;
    [SyncVar] public int currentHealth;

    public HealthBarBF healthBar;

    public override void OnStartServer()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

    }


    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector2 movementDirection = new Vector2(horizontalInput, verticalInput);
        float inputMagnitude = Mathf.Clamp01(movementDirection.magnitude);
        movementDirection.Normalize();

        transform.Translate(movementDirection * speed * inputMagnitude * Time.deltaTime, Space.World);

        if (movementDirection != Vector2.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, movementDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            TakeDamage(10);
            print(currentHealth);
        }

        
    }

    void Start()
    {
        Animator.animator.SetBool("moving", true); 
    }


    [Command(requiresAuthority = false)]
    public void TakeDamage(int dammage)
    {
        currentHealth -= dammage;


        RCPUpdateHealthBar();
        //HesDead();
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
        healthBar.SetHealth(currentHealth);
    }

}
