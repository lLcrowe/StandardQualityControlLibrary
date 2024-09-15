using UnityEngine;

namespace lLCroweTool
{
    //제약조건이 있는 오브젝트들은 해당 클래스를 참조할것
    [System.Serializable]
    public sealed class ConstraintsCondition
    {  
        public bool useTagCondition;
        [Tag] public string[] interectTags = new string[0];
      
        public bool useLayerCondition;
        public LayerMask interectLayer;

        //20220726
        //ContactFilter2D에서 레이어처리를 간단히할수있다
        //근데왜 레이어마스크를 배열로 했지?
        //20231112
        //없던함수를 지원해줘서 적어둔것

        ~ConstraintsCondition()
        {
            interectTags = null;
            //interectLayers = null;
        }
    }

    public static class ConstraintsConditionUtil
    {
        /// <summary>
        /// 게임오브젝트의 태그가 원하는 태그들중에 있는 체크하는 함수
        /// </summary>
        /// <param name="_gameObject">타겟된 게임오브젝트</param>
        /// <param name="_interectTags">상호작용 태그들</param>
        /// <returns>조건에 맞는가?</returns>
        public static bool FitConditionTag(this GameObject _gameObject, string[] _interectTags)//여러 원하는태그중에서 찾아서 있는지 체크하는 함수 //수정함 20190920//20210511수정
        {
            bool isExist = false;
            //태그[]배열로 했을시interectTag.Lengh
            //리스트로 했을시 interectTag.Count
            for (int i = 0; i < _interectTags.Length; i++)
            {
                if (_gameObject.CompareTag(_interectTags[i]))
                {
                    isExist = true;
                    //return returnTag;
                    break;
                }
            }
            return isExist;
        }

        /// <summary>
        /// 게잉오브젝트의 레이어가 원하는 레이어들중에 있는 체크하는 함수
        /// </summary>
        /// <param name="_gameObject">타겟된 게임오브젝트</param>
        /// <param name="_interectLayer">상호작용 레이어들</param>
        /// <returns>조건에 맞는가?</returns>
        public static bool FitConditionLayer(this GameObject _gameObject, LayerMask _interectLayer)//원하는 레이어가 여러개일 경우 사용원하는 레이어를 반환해서 있는지 체크//신규제작 20200827//20210511수정
        {
            bool isExist = _gameObject.layer == _interectLayer;
            //태그[]배열로 했을시interectTag.Lengh
            //리스트로 했을시 interectTag.Count
            return isExist;
        }

        //조건에 맞는가
        //사용법
        //if(FitCondition)

        /// <summary>
        /// 게임오브젝트의 레이어와 태그를 체크하는 함수
        /// </summary>
        /// <param name="_gameObject">타겟된 게임오브젝트</param>
        /// <param name="constraintsCondition">타겟된 제약조건</param>
        /// <returns></returns>
        public static bool FitConditionAll(this GameObject _gameObject, ConstraintsCondition constraintsCondition)
        {
            //20221111//추가
            bool isRight = false;

            //신규 제작 20200827
            //수정20210511

            //레이어체크
            if (constraintsCondition.useLayerCondition)
            {
                //이거체크하기
                if (!(isRight = FitConditionLayer(_gameObject, constraintsCondition.interectLayer)))
                {
                    return isRight;
                }
            }
            else
            {
                isRight = true;
            }

            //태그 체크
            if (constraintsCondition.useTagCondition)
            {
                isRight = FitConditionTag(_gameObject, constraintsCondition.interectTags);
            }
            return isRight;
        }

        /// <summary>
        /// 게임오브젝트의 레이어와 태그를 체크하는 함수
        /// </summary>
        /// <param name="_gameObject">타겟된 게임오브젝트</param>
        /// <param name="useLayerCondition">레이어 체크여부</param>
        /// <param name="interectLayer">체크할 레이어들</param>
        /// <param name="useTagCondition">태그 체크여부</param>
        /// <param name="interectTag">체크할 태그들</param>
        /// <returns>조건에 맞는가?</returns>
        public static bool FitConditionAll(this GameObject _gameObject, bool _useLayerCondition, int _interectLayer, bool _useTagCondition, string[] _interectTag)
        {
            bool isRight = false;

            //신규 제작 20200827
            //수정20210511

            //레이어체크
            if (_useLayerCondition)
            {
                if (!(isRight = FitConditionLayer(_gameObject, _interectLayer)))
                {
                    return isRight;
                }
            }
            else
            {
                isRight = true;
            }

            //태그 체크
            if (_useTagCondition)
            {
                isRight = FitConditionTag(_gameObject, _interectTag);
            }
            return isRight;
        }

        /// <summary>
        /// 게임오브젝트의 레이어와 태그를 체크하는 함수
        /// </summary>
        /// <param name="_gameObject">타겟된 게임오브젝트</param>
        /// <param name="_useLayerCondition">레이어 체크여부</param>
        /// <param name="_interectLayer">체크할 레이어들</param>
        /// <param name="_useTagCondition">태그 체크여부</param>
        /// <param name="_interectTag">체크할 태그들</param>
        /// <returns>조건에 맞는가?</returns>
        public static bool FitConditionAll(this GameObject _gameObject, bool _useLayerCondition, LayerMask _interectLayer, bool _useTagCondition, string[] _interectTag)
        {
            bool isRight = false;

            //신규 제작 20200827
            //수정20210511

            //레이어체크
            if (_useLayerCondition)
            {
                if (!(isRight = FitConditionLayer(_gameObject, _interectLayer)))
                {
                    return isRight;
                }
            }
            else
            {
                isRight = true;
            }

            //태그 체크
            if (_useTagCondition)
            {
                isRight = FitConditionTag(_gameObject, _interectTag);
            }
            return isRight;
        }
    }
}

