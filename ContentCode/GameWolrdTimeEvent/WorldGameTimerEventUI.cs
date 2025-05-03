using UnityEngine;
using lLCroweTool.TimerSystem;
using UILibrary.Text;

namespace lLCroweTool.GameWorldTimeSystem
{
    public class WorldGameTimerEventUI : MonoBehaviour
    {
        //월드타이머 UI 모듈
        //해당안에 작동되는 메서드를 구현

        public TextPanel worldDayUI;//월드회차UI
        public TextPanel worldTimeUI;//월드시간UI
        public TextPanel dayornightUI;//낮시간인지 아닌지
        public TextPanel timeRatioUI;//시간비율


        //월드회차 업데이트
        public void WorldDayUIUpdate(int worldDay)
        {
            worldDayUI.SetLabelText(string.Format("회차 : " + worldDay));
        }

        //월드시간 업데이트
        public void WorldTimeUIUpdate(int worldTime)
        {
            //임시
            worldTimeUI.SetLabelText(string.Format("시간 : " + worldTime));
            //아마그림으로 표현하는게 좋을거라판단됨
            //동그란 시계모양
        }

        //낮과밤 업데이트
        public void DayornightUIUpdate(bool isDay)
        {
            if (isDay)
            {
                dayornightUI.SetLabelText(string.Format("-= 낮 =-"));
            }
            else
            {
                dayornightUI.SetLabelText(string.Format("-= 밤 =-"));
            }
        }

        //시간비율UI업데이트
        public void TiemRatioUIUpdate(float timeSpeed)
        {
            TimerModuleManager.Instance.SetTimerScale(timeSpeed);
        }

        private void OnDestroy()
        {
            worldDayUI = null;
            worldTimeUI = null;
            dayornightUI = null;
            timeRatioUI = null;
        }
    }
}

