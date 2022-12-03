using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HunterMovement : NetworkBehaviour
{
    [SyncVar]
    private int currentAmmo;

    [SerializeField] private int maxAmmo;

    [SerializeField] private float speed;

    Vector3 move;

    void Start()
    {
        currentAmmo = maxAmmo;
    }


    void Update()
    {
        move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        transform.position += move * speed * Time.deltaTime;
    }


}
