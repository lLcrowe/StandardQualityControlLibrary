//using System.Collections;
//using UnityEngine;

//namespace lLCroweTool.Visual
//{
//    public class GearVisualObject : MonoBehaviour
//    {
//        //기어 오브젝트
//        //기어 비에 따른 회전을 계산해줌
//        // 기어비라고 하는것은 단순하게 표현하면,

//        //        엔진이 한번 회전할때 바퀴가 몇번 회전하는가를 비율로 나타낸 것입니다.

//        //        기어변속은 바퀴쪽 기어를 크기별로 바꿔주는 것이죠

//        //엔진이 한번   회전할때 바퀴가  1번 회전하면   기어비는 1대1이 되는것이죠.
//        //엔진이  1.5번 회전할때  바퀴가  1번 회전하면                1.5:1
//        //엔진이   2번 회전할때  바퀴가  1번 회전하면                2:1   이죠.

//        //기어비 표시는 " 1.5 "처럼 보통  앞부분만 표시하기도 합니다.

//        //기어비가 크면 가속성능이 좋아집니다.  


//        //기어비 1.5보다 1.7이 가속성능이 좋아집니다.
//        //그러나, 같은 배기량(또는 같은 출력)이라면 최고속도는 줄어들겠죠.

//        //엔진이 1.7번 회전해야 바퀴가 1번 회전하는것보다
//        //엔진    1.5회전때 바퀴가 1회전하는것이 속도가 빠르겠죠.


//        //module m = 10;
//        //z = 50
//        //a = 20도

//        //d = m * z = 50

//        //da = m * (z+2) = 52   

//        //draw the 3basic
//        //circles
//        //d, da, df

//        //moment = force * distance
//        //torque = force * length



//        //주회전 톱니바퀴 = drivergear
//        //근처의 회전되는 톱니바퀴 = drivengear


//        //회전공식
//        //driven size / driver size = gear ratio
//        //speed = size * size

//        //결과값
//        //드라이븐크기가 10이고 드라이버크기가 20이면 1대2 기어비율로
//        //10 / 20 = 1:2

//        //기어비율에맞게 톱니바퀴수도 같음//드라이븐이 8개면 드라이버는 16개

//        //outputSpeed = drinven gear = 1;//출력 나오는 스피드는 1
//        //inputSpeed = driver gear = 2 ;//받는스피드는 2

//        //톱니바퀴수는

//        //힘을 받을시 반대로 돌아감



//        public GearVisualObject[] nearGearVisualObjectArray;
        
//        public float size = 1;

//        public bool isDrivergear;
//        public float gearSpeed = 1;
        

        

//        // Update is called once per frame
//        void Update()
//        {           
//            transform.RotateAround(transform.position,Vector3.forward, gearSpeed * Time.deltaTime);
//            for (int i = 0; i < nearGearVisualObjectArray.Length; i++)
//            {
//                if (!nearGearVisualObjectArray[i].isDrivergear)
//                {

//                }
//                else
//                {
//                    nearGearVisualObjectArray[i].SetGearSpeed(-gearSpeed * nearGearVisualObjectArray[i].GetGearSize() * size);
//                }
//            }
//        }

//        public void SetGearSpeed(float speed)
//        {
//            gearSpeed = speed;
//        }

//        public float GetGearSize()
//        {
//            return size;
//        }
//    }
//}