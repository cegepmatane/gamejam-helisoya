using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEditor;
using UnityEngine;

//[CustomEditor(typeof(PerlinNoiseMap))]
public class EditorMap : Editor {

    public PerlinNoiseMap map;

    private void Awake() {
        map = (PerlinNoiseMap) target;
    }

    private void OnSceneGUI() {
        Debug.Log("on scene GUI");
        drawGrid();
        drawValueGen();
        /*
        if (Event.current.control) {
            drawGrid();
            drawValueGen();
        }*/
    }

    private void drawGrid() {
        Vector3 _position = new Vector3(0f, 0f, 0f);
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
    
    private void drawValueGen() {
        SyncList<List<int>> noiseGrid = map.getNoiseGrid();
        int _tempNoiseValue;
        for (int x = 0; x < map.map_width; x++) {
            for (int y = 0; y < map.map_height; y++) {
                _tempNoiseValue = noiseGrid[x][y];
                /*
        tileset.Add(0, prefab_Rock);
        tileset.Add(1, prefab_Dirt_Rock);
        tileset.Add(2, prefab_Dirt);
        tileset.Add(3, prefab_DirtyGrass);
        tileset.Add(4, prefab_Grass);
        tileset.Add(5, prefab_Grass);
        tileset.Add(6, prefab_Tree);
        tileset.Add(7, prefab_Tree);
                 */
                switch (_tempNoiseValue) {
                    case 0:
                        Gizmos.color = new Color32(150, 150, 150, 204);
                        break;
                    case 1:
                        Gizmos.color = new Color32(75, 47, 23, 204);
                        break;
                    case 2:
                        Gizmos.color = new Color32(65, 33, 7, 232);
                        break;
                    case 3:
                        Gizmos.color = new Color32(40, 51, 10, 232);
                        break;
                    case 4:
                        Gizmos.color = new Color32(29, 141, 22, 255);
                        break;
                    case 5:
                        Gizmos.color = new Color32(29, 141, 22, 255);
                        break;
                    case 6:
                        Gizmos.color = new Color32(19, 82, 15, 255);
                        break;
                    case 7:
                        Gizmos.color = new Color32(19, 82, 15, 255);
                        break;
                    default:
                        Gizmos.color = Color.magenta;
                        break;
                }
                Gizmos.DrawCube(new Vector3(x, y, 0), new Vector3(0.5f, 0.5f, 0.5f));
            }
        }
    }
}
