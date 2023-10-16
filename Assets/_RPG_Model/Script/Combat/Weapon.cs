using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{

    public class Weapon : MonoBehaviour
    {
        [SerializeField] UnityEvent onHit;
        public void OnHit()
        {
            print("Wepaon hit " + gameObject.name);
            onHit.Invoke();
        }
    }
}

