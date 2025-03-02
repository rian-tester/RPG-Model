using RPG.Core;
using UnityEngine;
using UnityEngine.AI;
using RPG.Saving;
using RPG.Attributes;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace RPG.Movement
{

    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] Transform target;
        [SerializeField] float maxSpeed = 6f;
        [SerializeField] float maxNavPathLength = 40f;

        NavMeshAgent navMeshAgent;
        Health health;

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }

        private void Start()
        {
            // WriteFileTest();

            // Debug.Log(Application.persistentDataPath);
        }

        void Update()
        {
            navMeshAgent.enabled = !health.IsDead();

            UpdateAnimator();

            // Debug.Log(ReadFileTest());
        }

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedFraction);
        }

        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);

            if (!hasPath) return false;
            if (path.status != NavMeshPathStatus.PathComplete) return false;

            if (GetPathLength(path) > maxNavPathLength) return false;

            return true;
        }

        private float GetPathLength(NavMeshPath path)
        {
            float total = 0;
            if (path.corners.Length < 2) return total;

            for (int i = 0; i < path.corners.Length -1; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }

            return total;
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            navMeshAgent.destination = destination;
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            navMeshAgent.isStopped = false;
        }

        public void Cancel()
        {
            // Tell navmesh agent to stop 
            navMeshAgent.isStopped = true;
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            GetComponent<Animator>().SetFloat("forwardSpeed", speed);
        }

        // specific save purposes
        [System.Serializable]
        struct MoverSaveData
        {
            public SerializableVector3 position;
            public SerializableVector3 rotation;
        }

        public object CaptureState()
        {
 
            // struct version
            MoverSaveData data = new MoverSaveData();
            data.position = new SerializableVector3(transform.position);
            data.rotation = new SerializableVector3(transform.eulerAngles);

            // Dictionary version
            //Dictionary<string, object> data = new Dictionary<string, object>();
            //data["rect"] = new SerializableVector3(transform.rect);
            //data["rotation"] = new SerializableVector3(transform.eulerAngles);
            return data;
        }

        public void RestoreState(object state)
        {
            MoverSaveData data = (MoverSaveData) state;
            //Dictionary<string, object> data = (Dictionary<string, object>)state;

            // Weird behaviour from Navmesh
            GetComponent<NavMeshAgent>().enabled = false;

            // Struct version
            transform.position = data.position.ToVector();
            transform.eulerAngles = data.rotation.ToVector();

            // Dictionary version
            //transform.rect = ((SerializableVector3)data["rect"]).ToVector();
            //transform.eulerAngles = ((SerializableVector3)data["rotation"]).ToVector();

            GetComponent<NavMeshAgent>().enabled = true;
        }


        #region Serialization
        string testFileName = "testingfile";
        private void WriteFileTest()
        {
            string path = Path.Combine(Application.persistentDataPath, testFileName + ".sav");
            using (FileStream stream = File.Open(path, FileMode.Create))
            {
                //byte[] bytes = Encoding.UTF8.GetBytes("This game is awesome!");
                //stream.WriteByte(0x47);
                //stream.WriteByte(0x65);
                //stream.WriteByte(0x72);
                //stream.WriteByte(0x69);
                //stream.WriteByte(0x6e);
                //stream.WriteByte(0x64);
                //stream.WriteByte(0x72);
                //stream.WriteByte(0x61);

                //Vector3 playerTransform = GetPlayerTransform().transform.position;
                //byte[] buffer = SerializeVector(playerTransform);
                //stream.Write(buffer, 0, buffer.Length);

                Vector3 playerTransform = GetPlayerTransform().transform.position;
                BinaryFormatter formatter = new BinaryFormatter();
                MySeriazableVector position = new MySeriazableVector(playerTransform);
                formatter.Serialize(stream, position);
            }
        }

        private string ReadFileTest()
        {
            string path = Path.Combine(Application.persistentDataPath, testFileName + ".sav");
            using (FileStream stream = File.Open(path, FileMode.Open))
            {
                //byte[] buffer = new byte[stream.Length];
                //stream.Read(buffer, 0, buffer.Length);

                //string message = Encoding.UTF8.GetString(buffer);

                //string message = DeserializeVector(buffer).ToString();
                //return message;

                BinaryFormatter formatter = new BinaryFormatter();

                string message = "";
                MySeriazableVector position = (MySeriazableVector)formatter.Deserialize(stream);
                message = position.ToVector().ToString();
                return message;
            }
        }

        private byte[] SerializeVector(Vector3 vector)
        {
            // because float take 4 bytes so its multiply by 3 (x.y.z)
            byte[] vectorBytes = new byte[3 * 4];
            BitConverter.GetBytes(vector.x).CopyTo(vectorBytes, 0);
            BitConverter.GetBytes(vector.y).CopyTo(vectorBytes, 4);
            BitConverter.GetBytes(vector.z).CopyTo(vectorBytes, 8);
            return vectorBytes;
        }

        private Vector3 DeserializeVector(byte[] buffer)
        {
            Vector3 result = new Vector3();
            result.x = BitConverter.ToSingle(buffer, 0);
            result.y = BitConverter.ToSingle(buffer, 4);
            result.z = BitConverter.ToSingle(buffer, 8);
            return result;
        }
        private Transform GetPlayerTransform()
        {
            return gameObject.transform;
        }
        #endregion
    }
}

