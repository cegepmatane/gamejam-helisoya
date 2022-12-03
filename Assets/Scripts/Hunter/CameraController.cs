using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    void Update()
    {
        if (HunterMovement.localPlayer == null) return;
        Vector3 playerPos = HunterMovement.localPlayer.transform.position;
        transform.position = new Vector3(playerPos.x, playerPos.y, -10);
    }
}
