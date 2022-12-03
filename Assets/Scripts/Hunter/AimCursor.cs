using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimCursor : MonoBehaviour
{

    [SerializeField] private SpriteRenderer sprite;
    void Update()
    {
        if (GameGUI.instance.paused)
        {
            sprite.color = Color.clear;
            return;
        }
        else
        {
            sprite.color = Color.white;
        }
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        transform.position = pos;
    }
}
