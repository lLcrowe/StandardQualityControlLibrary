#if MEC
namespace lLCroweTool.DialogueSystem
{
    [System.Serializable]
    public class DialogueInterectLogData
    {
        //20220611
        //대화를 할때 기록할 데이터들//무엇이 필요할까
        public string dialogueIDKey;//대화키
        public int dialogueActionCount = 0;//대화를 얼마나 한 카운트//2회차 3회차일때는 스킵가능하게

        //해당대화데이터에서 가져올 프로퍼티값들
        public CustomPropertyBool[] customPropertyBoolArray = new CustomPropertyBool[0];
        public CustomPropertyFloat[] customPropertyFloatArray = new CustomPropertyFloat[0];
        public CustomPropertyInt[] customPropertyIntArray = new CustomPropertyInt[0];
        public CustomPropertyString[] customPropertyStringArray = new CustomPropertyString[0];
    }
}
#endif