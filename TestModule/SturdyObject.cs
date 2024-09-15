using lLCroweTool;
using UnityEngine;

public class SturdyObject : MonoBehaviour
{
    //캣멀룸으로 튼실탄실한거 만들기
    //튼실튼실 푸슛푸슛
    //러프로 줄어드는 위치와 서는 위치처리
    //특정콜라이더로 특정영역에 들어가면
    //특정경로로 입출구 가능하게 처리


    //IK처리는 에셋사용

    public float weight;
    public Transform posTr1;
    public Transform posTr2;
    public Transform[] boneArray = new Transform[0];

    
    void Update()
    {
        Vector3 pos1 = posTr1.position;
        Vector3 pos2 = posTr2.position;
        Vector3 pos = Vector3.Lerp(pos1, pos2, weight);

        int length = boneArray.Length;
        for (int i = 0; i < boneArray.Length; i++)
        {

             var value = lLcroweUtil.MinMaxNormalize(0, length, i);

             //lLcroweUtil.CatmullRomSpline(pos1,,, pos1,);
        }
    }
}
