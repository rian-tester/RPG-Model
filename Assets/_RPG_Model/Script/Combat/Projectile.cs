using RPG.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float speed = 1;
        [SerializeField] bool isHoming = true;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifeTime = 10f;
        [SerializeField] GameObject[] destroyOnHit = null;
        [SerializeField] float lifeAfterImpact = 2f;
        [SerializeField] UnityEvent onHit;

        Health target = null;
        GameObject instigator = null;
        float damage = 0;

        private void Start()
        {
            // Set the initial direction of the projectile
            transform.LookAt(GetAimLocation());
        }

        private void Update()
        {
            if (target == null) return;
            if (isHoming && !target.IsDead())
            {
                // Adjust the direction towards the target if homing
                transform.LookAt(GetAimLocation());
            }

            // Move the projectile forward
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            // Set the target, instigator, and damage for the projectile
            this.target = target;
            this.damage = damage;
            this.instigator = instigator;

            // Destroy the projectile after its maximum lifetime
            Destroy(gameObject, maxLifeTime);
        }

        private Vector3 GetAimLocation()
        {
            // Get the aim location, which is the center of the target's collider
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if (targetCapsule == null)
            {
                return target.transform.position;
            }

            return target.transform.position + Vector3.up * (targetCapsule.height / 2);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != target) return;
            if (target.IsDead()) return;

            // Apply damage to the target
            target.TakeDamage(instigator, damage);

            // Stop the projectile
            speed = 0;

            // Invoke the onHit event
            onHit.Invoke();

            // Instantiate the hit effect
            if (hitEffect != null)
            {
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }

            // Destroy specified objects on hit
            foreach (GameObject toDestroy in destroyOnHit)
            {
                Destroy(toDestroy);
            }

            // Destroy the projectile after a delay
            Destroy(gameObject, lifeAfterImpact);
        }
    }
}

