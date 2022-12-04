using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PathFinder : MonoBehaviour {
    
    // targets
    private List<Vector3> Targets;
    private int currentTarget = 0;
    
    // map
    public PerlinNoiseMap map;

    public void setMap(PerlinNoiseMap map) {
        this.map = map;
    }

    public Vector3 getMouvmentVector(Vector3 _currentPos) {
        if (Targets[currentTarget] == _currentPos) {
            currentTarget = (currentTarget + 1) % Targets.Count;
        }


        Vector3 directorVector;
        
        
        
        return Vector3.zero;
    }
}
