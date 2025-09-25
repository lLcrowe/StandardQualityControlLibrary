using lLCroweTool.Dictionary;
using UnityEngine;

namespace lLCroweTool.ScoreSystem.UI
{
    public class UnitScoreUI : MonoBehaviour
    {
        //유닛에 대한 점수를 시각적으로 보여주는 역할을 맏음
        //점수판(ScoreBoard)에서 사용
        public sealed class UIActionBible : CustomDictionary<ScoreType, UIActionGroup> { }

        public sealed class UIActionGroup
        {
            ~UIActionGroup()
            {
                SetSpriteAction = null;
                InitAction = null;
                SetAction = null;
            }

            public System.Action<Sprite> SetSpriteAction;
            public System.Action<float, float, float> InitAction;
            public System.Action<float> SetAction;
        }

        //아이콘들
        public event System.Action<Sprite> classIconAction;
        public event System.Action<Sprite> unitIconAction;


        //UI
        public UIActionBible uiActionBible = new();


        private void Awake()
        {
            for (int i = 0; i < ScoreManager.ScoreTypeArray.Length; i++)
            {
                var scoreType = ScoreManager.ScoreTypeArray[i];

                UIActionGroup uIActionGroup = new();
                uiActionBible.Add(scoreType, uIActionGroup);
            }
        }

        private void OnDestroy()
        {
            uiActionBible.Clear();
        }


        public void BlindUIAction(ScoreType scoreType, System.Action<float, float, float> initAction, System.Action<float> setAction)
        {
            if (!uiActionBible.TryGetValue(scoreType, out var data))
            {
                data = new UIActionGroup();
                uiActionBible[scoreType] = data;
            }

            data.InitAction = initAction;
            data.SetAction = setAction;
        }

        /// <summary>
        /// 유닛스코어 UI초기화
        /// </summary>
        /// <param name="classIconSprite">클래스아이콘</param>
        /// <param name="unitIconSprite">유닛아이콘</param>
        /// <param name="maxValue">최대수치</param>
        public void InitUnitScoreUI(Sprite classIconSprite, Sprite unitIconSprite, int maxValue)
        {
            //
            classIconAction?.Invoke(classIconSprite);
            unitIconAction?.Invoke(unitIconSprite);

            //UI바 초기화
            foreach (var item in uiActionBible)
            {
                var data = item.Value;
                data.InitAction(0, maxValue, 0);
            }
        }

        public void SetScoreValue(ScoreType scoreType,  int scoreValue) 
        {
            if (!uiActionBible.TryGetValue(scoreType, out var data))
            {
                return;
            }

            data.SetAction.Invoke(scoreValue);
        }

    }
}