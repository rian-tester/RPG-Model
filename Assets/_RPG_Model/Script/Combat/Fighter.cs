using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;
using GameDevTV.Utils;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] WeaponConfig defaultWeapon = null;

        Health target;
        float TimeSinceLastAttack = Mathf.Infinity;

        WeaponConfig currentWeaponConfig;
        LazyValue<Weapon> currentWeapon;

        private void Awake()
        {
            // Initialize the default weapon
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        // Lazy Setter
        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeapon);
        }

        private void Start()
        {
            // Attach the current weapon
            AttachWeapon(currentWeaponConfig);
        }

        private void Update()
        {
            TimeSinceLastAttack += Time.deltaTime;

            if (target == null) return;
            if (target.IsDead()) return;

            // if melee weapon and target is not in range, move to target
            if (!GetIsInRange(target.transform))
            {
                // Move towards the target 
                GetComponent<Mover>().MoveTo(target.transform.position, 1f);
            }
            else
            {
                // Stop moving and attack the target
                GetComponent<Mover>().Cancel();

                // Start attacking the target, run attack sequence
                AttackBehaviour();
            }
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            // Equip the specified weapon
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            // Attach the weapon to the character
            Animator animator = GetComponent<Animator>();
            return weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        public Health GetTarget()
        {
            return target;
        }

        public void SetTarget(Health target)
        {
            this.target = target;
        }

        // Purely for animation purposes not related to trigger damage
        private void AttackBehaviour()
        {
            // Face the target and attack if the time between attacks has passed
            transform.LookAt(target.transform);

            if (TimeSinceLastAttack > timeBetweenAttacks)
            {
                TriggerAttack();
                TimeSinceLastAttack = 0;
            }
        }

        // Make sure play animation properly
        private void TriggerAttack()
        {
            // if it is currently on Attack state make sure to go back to Locomotion
            GetComponent<Animator>().ResetTrigger("stopAttack");

            // Trigger the attack animation
            GetComponent<Animator>().SetTrigger("attack");
        }

        // Once Hit event in meele attack is called, it will call this function
        void Hit()
        {
            if (target == null) return;

            float damage = GetComponent<BaseStats>().GetStat(Stat.BaseDamage);

            // Melee weapon fire OnHit unity event to play sound
            if (currentWeapon.value != null)
            {
                currentWeapon.value.OnHit();
            }

            // Ranged weapon
            if (currentWeaponConfig.HasProjectile())
            {
                // Launch the projectile and handle registering damage on projectile class (if collider collides)
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
            }
            else
            {
                // Melee weapon make sure to register the damage
                target.TakeDamage(gameObject, damage);
            }
        }

        // // Once Shoot event in ranged attack is called, it will call this function
        void Shoot()
        {
            Hit();
        }

        public bool GetIsInRange(Transform targetTransform)
        {
            // Check if the target is within range
            return Vector3.Distance(transform.position, target.transform.position) < currentWeaponConfig.GetRange();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            // if no attack target, cancel operation with false
            if (combatTarget == null)
            {
                return false;
            }

            // if there is a way to move to target, and target is not in attack range, cancel operation with false
            if (!GetComponent<Mover>().CanMoveTo(combatTarget.transform.position) && !GetIsInRange(combatTarget.transform))
            {
                return false;
            }

            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public void Attack(GameObject combatTarget)
        {
            // Start attacking the specified target
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            // Cancel the attack
            StopAttack();

            // removing target
            target = null;

            // stop moving
            GetComponent<Mover>().Cancel();
        }

        private void StopAttack()
        {
            // Stop the attack animation
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.BaseDamage)
            {
                yield return currentWeaponConfig.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.BaseDamage)
            {
                yield return currentWeaponConfig.GetPercentageBonus();
            }
        }

        public object CaptureState()
        {
            // Save the current weapon state
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            // Restore the weapon state
            string weaponName = (string)state;
            WeaponConfig weapon = Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }
    }
}


