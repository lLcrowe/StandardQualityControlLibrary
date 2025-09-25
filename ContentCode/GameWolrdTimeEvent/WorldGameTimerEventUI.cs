using UnityEngine;

namespace lLCroweTool.GameWorldTimeSystem
{
    public class WorldGameTimerEventUI : MonoBehaviour
    {
        //월드타이머 UI 모듈
        //해당안에 작동되는 메서드를 구현


#if UILibrary
        public UILibrary.Text.TextPanel worldDayUI;//월드회차UI
        public UILibrary.Text.TextPanel worldTimeUI;//월드시간UI
        public UILibrary.Text.TextPanel dayornightUI;//낮시간인지 아닌지
        public UILibrary.Text.TextPanel timeRatioUI;//시간비율
#endif
        public event System.Action<float> timeSpeedAction;


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

        private void OnDestroy()
        {
            worldDayUI = null;
            worldTimeUI = null;
            dayornightUI = null;
            timeRatioUI = null;
        }
    }
}

