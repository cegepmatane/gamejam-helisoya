using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimCursor : MonoBehaviour
{
    void Update()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        transform.position = pos;
    }
}
