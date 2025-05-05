using System.Collections;
using UnityEngine;

namespace lLCroweTool.Sound
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundObject : MonoBehaviour
    {
        //오디오 작동시키는 오브젝트
        public AudioSource audioSource;
        public Transform followObject;

        public void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void Action(AudioClip audioClip)
        {
            //사운드작동
            audioSource.clip = audioClip;
            StartCoroutine(DeActive());
            
            audioSource.Stop();
            audioSource.PlayOneShot(audioClip);
        }
        private IEnumerator DeActive()
        {
            float time = Time.time;
            float length = audioSource.clip.length;
            var tr = transform;
            do
            {   
                if (followObject != null)
                {
                    tr.position = followObject.position;
                }
                yield return null;                
            } while (Time.time < length + time);

            followObject = null;
            gameObject.SetActive(false);
        }
       
    }
}