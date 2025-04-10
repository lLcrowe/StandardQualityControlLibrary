using lLCroweTool.Singleton;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace lLCroweTool.Sound
{
    public class SoundManager : MonoBehaviourSingleton<SoundManager>
    {
        public AudioListener audioListener;
        public Transform audioListenerTr;
        public SoundObject soundObjectPrefab;
        public float distance;

        public Queue<SoundObject> activeSoundObjectQueue = new Queue<SoundObject>(24);

        protected override void Init()
        {
            audioListener = FindFirstObjectByType<AudioListener>(FindObjectsInactive.Include);
            audioListenerTr = audioListener.transform;

            if (soundObjectPrefab == null)
            {
                var go = new GameObject("Sound");
                soundObjectPrefab = go.AddComponent<SoundObject>();
                soundObjectPrefab.transform.parent = transform;
            }
        }

        //리쿼스트를 여기서하고
        //에셋연동도 여기서하도록 한번래핑
        public void PlaySound3DAtTransform(AudioClip audioClip, Transform transform)
        {
            PlaySound3DAtVector3(audioClip, transform.position);


            //if (lLcroweUtil.CheckDistance(audioListenerTr.position, transform.position))
            //{

            //}
            //MasterAudio.PlaySound3DAtTransform(_weaponModule.GetActionEquipmentPartData().jamSound, _weaponModule.transform);
        }

        public void PlaySound3DAtVector3(AudioClip audioClip, Vector3 pos)
        {
            if (ReferenceEquals(null, audioClip) )
            {
                return;
            }
            var target = ObjectPoolManager.Instance.RequestDynamicComponentObject(soundObjectPrefab);
            activeSoundObjectQueue.Enqueue(target);

            if (activeSoundObjectQueue.Count > 24)
            {
                do
                {
                    var sound =  activeSoundObjectQueue.Dequeue();
                    if (!sound.isActiveAndEnabled)
                    {
                        continue;
                    }

                    sound.SetActive(false);

                    if (activeSoundObjectQueue.Count > 8)
                    {
                        continue;
                    }
                    break;

                } while (true);
            }
            


            target.transform.SetParent(transform);
            target.InitTrObjPrefab(pos,transform);

#if SteamAudio
            InitSteamAudio(target.audioSource, audioClip);
#endif

            target.Action(audioClip);
        }

#if SteamAudio
        public void InitSteamAudio(in AudioSource audioSource, in AudioClip audioClip)
        {
            //스팀오디오일시//클립에 적용할려면 Ambisonic;
            audioSource.spatialize = true;
            audioSource.TryGetComponent(out SteamAudio.SteamAudioSource steamAudioSource);
            steamAudioSource.airAbsorption = true;//거리에 따른 공기흡후

            //사운드의 지향성패턴처리//사운드의 방향철
            steamAudioSource.directivity = true;
            return;

        }
#endif

#if MasterAudio


        public void PlaySound3DAtTransform(AudioClip audioClip, Transform transform)
        {
            //MasterAudio.PlaySound3DAtTransform(_weaponModule.GetActionEquipmentPartData().jamSound, _weaponModule.transform);
        }

        public void PlaySound3DAtTransform(string audioName, Transform transform)
        {
            //MasterAudio.PlaySound3DAtTransform(_weaponModule.GetActionEquipmentPartData().jamSound, _weaponModule.transform);
        }

        internal void PlaySound3DAtVector3(string value, Vector2 hitWorldPos)
        {
            //MasterAudio.PlaySound3DAtVector3(attackBox.projectilePartData.reflectSoundArray[index], collision2D.contacts[0].point);
        }
#endif


    }
}