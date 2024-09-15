using UnityEngine;

namespace lLCroweTool.RaderSystem
{
    [CreateAssetMenu(fileName = "New RaderPingInfo", menuName = "lLcroweTool/RaderPingInfo")]
    public class RaderPingInfo : ScriptableObject
    {
        //20240606
        //새로제작//새로운 에디터로 제작
        public RaderPingData raderPingData = new RaderPingData();
    }

    [System.Serializable]
    public class RaderPingData 
    {
        //규칙
        //음수일시 기타
        //양수일시 체크
        public int pingTag;
        public PingPreset passPreset = new();//동일
        public PingPreset blockPreset = new();//동일하지않음//이름체크바람
        public PingPreset unknownPreset = new();//기타//알수없는
    }

    [System.Serializable]
    public class PingPreset
    {
        public Color pingColor = Color.green;
        public Sprite pingSprite;
        public float time;
        public float pingStartScale;
        public float pingEndScale;
        public AudioClip audioClip;
    }
}