using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AmmoBox : NetworkBehaviour
{
    public float amplitude;          //Set in Inspector 
    public float speed;                  //Set in Inspector 
    private float tempVal;
    private Vector3 tempPos;
    [SerializeField] private int ammoNumber = 20;


    // Start is called before the first frame update
    void Start()
    {
        tempVal = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        tempPos.y = tempVal + amplitude * Mathf.Sin(speed * Time.time);
        transform.position = tempPos;
    }


    public int GetAmmo()
    {
        return ammoNumber;
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && collision.GetComponent<HunterMovement>().isLocalPlayer)
        {
            HunterMovement.localPlayer.AddAmmoFromBox(this);
        }
    }


}
