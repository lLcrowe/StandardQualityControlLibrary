using lLCroweTool;
using System.Collections;
using UnityEngine;

namespace lLCroweTool.Sound
{
    public class SoundObject : MonoBehaviour
    {
        //오디오 작동시키는 오브젝트
        public AudioSource audioSource;


        public void Action(AudioClip audioClip)
        {
            //사운드작동
            audioSource.Stop();
            audioSource.PlayOneShot(audioClip);
        }
       
    }
}