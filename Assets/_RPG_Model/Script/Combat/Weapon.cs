using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{

    public class Weapon : MonoBehaviour
    {
        // Unity Event system for playing the sound
        [SerializeField] UnityEvent onHit;

        public void OnHit()
        {
            // Print a message and invoke the onHit event
            print("Weapon hit " + gameObject.name);
            onHit.Invoke();
        }
    }
}

