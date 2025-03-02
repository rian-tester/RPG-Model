using UnityEngine;

namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            const float waypointGizmosRadius = 0.2f;

            for (int i = 0; i < transform.childCount; i++)
            {
                int j = GetNextIndex(i);

                Gizmos.DrawSphere(transform.GetChild(i).position, waypointGizmosRadius);
                Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(j));

            }
        }

        public int GetNextIndex(int index)
        {
            if( index + 1 >= transform.childCount)
            {
                return 0;
            }
            return index + 1;
        }

        public Vector3 GetWaypoint(int index) 
        {
            return transform.GetChild(index).position;
        }
    }
}

