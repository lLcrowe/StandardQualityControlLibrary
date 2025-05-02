#if CommandKey
using lLCroweTool.Dictionary;
using lLCroweTool.TimerSystem;
using System.Collections.Generic;
using UnityEngine;

namespace lLCroweTool.InputKey
{

    //20250303
    //기본올드 키버전으로 처리
    //심볼로 처리

    //20250429
    //string으로 키이름같은걸 받아서 처리하는 방식으로 변경
    //그래야지만 구지 키코드를 매번 직접 바꿀필요없이 변경가능
    //성능을 좀 감소시키더라도 string으로 변경하여 귀찮음 방지
    //런타임 조합이면 리터럴이 아님
    //리터널 + 리터널은 리터널(상수폴)로 됨
    //intern 폴에 집어넣어서 처리


    /// <summary>
    /// 커맨드 키 데이터
    /// </summary>
    [System.Serializable]
    public class CommandKeyData
    {
        /// <summary>
        /// 커맨드 이름
        /// </summary>
        public string commandName;

        /// <summary>
        /// 커맨드로 쓸 키순서들
        /// </summary>
        public string[] keyArray = new string[0];

        /// <summary>
        /// 커맨드가 작동될때 액션
        /// </summary>
        public System.Action action;
    }


    /// <summary>
    /// 커맨드 시스템. 인풋킷시스템에서 작동시켜준다
    /// </summary>
    [System.Serializable]
    public class CommandSystem
    {
        public bool isUseMenualCheck;//수동으로 체크할지 여부//헬다이버즈2같은 류
        public TimerModule_Element checkTimer;//시간으로 커맨드를 체크
        public List<string> keyCodeList = new (5);//입력키코드
        [System.Serializable] public class CommandBible : CustomDictionary<string, CommandKeyData> { };
        public CommandBible commandBible = new();

        /// <summary>
        /// 커맨드시스템 초기화
        /// </summary>
        /// <param name="commandDataArray">커맨드 키 배열들</param>
        public void Init(in CommandKeyData[] commandDataArray)
        {
            checkTimer.SetTimer(0.3f);
            var temp = typeof(CommandSystem);
            LogSystem.LogManager.Register(temp, temp.Name, true, true);


            //커맨드 등록
            commandBible.Clear();
            for (int i = 0; i < commandDataArray.Length; i++)
            {
                var index = i;
                var commandData = commandDataArray[index];

                //리터널로 등록
                var combineKey = string.Join("", commandData.keyArray);
                combineKey = CheckAndAddReternal(combineKey);

                commandBible.Add(combineKey, commandData);
            }
        }

        public void InputKey(in string keyName)
        {
            keyCodeList.Add(keyName);
            checkTimer.ResetTime();
        }

        public void Update()
        {
            if (isUseMenualCheck)
            {
                return;
            }

            if (!checkTimer.CheckTimer())
            {
                return;
            }

            //분할처리
            ResetInputList(); 
            ActionCommandKey();
        }

        /// <summary>
        /// 리터널(상수폴)에 등록
        /// </summary>
        /// <param name="content">컨텐츠</param>
        /// <returns>컨텐츠</returns>
        private string CheckAndAddReternal(in string content)
        {
            //비었는지 체크
            if (string.IsNullOrEmpty(content))
            {
                return content;
            }

            //등록됫는지
            if (string.IsInterned(content) != null)
            {
                return content;
            }

            //등록
            var newContent = string.Intern(content);
            return newContent;
        }

        public void ResetInputList()
        {
            //if (keyCodes.Count < 3)
            //{
            //    return;
            //}

            //List<CommandKey> splitInputKey = new List<CommandKey>();//자르는 키값들
            //CommandKey[] keys = new CommandKey[3];//값비교를 위한 선언
            //bool isFindSkill = false;
           

            ////커맨드리스트에 있는 커맨드스킬과 비교하여 같으면 스킬 발동
            ////리스트안의 값을 꺼내서 값을 비교함 //값비교 //잘됨
            //bool[] IsRights = new bool[3];
            //for (int i = 0; i < commandDataArray.Length; i++)
            //{
            //    //초기화
            //    isFindSkill = true;
            //    for (int j = 0; j < IsRights.Length; j++)
            //    {
            //        IsRights[j] = false;
            //    }

            //    //커맨드키 리스트값을 가져와서는 비교
            //    //해당키값마다 비교해서 맞는지 틀린지 체크하는 구역
            //    for (int j = 0; j < commandDataArray[i].commandkeys.Length; j++)
            //    {
            //        if (keys[j] == commandDataArray[i].commandkeys[j])
            //        {
            //            IsRights[j] = true;
            //        }
            //    }

            //    //집어넣은 키값들이 다 옳으면 스킬을 찾은걸로 간주
            //    for (int j = 0; j < IsRights.Length; j++)
            //    {
            //        if (!IsRights[j])
            //        {
            //            isFindSkill = false;
            //        }
            //    }

            //    //스킬을 찾았으면 빠져나오고 스킬발동
            //    if (isFindSkill)
            //    {
            //        //기능구현 //해당번호의 커맨드스킬 반환 or 작동
            //        //string 값을 반환시키는게 좋아보인다.
            //        //ActiveSkill(commandDataArray[i].targetSkillData.targetSkillPrefab);
            //        Debug.Log(i - 1 + " 번째에 스킬이 있습니다.");

            //        break;
            //    }
            //}
            //splitInputKey.Clear();

            //keyCodes.Clear();
        }

        /// <summary>
        /// 커맨드 데이터 작동
        /// </summary>
        private void ActionCommandKey()
        {
            //입력키 체크
            var listCount = keyCodeList.Count;
            if (listCount < 1)
            {
                return;
            }

            //메뉴얼이 아니면 특정 수만큼 자름
            if (!isUseMenualCheck)
            {
                //잘라버릴 수량
                int spriteNum = 3;

                //입력한 키에서 특정 수량까지 자르기
                if (spriteNum < listCount)
                {
                    int diff = listCount - spriteNum;
                    for (int i = 0; i < diff; i++)
                    {
                        //뒤에 자르기
                        keyCodeList.RemoveAt(keyCodeList.Count - 1);
                    }
                }
            }

            //이어붙혀서 키값 제작
            var combineKey = string.Join("", keyCodeList);

            if (commandBible.ContainsKey(combineKey))
            {
                //스킬작동
                //LogSystem.LogManager.Log(typeof(CommandSystemManager), "커맨드스킬작동", unitObject, LogManager.LogType.Info);

                //해당유닛으로부터 스킬이 작동되게
                Debug.Log("작동");
            }

            ////커맨드타임입력종료&&초기화
            keyCodeList.Clear();
        }
    }
}
#endif
