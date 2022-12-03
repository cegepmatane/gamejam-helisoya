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

    [SerializeField] private Transform feets;
    [SerializeField] private NetworkAnimator feetAnimator;
    [SerializeField] private NetworkAnimator bodyAnimator;

    Vector3 move;

    public static HunterMovement localPlayer;

    public override void OnStartClient()
    {
        if (isLocalPlayer)
        {
            localPlayer = this;
        }
        currentAmmo = maxAmmo;
    }


    void Update()
    {
        move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        transform.position += move * speed * Time.deltaTime;

        feetAnimator.animator.SetBool("moving", move != Vector3.zero);
        bodyAnimator.animator.SetBool("moving", move != Vector3.zero);

        if (move != Vector3.zero)
        {
            Quaternion toRotate = Quaternion.LookRotation(Vector3.forward, move);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotate, 720 * Time.deltaTime);
        }

    }


}
