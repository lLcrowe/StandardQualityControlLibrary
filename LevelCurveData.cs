using System.Collections.Generic;
using UnityEngine;

namespace lLCroweTool.QC.Curve
{
    /// <summary>
    /// 레벨커브데이터(커브, 고정형데이터)
    /// </summary>
    [System.Serializable]
    public class LevelCurveData
    {
        //인트변수를 커브로 표현하기 위한 클래스
        //스킬레벨이라든가 스탯등에서 사용하기 편리를 위해 제작

        //중간값을 디테일하게 알 필요없을시

        //상당히 유용한데 잘이용하면 각 스탯에 커브를 적용
        //밸런스를 잡을때 사용할때 정말 좋은거 같다
        //전에 체크해보니 커브는 크게 두종류를 사용한다.
        //Linear 커브(일정하게 난이도 상승)과 EaseIn (가면갈수록 난이도가 상승)

        //이거 어차피 소수점으로 바꾸면 더 연산 많이드니 그냥 정수로 하는게 맞아보이는데        
        //time = maxLv(최대레벨)
        //value = 100000(maximum 경험치)
        //이거 에디터에서 처리하기

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="minLevel">최소레벨</param>
        /// <param name="maxLevel">최대레벨</param>
        /// <param name="value">최대최소 값지정</param>
        public LevelCurveData(int minLevel, int maxLevel)
        {
            MinLevel = minLevel;
            MaxLevel = maxLevel;
        }


        //레벨커브//위의 수치에 따라 변동되는 데이터
        [SerializeField] protected AnimationCurve levelCurve = new AnimationCurve(
            new Keyframe(1, 0), new Keyframe(10, value: 100));

        public AnimationCurve LevelCurve
        {
            get => levelCurve;
#if UNITY_EDITOR
            set => levelCurve = value;
#endif
        }
     
        public int MinLevel
        {
            get => (int)levelCurve.keys[0].time;
            set 
            {
                var keys = levelCurve.keys;                
                keys[0].time = value;
                levelCurve.keys = keys;
            }
        }

        public int MaxLevel
        {
            get 
            {
                var keys = levelCurve.keys;
                return (int)keys[keys.Length - 1].time;
            }
            set
            {
                var keys = levelCurve.keys;
                int index = keys.Length - 1;
                keys[index].time = value;
                levelCurve.keys = keys;
            }
        }

        public float MinValue 
        {
            get => levelCurve.keys[0].value;
            set
            {
                var keys = levelCurve.keys;
                keys[0].value = value;
                levelCurve.keys = keys;
            }
        }
        public float MaxValue 
        {
            get 
            { 
                var keys = levelCurve.keys;
                return keys[keys.Length - 1].value; 
            }
            set
            {
                var keys = levelCurve.keys;
                int index = keys.Length - 1;
                keys[index].value = value;
                levelCurve.keys = keys;
            }
        }


        /// <summary>
        /// 현재 레벨에 따른 커브값을 가져오는 함수(Float)
        /// </summary>
        /// <param name="curLevel">현재 레벨</param>
        /// <returns>값</returns>
        public float GetLevelCurveFloatValue(int curLevel)
        {
            curLevel = curLevel.GetLimitAmount(MinLevel, MaxLevel);
            return LevelCurve.Evaluate(curLevel);
        }

        /// <summary>
        /// 현재 값에 따른 커브값을 가져오는 함수(Float)
        /// </summary>
        /// <param name="curValue">현재값</param>
        /// <returns>값</returns>
        public float GetLevelCurveFloatValue(float curValue)
        {
            curValue = curValue.GetLimitAmount(MinLevel, MaxLevel);
            return LevelCurve.Evaluate(curValue);
        }

        /// <summary>
        /// 현재 레벨에 따른 커브값을 가져오는 함수(Int반환형)
        /// </summary>
        /// <param name="curLevel">현재 레벨</param>
        /// <returns>값</returns>
        public int GetLevelCurveIntValue(int curLevel)
        {
            curLevel = curLevel.GetLimitAmount(MinLevel, MaxLevel);
            return LevelCurve.Evaluate(curLevel).RoundToInt();
        }
    }
}
