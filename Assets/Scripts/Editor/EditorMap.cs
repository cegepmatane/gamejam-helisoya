using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorMap : Editor {

    public PerlinNoiseMap map;

    private void OnSceneGUI() {
        if (Event.current.control) {
            
        }
    }

    private void drawGrid() {
        Vector3 _position = new Vector3(-0.5f, -0.5f, 0f);
        float lenght;
        Vector3 t_D, t_A;
        Gizmos.color = Color.white;
        // vertical lignes
        for (int i = 0; i <= map.map_width; i++) {
            lenght = map.map_height;
            t_D = _position + (Vector3.right * i);
            t_A = t_D + (Vector3.up * lenght); 
            Gizmos.DrawLine(t_D, t_A);
        }
        // horizontal lignes
        for (int i = 0; i <= map.map_height; i++) {
            lenght = map.map_width;
            t_D = _position + (Vector3.up * i );
            t_A = t_D + (Vector3.right * lenght);
            Gizmos.DrawLine(t_D, t_A);
        }
    }
}
