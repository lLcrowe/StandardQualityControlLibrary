
namespace lLCroweTool.InputKey.Define
{
    /// <summary>
    /// 여기서 프로젝트 게임에 쓸 인풋에 대한 걸 설정하여 집어넣음
    /// </summary>
    public static class InputKeyDefine
    {
        //샘플코드

        //키 정의
        public static KeyData[] keyDataArray = new KeyData[]
        {
                new KeyData() { keyName = "LeftKey", inputKeyType = InputKeyType.KeyPress, },
                new KeyData() { keyName = "RightKey", inputKeyType = InputKeyType.KeyPress },
                new KeyData() { keyName = "UpKey", inputKeyType = InputKeyType.KeyPress },
                new KeyData() { keyName = "DownKey", inputKeyType = InputKeyType.KeyPress },
                new KeyData() { keyName = "Fire", inputKeyType = InputKeyType.KeyPress },
        };


#if CommandKey
        //커맨드 정의


#endif
    }
}
