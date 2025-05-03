using UnityEngine;

namespace lLCroweTool.AutoAnimation2D
{
    /// <summary>
    /// 싸인웨이브를 애니메이션해서 보여줄수 있게하는 기능을 가짐
    /// </summary>
    public class SinWaveObject : MonoBehaviour
    {
        [Header("싸인웨이브 파워설정")]
        [SerializeField] private bool isWave = false;   //웨이브여부
        [SerializeField] private bool isY = false;      //Y축여부

        //주기 설정
        [SerializeField] private float amplitude = 1;
        [SerializeField] private float frequency = 1;

        //외부에서 사용할 변수들
        [SerializeField] private float power = 1;       //외부로 받는 파워체크

        
        void Update()
        {
            //작동여부
            if (!isWave)
            {
                return;
            }

            //0보다 작으면 진행안함
            if (power < 0.001f)
            {
                return;
            }

            //Y축여부
            if (isY)
            {   
                transform.position += transform.right * lLcroweUtil.SinWave(amplitude * power, frequency) * Time.deltaTime;
            }
            else
            {
                transform.position += transform.up * lLcroweUtil.SinWave(amplitude * power, frequency) * Time.deltaTime;
            }
        }

        /// <summary>
        /// 파워 세팅함수
        /// </summary>
        /// <param name="value">파워값</param>
        public void SetPower(float value)
        {
            power = value;
        }
    }
}