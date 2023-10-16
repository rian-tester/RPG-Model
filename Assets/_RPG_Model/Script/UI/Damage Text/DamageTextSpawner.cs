using UnityEngine;

namespace RPG.UI.DamageText
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] DamageText damageTextPrefab = null;

        public void Spawn(float damageAmount)
        {
            DamageText instannce =  Instantiate<DamageText>(damageTextPrefab, transform);
            instannce.SetValue(damageAmount);
        }
    }
}

