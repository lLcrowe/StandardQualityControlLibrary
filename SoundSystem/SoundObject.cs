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
            //float time = Time.time;
            //float length = audioSource.clip.length;
            var tr = transform;
            do
            {   
                if (followObject != null)
                {
                    tr.position = followObject.position;
                }
                yield return null;
                //} while (Time.time < length + time);
            } while (audioSource.isPlaying);

                
            followObject = null;
            gameObject.SetActive(false);
        }

        private IEnumerator FadeOutAndDisable(SoundObject soundObject, float duration = 0.5f)
        {
            var audioSource = soundObject.audioSource;
            if (audioSource == null) yield break;

            float startVolume = audioSource.volume;
            float time = 0f;

            while (time < duration)
            {
                if (!audioSource.isPlaying) break;

                time += Time.unscaledDeltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
                yield return null;
            }

            audioSource.Stop();
            audioSource.volume = startVolume; // 원복
            soundObject.SetActive(false);
        }

    }
}