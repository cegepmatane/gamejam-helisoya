using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BigfootController : NetworkBehaviour
{

    //[SerializeField] Bullet bullet;

    [SerializeField] private float speed;

    [SerializeField] private float rotationSpeed;

    public int maxHealth = 50;
    [SyncVar] public int currentHealth;

    public override void OnStartServer()
    {
        currentHealth = maxHealth;

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
            TakeDamage(20);
            print(currentHealth);
        }
    }


    [Command(requiresAuthority = false)]
    public void TakeDamage(int dammage)
    {
        currentHealth -= dammage;

        HesDead();
    }

    public void HesDead()
    {
        if (currentHealth <= 0)
        {
            print("le bigfoot est mort");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {


        // if (Input.GetKeyDown(KeyCode.P))
        //{
        Debug.LogWarning("collision ennemi");

        TakeDamage(20);
        //}
    }


}
