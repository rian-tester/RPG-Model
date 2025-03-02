using UnityEngine;
using RPG.Attributes;
using RPG.Control;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        // Behaviour specific for combat target
        public bool HandleRaycast(PlayerController callingController)
        {
            // Check if the player can attack this target
            if (!callingController.GetComponent<Fighter>().CanAttack(gameObject))
            {
                return false;
            }

            // If the left mouse button is clicked, attack the target
            if (Input.GetMouseButton(0))
            {
                callingController.GetComponent<Fighter>().Attack(gameObject);
            }

            return true;
        }

        public CursorType GetCursorType()
        {
            // Return the cursor type for combat
            return CursorType.Combat;
        }
    }
}
