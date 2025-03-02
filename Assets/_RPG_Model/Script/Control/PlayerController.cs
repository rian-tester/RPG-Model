using UnityEngine;
using RPG.Movement;
using RPG.Attributes;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control
{
    [System.Serializable]
    struct CursorMapping
    {
        public CursorType cursorType;
        public Vector2 hotspot;
        public Texture2D texture;
    }

    public class PlayerController : MonoBehaviour
    {
        Health health;
        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float maxMeshProjectionDistance = 1f;
        [SerializeField] float raycastRadius = 1f;

        private void Awake()
        {
            health = GetComponent<Health>();
        }

        private void Update()
        {
            // When player clicking UI cancel operation
            if (InteractWithUI()) return;

            // When player is dead, cancel operation
            if (health.IsDead())
            {
                SetCursor(CursorType.None);
                return;
            }

            // When player notice gameobject that can be interacted (enemies and items)
            if (InteractWithComponent()) return;

            // When player click on walkable area
            if (InteractWithMovement()) return;

            // If all previous condition false, set cursor to default
            SetCursor(CursorType.None);
        }
        
        // Specific for pointer UI icon
        private bool InteractWithUI()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                SetCursor(CursorType.UI); 
                return true;
            }

            return false;
        }

        // Identifying object in the scene that has IRaycastable, return true gameobject can be interacted
        private bool InteractWithComponent()
        {
            // Make list that contain all raycasted object (sphere radius)
            RaycastHit[] hits = RaycastAllSorted();

            foreach (RaycastHit hit in hits) 
            {
                // Iterate from index 0 whoever has raycastable component
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();

                // Iterate from index 0 from all raycastable
                foreach (IRaycastable raycastable in raycastables)
                {
                    // Do handling the raycast depends on each raycastable behaviour
                    if (raycastable.HandleRaycast(this))
                    {
                        // Set cursor by using each cursor type
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }

            return false;
        }

        // Helper function for InteractWithComponent
        RaycastHit[] RaycastAllSorted()
        {
            // Get all colliders in the scene within the raycast radius
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);

            // Build array of distances from hits array total number
            float[] distances = new float[hits.Length];

            // Iterate the hits array and store the distance
            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }

            // Sort the distances
            Array.Sort(distances, hits);

            // Return hits array already sorted from nearest to farthest
            return hits;
        }

        // Decide to walk or not, movement entry point
        private bool InteractWithMovement()
        {
            //RaycastHit hit;
            //bool hasHit = Physics.Raycast(GetMouseRay(), out hit);

            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);

            // hasHit true meaning we have walkable area
            if (hasHit)
            {
                // If its not walkable cancel operation
                if (!GetComponent<Mover>().CanMoveTo(target)) return false;
                
                // If its walkable and left click, start moving
                if (Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(target, 1f);
                }
                // Set cursor to movement and cancel operation
                SetCursor(CursorType.Movement);
                return true;
            }

            // Just cancel operation since nothing happened
            return false;
        }

        // Define walkable area
        private bool RaycastNavMesh(out Vector3 target)
        {
            // Step 1: Cast a ray from camera through mouse position into the world
            target = new Vector3();

            // Step 1: Cast a ray from camera through mouse position into the world
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (!hasHit) return false; // Exit if ray didn't hit any collider

            // Step 2: Find the nearest valid NavMesh point to where the ray hit
            NavMeshHit navMeshHit;
            bool hasCastToNavMesh =  NavMesh.SamplePosition(
                hit.point,                      // The world point to start searching from
                out navMeshHit,                 // Container for the result
                maxMeshProjectionDistance,      // Maximum distance to search for NavMesh
                NavMesh.AllAreas);              // Consider all types of NavMesh areas

            // Exit if no NavMesh found nearby
            if (!hasCastToNavMesh) return false;

            // Success! Update the target with the valid position and return true
            target = navMeshHit.position;

            return true;
        }

        // Define cursor type based on raycast hit (walk, items, combat, etc)
        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            UnityEngine.Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        // Helper function for SetCursor
        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach(CursorMapping mapping in cursorMappings)
            {
                if (mapping.cursorType == type)
                {
                    return mapping;
                }
            }

            return cursorMappings[0];
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

    }
}


