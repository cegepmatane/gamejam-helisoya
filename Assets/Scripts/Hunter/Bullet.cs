using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class Bullet : NetworkBehaviour
{
    [Header("General Informations")]
    [SerializeField] private float speed;
    [SerializeField] private float lifeTime;
    [SerializeField] private int damage;

    private Vector3 movementVector;

    private float timeOfCreation;


    public void Init(Vector3 vector)
    {
        timeOfCreation = Time.time;
        movementVector = vector;
        transform.right = (transform.position + vector) - transform.position;
    }



    void Update()
    {
        if (Time.time - timeOfCreation > lifeTime)
        {
            DestroySelf();
            return;
        }

        transform.position += movementVector * Time.deltaTime * speed;
    }



    public void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag.Equals("BigFoot"))
        {
            col.GetComponent<BigfootController>().TakeDamage(damage);
            DestroySelf();
        }
        else if (col.tag.Equals("Player"))
        {
            if (col.GetComponent<HunterMovement>().isLocalPlayer) return;
            // Stun Hunter (Like when hit by bigfoot)
        }
    }
}
