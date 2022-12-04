using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class PathFinder : MonoBehaviour {
    
    // targets
    private List<Vector3> Targets;
    private int currentTarget = 0;
    
    // map
    public PerlinNoiseMap map;
    
    // Bigfoot
    [SerializeField] private float speed;
    
    // Movement Components
    private Vector3 vectorDirector = Vector3.zero;
    private Vector3 lastVectorDirector;
    private Vector3 movementVector;
    // LayerMask seems to be useless since RayCast ignore colider if tey started inside it. 
    //private LayerMask layerMask = ~(1 << 1);

    public void setMap(PerlinNoiseMap map) {
        this.map = map;
        Targets = map.getTargets();
    }

    public void setSpeed(float speed) {
        this.speed = speed;
    }

    public Vector3 getMouvmentVector(Vector3 _currentPos) {
        if (Mathf.Abs((Targets[currentTarget] - _currentPos).magnitude) <= 0.1f) {
            currentTarget = (currentTarget + 1) % Targets.Count;
        }
        
        // ----- def directorVector ----- //
        vectorDirector = Targets[currentTarget] - _currentPos;
        // get the vector normal of the direction
        vectorDirector = vectorDirector.normalized;
        
        
        // Trace Ray
        RaycastHit raycastHitDirection;
        // Todo Gizmo Editor

        // ----- check collistion ----- //
        if (Physics.Raycast(_currentPos, vectorDirector, out raycastHitDirection, speed)) {
            // Hit therefore => seek new directorVector
            
            // Check if last vectorDirector is good
            if (lastVectorDirector == null || Physics.Raycast(_currentPos, lastVectorDirector, out raycastHitDirection,
                    speed)) {
                
                // no lastVectorDirector or an invalid one so we need to calculate a new one
                
                // --- Calculate new VectorDirector --- //
                // Rotate Director vector arround Z axis
                // Z axis Vector3.forward
                Vector3 _tempRotatedDirectorVectort = vectorDirector;
                
                // we start from the lastVectorDirector if possible since it was working on last iteration 
                if (lastVectorDirector != null) {
                    _tempRotatedDirectorVectort = lastVectorDirector;
                }

                // rotation from -10 degres
                _tempRotatedDirectorVectort = Quaternion.AngleAxis(-10f, Vector3.forward) * _tempRotatedDirectorVectort;

                // Rotate the vector until the raycast doesn't it anything
                while (Physics.Raycast(_currentPos, _tempRotatedDirectorVectort, out raycastHitDirection, speed)) {
                    _tempRotatedDirectorVectort = Quaternion.AngleAxis(-10f, Vector3.forward) * _tempRotatedDirectorVectort;
                }

                vectorDirector = _tempRotatedDirectorVectort;

            }
            
        }
        // no hit
            
        // movementVector depending speed and rendering.
        Vector3 movementVector = vectorDirector * (speed * Time.deltaTime);
        
        // save last vectorDirector for faster process
        lastVectorDirector = vectorDirector;
        
        return movementVector;
    }
}
