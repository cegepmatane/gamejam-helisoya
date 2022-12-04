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

    private HunterMovement parent;

    [SyncVar] private bool dead;

    public override void OnStartServer()
    {
        dead = false;
    }

    public void Init(Vector3 vector, HunterMovement hunter)
    {
        parent = hunter;
        timeOfCreation = Time.time;
        movementVector = vector;
        transform.right = (transform.position + vector) - transform.position;
    }



    void Update()
    {
        if (Time.time - timeOfCreation > lifeTime)
        {
            if (parent == HunterMovement.localPlayer)
            {
                parent.AddMissedShot();
            }
            DestroySelf();
            return;
        }

        transform.position += movementVector * Time.deltaTime * speed;
    }

    [Command(requiresAuthority = false)]
    public void CmdSetDead()
    {
        dead = true;
    }


    public void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        if (dead) return;
        if (col.tag.Equals("BigFoot"))
        {
            if (parent == HunterMovement.localPlayer)
            {
                parent.AddGoodShot();
            }
            col.GetComponent<BigfootController>().TakeDamage(damage);
            DestroySelf();
        }
        else if (col.tag.Equals("Player"))
        {
            if (parent == col.GetComponent<HunterMovement>()) return;
            if (parent == HunterMovement.localPlayer)
            {
                parent.AddFriendlyShoot();
            }
            Vector3 vec = col.transform.position - transform.position;
            vec.Normalize();
            col.GetComponent<HunterMovement>().Stun(vec);
            DestroySelf();
        }
    }
}
