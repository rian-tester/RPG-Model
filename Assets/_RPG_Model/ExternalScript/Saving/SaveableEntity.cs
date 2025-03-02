using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Saving
{
    [ExecuteAlways]
    public class SaveableEntity : MonoBehaviour
    {
        [SerializeField] string uniqueIdentifier = "";
        static Dictionary<string, SaveableEntity> globalLookup = new Dictionary<string, SaveableEntity>();

        public string GetUniqueIdentifier()
        {
            return uniqueIdentifier;
        }

        public object CaptureState()
        {
            // Create a new dictionary to store component states
            Dictionary<string, object> state = new Dictionary<string, object>();

            // Find all components attached to this GameObject that implement ISaveable
            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                // Use the component's type name as the key in our dictionary
                // This ensures we can match states back to their components during restore
                // Store whatever data the component returns from its own CaptureState method
                state[saveable.GetType().ToString()] = saveable.CaptureState();
            }
            return state;
        }

        public void RestoreState(object state)
        {
            // Cast the generic object back to the dictionary structure we created in CaptureState
            Dictionary<string, object> stateDict = (Dictionary<string, object>)state;

            // Find all components attached to this GameObject that implement ISaveable
            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                // Get the type name to use as lookup key in our dictionary
                string typeString = saveable.GetType().ToString();

                // Check if we have saved state for this specific component type
                if (stateDict.ContainsKey(typeString))
                {
                    // Pass the component's saved state back to its own RestoreState method
                    // This allows each component to handle its own data restoration
                    saveable.RestoreState(stateDict[typeString]);
                }
                // If no state exists for this component, we simply skip it
                // (component will use its default values)
            }
        }

#if UNITY_EDITOR
        private void Update() {
            if (Application.IsPlaying(gameObject)) return;
            if (string.IsNullOrEmpty(gameObject.scene.path)) return;

            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty property = serializedObject.FindProperty("uniqueIdentifier");
            
            if (string.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue))
            {
                property.stringValue = System.Guid.NewGuid().ToString();
                serializedObject.ApplyModifiedProperties();
            }

            globalLookup[property.stringValue] = this;
        }
#endif

        private bool IsUnique(string candidate)
        {
            if (!globalLookup.ContainsKey(candidate)) return true;

            if (globalLookup[candidate] == this) return true;

            if (globalLookup[candidate] == null)
            {
                globalLookup.Remove(candidate);
                return true;
            }

            if (globalLookup[candidate].GetUniqueIdentifier() != candidate)
            {
                globalLookup.Remove(candidate);
                return true;
            }

            return false;
        }
    }
}