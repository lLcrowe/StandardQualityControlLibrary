using System.Collections;
using TMPro;
using UnityEngine;

namespace lLCroweTool.QuestSystem
{
    public class QuestMissionCheckerUI : MonoBehaviour
    {   
        //미션체커를 보여주는 UI
        //vertical로 가로로 정렬해줘야함
        //미리캐싱해줘야함
        [SerializeField] private TextMeshProUGUI missionNameText;//미션이름
        [SerializeField] private TextMeshProUGUI missionGoalText;//목표 수량
        [SerializeField] private TextMeshProUGUI missionCountText;//현재 수량
        [SerializeField] [HideInInspector] private int missionCountValue;//현재 수량

        /// <summary>
        /// 미션이름을 정해주는 함수
        /// </summary>
        /// <param name="name">이름</param>
        public void SetMissionNameText(string name)
        {
            missionNameText.text = name;
        }

        /// <summary>
        /// 미션 목표텍스트를 세팅해주는 함수
        /// </summary>
        /// <param name="checkCountValue">목표수량</param>        
        public void SetMissionCheckText(int checkCountValue)
        {
            missionGoalText.text = checkCountValue + "/";
        }

        /// <summary>
        /// 미션 현재수량을 세팅해주는 함수
        /// </summary>
        /// <param name="countValue">현재수량</param>
        public void SetMissionCountValue(int countValue)
        {
            missionCountText.text = countValue.ToString();
            missionCountValue = countValue;
        }

        /// <summary>
        /// 이름, 체크텍스트중에 선택해서 색깔과 알파값을 변경시키는것
        /// </summary>
        /// <param name="_isNameText"></param>
        /// <param name="_color"></param>
        /// <param name="_alpha"></param>
        public void SetTextColor(bool _isNameText, Color _color, float _alpha)
        {
            TextMeshProUGUI target = _isNameText ? missionNameText : missionGoalText;
            _color.a = _alpha;
            target.color = _color;
        }

        /// <summary>
        /// 미션의 카운트수를 가져오는 함수
        /// </summary>
        /// <returns>현 카운트수</returns>
        public int GetCountValue()
        {
            return missionCountValue;
        }
    }
}