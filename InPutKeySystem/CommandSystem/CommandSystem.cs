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

    public class CommandSystem
    {
        public TimerModule_Element timer;
        public List<string> keyCodeList = new ();//입력키코드
        [System.Serializable] public class CommandBible : CustomDictionary<string[], System.Action> { };
        public CommandBible commandBible = new();

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

        public void Init()
        {
            timer.SetTimer(0.3f);
            var temp = typeof(CommandSystem);
            LogSystem.LogManager.Register(temp, temp.Name, true, true);
        }

        public void SetCommandDataArray(CommandKeyData[] commandDataArray)
        {
            //키체크
            //커맨드 체크
            commandBible.Clear();
            for (int i = 0; i < commandDataArray.Length; i++)
            {
                commandBible.Add(commandDataArray[i].keyArray, commandDataArray[i].action);
            }
        }

        public void InputKey(in string keyName)
        {
            keyCodeList.Add(keyName);
            timer.ResetTime();
        }

        public void Update()
        {
            if (!timer.CheckTimer())
            {
                return;
            }

            //분할처리
            ResetInputList(); 
            CheckCommandKey();
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
            //int spriteNum = 3;
            ////입력한 키코드에서 3번째까지만 잘라서 집어넣는곳
            //for (int i = 0; i < keyCodes.Count; i++)
            //{
            //    if (i > spriteNum - 1)
            //    {
            //        break;
            //    }
            //    keys[i] = keyCodes[i];
            //    splitInputKey.Add(keyCodes[i]);
            //}

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

            ////커맨드타임입력종료&&초기화
            //keyCodes.Clear();
        }


        //이거 작동되는건가? 이상하네
        private void CheckCommandKey()
        {
            //커맨드키관련체크
            if (keyCodeList.Count < 3)
            {
                return;
            }
            if (commandBible.ContainsKey(keyCodeList.ToArray()))
            {
                //스킬작동
                //LogSystem.LogManager.Log(typeof(CommandSystemManager), "커맨드스킬작동", unitObject, LogManager.LogType.Info);

                //해당유닛으로부터 스킬이 작동되게
                Debug.Log("작동");
            }
            keyCodeList.Clear();
        }
    }
}
#endif
