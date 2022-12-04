using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoTrigger : MonoBehaviour
{
    public AmmoBox box;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player") && collision.GetComponent<HunterMovement>().isLocalPlayer)
        {
            HunterMovement.localPlayer.AddAmmoFromBox(box);
        }
    }
}
