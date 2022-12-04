using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class PathFinder : MonoBehaviour
{

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
    private RaycastHit raycastHitDirection;

    public void setMap(PerlinNoiseMap map)
    {
        this.map = map;
        Targets = map.getTargets();
    }

    public void setSpeed(float speed)
    {
        this.speed = speed;
    }

    public Vector3 getMouvmentVector(Vector3 _currentPos)
    {
        if (Mathf.Abs((Targets[currentTarget] - _currentPos).magnitude) <= 0.1f)
        {
            currentTarget = (currentTarget + 1) % Targets.Count;
            if (currentTarget == 0)
            {
                shuffleTargets();
            }
        }

        // ----- def directorVector ----- //
        vectorDirector = Targets[currentTarget] - _currentPos;
        // get the vector normal of the direction
        vectorDirector = vectorDirector.normalized;


        // Trace Ray

        // ----- check collistion ----- //

        Vector3 nextPos = _currentPos + vectorDirector * speed * Time.deltaTime;

        if (map.impassibleTilemap.HasTile(new Vector3Int(Mathf.FloorToInt(nextPos.x), Mathf.FloorToInt(nextPos.y), 0)))
        {
            // Hit therefore => seek new directorVector

            // Check if last vectorDirector is good

            Vector3 lastPosDirector = _currentPos + lastVectorDirector * speed * Time.deltaTime;

            if (map.impassibleTilemap.HasTile(new Vector3Int(Mathf.FloorToInt(lastPosDirector.x), Mathf.FloorToInt(lastPosDirector.y), 0)))
            {

                // no lastVectorDirector or an invalid one so we need to calculate a new one

                // --- Calculate new VectorDirector --- //
                // Rotate Director vector arround Z axis
                // Z axis Vector3.forward
                Vector3 _tempRotatedDirectorVectort = vectorDirector;

                // we start from the lastVectorDirector if possible since it was working on last iteration 
                if (lastVectorDirector != null)
                {
                    _tempRotatedDirectorVectort = lastVectorDirector;
                }

                // rotation from -10 degres
                _tempRotatedDirectorVectort = Quaternion.AngleAxis(-10f, Vector3.forward) * _tempRotatedDirectorVectort;

                // Rotate the vector until the raycast doesn't it anything

                Vector3 newPodRotated = _currentPos + _tempRotatedDirectorVectort * speed * Time.deltaTime;

                /*
                while (map.impassibleTilemap.HasTile(new Vector3Int(Mathf.FloorToInt(newPodRotated.x), Mathf.FloorToInt(newPodRotated.y), 0)))
                {
                    _tempRotatedDirectorVectort = Quaternion.AngleAxis(-10f, Vector3.forward) * _tempRotatedDirectorVectort;
                    newPodRotated = _currentPos + _tempRotatedDirectorVectort * speed;
                }*/

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

    private void shuffleTargets()
    {
        for (int i = 0; i < Targets.Count; i++)
        {
            Vector3 _temp = Targets[i];
            int randomIndex = Random.Range(i, Targets.Count);
            Targets[i] = Targets[randomIndex];
            Targets[randomIndex] = _temp;
        }


    }
    public void OnDrawGizmosSelected()
    {

        // Draw Full path
        Vector3 _tempVector3 = Targets[0];
        Gizmos.color = Color.white;
        foreach (Vector3 Target in Targets)
        {
            Gizmos.DrawLine(_tempVector3, Target);
            _tempVector3 = Target;
        }
        // From last to first target
        Gizmos.DrawLine(_tempVector3, Targets[0]);


        if (Physics.Raycast(transform.position, vectorDirector, out raycastHitDirection, speed))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + (vectorDirector * raycastHitDirection.distance));
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + (vectorDirector * speed));
        }
    }
}
