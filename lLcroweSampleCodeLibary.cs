namespace lLCroweTool.Sample
{
    /// <summary>
    /// 성능때문에 사용안하고 따로 함수나 로직에 사용할수 있게 주석된 코드를 보관하는 라이브러리
    /// </summary>
    public class lLcroweSampleCodeLibrary
    {
        //Enum타입을 변경시킬때 사용하는 함수
        //public static void SampleChangeEnumType<T>(ref T enumType, bool isNext = true) where T : struct
        //{
        //    int add = isNext ? 1 : -1;
        //    int curValue = (int)formationType;
        //    int maxValue = lLcroweUtil.GetEnumDefineLength<FormationType>();


        //    curValue += add;
        //    curValue = curValue >= maxValue ? 0 : curValue;
        //    curValue = curValue < 0 ? maxValue - 1 : curValue;
        //    formationType = (FormationType)curValue;
        //}
    }
}
