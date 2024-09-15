using lLCroweTool.Singleton;
using System;
using System.Collections;
using UnityEngine;

namespace lLCroweTool.Sound
{
    public class SoundManager : MonoBehaviourSingleton<SoundManager>
    {
        protected override void Init()
        {
            
        }

        //리쿼스트를 여기서하고
        //에셋연동도 여기서하도록 한번래핑
        public static void PlaySound3DAtTransform(AudioClip audioClip, Transform transform)
        {
            //MasterAudio.PlaySound3DAtTransform(_weaponModule.GetActionEquipmentPartData().jamSound, _weaponModule.transform);
        }

        public static void PlaySound3DAtVector3(AudioClip audioClip, Vector3 pos)
        {

        }

#if MasterAudio


        public static void PlaySound3DAtTransform(AudioClip audioClip, Transform transform)
        {
            //MasterAudio.PlaySound3DAtTransform(_weaponModule.GetActionEquipmentPartData().jamSound, _weaponModule.transform);
        }

        public static void PlaySound3DAtTransform(string audioName, Transform transform)
        {
            //MasterAudio.PlaySound3DAtTransform(_weaponModule.GetActionEquipmentPartData().jamSound, _weaponModule.transform);
        }

        internal static void PlaySound3DAtVector3(string value, Vector2 hitWorldPos)
        {
            //MasterAudio.PlaySound3DAtVector3(attackBox.projectilePartData.reflectSoundArray[index], collision2D.contacts[0].point);
        }
#endif
    }
}