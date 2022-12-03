using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RotateTowardMouse : MonoBehaviour
{
    [SerializeField] private float stunRotationSpeed;
    void Update()
    {

        if (GameGUI.instance.paused) return;


        if (HunterMovement.localPlayer.stuned)
        {
            Vector3 angles = transform.eulerAngles;
            angles.z += stunRotationSpeed * Time.deltaTime;
            transform.eulerAngles = angles;
        }
        else
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 perpendicular = transform.position - mousePos;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, perpendicular);
        }
    }
}
