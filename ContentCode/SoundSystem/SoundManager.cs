using lLCroweTool.Dictionary;
using lLCroweTool.Singleton;
using System.Collections;
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

        public int limitSoundObjectAmount;
        public Queue<SoundObject> activeSoundObjectQueue = new Queue<SoundObject>(24);
        

        [System.Serializable]
        public class TagToSoundDataBible : CustomDictionary<string, List<AudioClip>> { }

        public TagToSoundDataBible tagToSoundDataBible = new();


        public int activeSoundAmount;

        //오랫동안 작동할거
        [Header("백그라운드뮤직")]
        public List<SoundObject> soundObjectList = new();

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

            StartCoroutine(SoundLimitCoroutine());
        }


        public void RegisterTagToSoundData(in string tag, in AudioClip[] audioClipArray)
        {
            for (int i = 0; i < audioClipArray.Length; i++)
            {
                var clip = audioClipArray[i];
                RegisterTagToSoundData(tag, clip);
            }
        }

        public void RegisterTagToSoundData(in string tag, in AudioClip audioClip)
        {             
            if (audioClip == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(tag))
            {
                return;
            }

            //등록
            if (!tagToSoundDataBible.TryGetValue(tag, out var clipList))
            {
                List<AudioClip> audioClipList = new();
                var targetTag = lLcroweUtil.CheckAndAddReternal(tag);
                tagToSoundDataBible.Add(targetTag, audioClipList);
                

                clipList = audioClipList;
            }

            //중복처리
            if (clipList.Contains(audioClip))
            {
                return;
            }
            clipList.Add(audioClip);
        }

        public void PlayTagToSound(in string tag, Vector3 pos, in Transform attackTr)
        {
            if (!tagToSoundDataBible.TryGetValue(tag, out var clipList))
            {
                return;
            }

            //랜덤클립가져와서 재생//테스트하기
            var clip = clipList[Random.Range(0, clipList.Count)];            
            PlaySound3DAtVector3(clip, pos, attackTr);
        }


        //리쿼스트를 여기서하고
        //에셋연동도 여기서하도록 한번래핑
        public void PlaySound3DAtTransform(AudioClip audioClip, Transform attackTr)
        {   
            PlaySound3DAtVector3(audioClip, attackTr.position, attackTr);


            //if (lLcroweUtil.CheckDistance(audioListenerTr.position, attackTransform.position))
            //{

            //}
            //MasterAudio.PlaySound3DAtTransform(_weaponModule.GetActionEquipmentPartData().jamSound, _weaponModule.attackTransform);
        }

        public void PlaySound3DAtVector3(AudioClip audioClip, Vector3 pos, Transform attackTr)
        {
            if (ReferenceEquals(null, audioClip))
            {
                return;
            }
            var target = ObjectPoolManager.Instance.RequestDynamicComponentObject(soundObjectPrefab);
            activeSoundObjectQueue.Enqueue(target);
            target.followObject = attackTr;            


            //사운드매니저 하위에 배치
            target.InitTrObjPrefab(pos, transform);

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

        private IEnumerator SoundLimitCoroutine()
        {
            while (true)
            {
                activeSoundAmount = activeSoundObjectQueue.Count;
                
                //일정수량이상이면 작동//아니면 넘어감
                if (activeSoundObjectQueue.Count > limitSoundObjectAmount)
                {   
                    var sound = activeSoundObjectQueue.Dequeue();

                    //비활성화된오브젝트는 넘어감
                    if (!sound.isActiveAndEnabled)
                    {
                        continue;
                    }

                    //큐는 먼저싸인거 순서로 작동되므로 활성화되있으면 비활성화처리
                    sound.SetActive(false);
                }
                yield return null;
            }
            
        }
    }
}