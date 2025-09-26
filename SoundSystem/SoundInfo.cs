using UnityEngine;

namespace lLCroweTool.Sound
{    
    [CreateAssetMenu(fileName = "New SoundInfo", menuName = "lLcroweTool/New SoundInfo")]
    public class SoundInfo : ScriptableObject
    {
        public AudioClip[] audioClipArray = new AudioClip[0];


        //여러설정처리
        //소리
        //음량

    }
}
