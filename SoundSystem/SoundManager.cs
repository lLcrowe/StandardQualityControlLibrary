using lLCroweTool.Singleton;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.AI;

namespace lLCroweTool.Sound
{
    public class SoundManager : MonoBehaviourSingleton<SoundManager>
    {
        public AudioListener audioListener;
        public Transform audioListenerTr;
        public SoundObject soundObjectPrefab;
        public float distance;

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
            target.transform.SetParent(transform);
            target.InitTrObjPrefab(pos,transform);
            target.Action(audioClip);
        }

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