using UnityEngine;

namespace RPG.Saving
{
    [System.Serializable]
    public class MySeriazableVector
    {
        float x, y, z;
        public MySeriazableVector(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public Vector3 ToVector()
        {
            return new Vector3(x, y, z);
        }
    }
}
