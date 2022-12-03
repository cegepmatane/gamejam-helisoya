using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RotateTowardMouse : MonoBehaviour
{
    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 perpendicular = transform.position - mousePos;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, perpendicular);
    }
}
