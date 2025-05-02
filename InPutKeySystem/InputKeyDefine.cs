using lLCroweTool.InputKey;
using UnityEngine;

[DefaultExecutionOrder(-900)]
/// <summary>
/// 프로젝트 게임에 쓸 인풋키에 대한 걸 설정하여 집어넣음
/// </summary>
public abstract class InputKeyDefine : MonoBehaviour
{
    private void Awake()
    {
        Init();
    }

    /// <summary>
    /// 초기화
    /// </summary>
    protected virtual void Init()
    {
        //키 등록
        InPutKeySystem.Instance.InitInputSetting(GetKeyDataArray());

#if CommandKey
        //커맨드키 등록
        InPutKeySystem.Instance.InitInputCommandSetting(GetCommandKeyData());
#endif
    }


    /// <summary>
    /// 키데이터를 가져오는 함수
    /// </summary>
    /// <returns></returns>
    public abstract KeyData[] GetKeyDataArray();

#if CommandKey
        /// <summary>
        /// 커맨드 키데이터를 가져오는 함수
        /// </summary>
        /// <returns></returns>
        public abstract CommandKeyData[] GetCommandKeyData();

#endif

    #region 샘플코드        
    //public override CommandKeyData[] GetCommandKeyData()
    //{
    //    return new[]
    //    {
    //        new CommandKeyData(){commandName = "Skill1", keyArray = new[]{ "LeftKey", "LeftKey", "Fire"}},
    //        new CommandKeyData(){commandName = "Skill2", keyArray = new[]{ "RightKey", "RightKey",""}},
    //        new CommandKeyData(){commandName = "Skill3", keyArray = new[]{ "Fire", "Fire", ""}},
    //    };
    //}

    //public override KeyData[] GetKeyDataArray()
    //{
    //    return new KeyData[]
    //    {
    //        new KeyData() { keyName = "LeftKey", inputKeyType = InputKeyType.KeyPress, },
    //        new KeyData() { keyName = "RightKey", inputKeyType = InputKeyType.KeyPress },
    //        new KeyData() { keyName = "UpKey", inputKeyType = InputKeyType.KeyPress },
    //        new KeyData() { keyName = "DownKey", inputKeyType = InputKeyType.KeyPress },
    //        new KeyData() { keyName = "Fire", inputKeyType = InputKeyType.KeyPress },
    //    };
    //}
    #endregion
}
