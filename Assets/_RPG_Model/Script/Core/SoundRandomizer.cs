using UnityEngine;

namespace RPG.Core
{
    public  class SoundRandomizer : MonoBehaviour
    {
        [SerializeField] AudioClip[] damageSounds = null;
        public void SwitchSound()
        {
            if(damageSounds.Length > 0)
            {
                AudioSource audio =  GetComponent<AudioSource>();
                audio.clip = damageSounds[Random.Range(0, damageSounds.Length)];
            }
        }
    }
}
