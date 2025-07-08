using lLCroweTool;
using UnityEngine;

namespace lLcrowe
{
    /// <summary>
    /// 기즈모나 색깔처리를 편하기 위해 만든 모듈
    /// 랜덤한 색깔을 설정하고 필요시 꺼내와서 처리
    /// </summary>
    
    public class ColorBoxModule : MonoBehaviour
    {
        public ColorBox colorBox = new ColorBox();

        [ButtonMethod]
        public void SetRandomColorBatch()
        {
            colorBox.SetRandomColorBatch();
        }

        public Color GetRandomColorIndex(int index)
        {
           return colorBox.GetRandomColorIndex(index);
        }
    }

    [System.Serializable]
    public class ColorBox
    {
        //이쪽 랜덤컬러박스 모듈로 만들어두기
        public int randomColorAmount = 10;
        public Color[] colorArray = new Color[0];


        public void SetRandomColorBatch()
        {
            colorArray = new Color[randomColorAmount];
            for (int i = 0; i < randomColorAmount; i++)
            {
                colorArray[i] = Random.ColorHSV();
            }
        }

        /// <summary>
        /// 랜덤컬러를 인덱스로 가져오는 함수
        /// </summary>
        /// <param name="index">인덱스</param>
        /// <returns></returns>
        public Color GetRandomColorIndex(int index)
        {
            int length = colorArray.Length - 1;

            //없을시 흰색으로 줌
            if (length < 0)
            {
                return Color.white;
            }

            //있지만 인덱스가 크면 그만큼 배열크기만큼 빼서 배열안에서 주기
            //만약 0일시 처음껄로
            if (length < index)
            {
                index = /*length == 0 ? 0 : */(index % length);
            }

            return colorArray[index];
        }
    }
}
