using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace lLCroweTool
{
    // Vector3의 + 연산자 오버로딩//테스트요망//빠를까 느릴까
    //20240407
    //확장함수사용하지말기
    //아예따로빼는게 맞아보임

    //public static class Vector3ExtensionOP
    //{
    //    private static float x1;
    //    private static float y1;
    //    private static float z1;
    //    private static float x2;
    //    private static float y2;
    //    private static float z2;

    //    private static void SetFirstPara(ref this Vector3 vector)
    //    {
    //        x1 = vector.x;
    //        y1 = vector.y;
    //        z1 = vector.z;
    //    }
    //    private static void SetSecondPara(ref this Vector3 vector)
    //    {
    //        x2 = vector.x;
    //        y2 = vector.y;
    //        z2 = vector.z;
    //    }


    //    public static void Add(ref this Vector3 v1, Vector3 v2)
    //    {
    //        v1.SetFirstPara();
    //        v2.SetSecondPara();
    //        x1 += x2;
    //        y1 += y2;
    //        z1 += z2;
    //        v1 = new Vector3(x1,y1,z1);
    //    }
    //    public static void Sub(ref this Vector3 v1, Vector3 v2)
    //    {
    //        v1.SetFirstPara();
    //        v2.SetSecondPara();
    //        x1 -= x2;
    //        y1 -= y2;
    //        z1 -= z2;
    //        v1 = new Vector3(x1, y1, z1);
    //    }
    //    public static void Mul(ref this Vector3 v1, Vector3 v2)
    //    {
    //        v1.SetFirstPara();
    //        v2.SetSecondPara();
    //        x1 *= x2;
    //        y1 *= y2;
    //        z1 *= z2;
    //        v1 = new Vector3(x1, y1, z1);
    //    }
    //    public static void Div(ref this Vector3 v1, Vector3 v2)
    //    {
    //        v1.SetFirstPara();
    //        v2.SetSecondPara();
    //        x1 /= x2;
    //        y1 /= y2;
    //        z1 /= z2;
    //        v1 = new Vector3(x1, y1, z1);
    //    }
    //}


    //20240407
    //유틸을 한번 정리해야겠음
    //vector수식관련(수학)
    //곡선
    //회전
    //이동
    //물리

    /// <summary>
    /// lLcrowe 작업에 필요한 여러 유틸들
    /// </summary>
    public static class lLcroweUtil 
    {
        /// <summary>
        /// 함수에서 float형 데이터를 리턴이나 Ref할때 사용하는 값
        /// </summary>
        public static float valueF;

        /// <summary>
        /// 함수에서 int형 데이터를 리턴이나 Ref할때 사용하는 값
        /// </summary>
        public static int value;


        //  String 클래스를 사용하는 경우 
        //  -문자열을 수정하는 수가 적을 경우(stringbuilder는 string에 비해서 무시해도 좋을 수준의 성능향상을 제공하거나 전혀 제공하지 않을 수 있음)
        //  -부분적인 문자열 글자로 고정된 수의 문자열 연결 작업을 할때(컴파일러가 연결 작업을 단일 작업으로 결합할 수 있음)
        //  -문자열을 작성하는 동안 광법위한 검색 작업을 수행할 때(StringBuilder 클래스는 IndexOf 또는 StartsWith같은 함수가 없다)

        //  StringBuiler 클래스를 사용 하는 경우
        //  -응용 프로그램이 설계시에는 알 수 없는 횟수의 문자열을 변경해야 할 때(사용자의 입력등으로 조합할때 )
        //  -문자열에서 많은 횟수의 변경이 예상될때

        //---------------스트링더하기&스트링빌더원리----------------
        //txt = txt + "1";
        //1. 힙에 특정문자열을 담는 공간을 할당
        //2. 스택에 있는 txt변수에 1번과정에서 할당된 힙의 주소를 저장
        //3. txt + "1" 동작을 수행하기 위해 txt.length + "1".length에 해당되는 크기의 메모리를 힙에 할당.
        //해당 메모리에 txt변수가 가리키는 힙의 문자열과 "1"문자열을 복사한다.
        //4. 다시 스택에 있는 txt변수에 3번과정에서 새롭게 할당된 힙의 주소를 저장
        //5. 3번, 4번의 과정을 X만큼 반복

        //stringBuilder.append("1");
        //1. stringBuilder는 내부적으로 일정한 양의 메모리를 미리할당한다.
        //2. Append 메서드에 들어온 이자를 미리 할당한 메모리에 복사한다.
        //3. 2번과정을 X만큼 반복. Append로 추가된 문자열이 미리 할당한 메모리보다 많아지면 새롭게 여유분의 메모리를 할당
        //4. ToString 메서드를 호출하면 연속적으로 연결된 하나의 문자열을 반환.

        private static System.Text.StringBuilder builder = new ();

        /// <summary>
        /// 여러문자열들 결합해주는 함수
        /// </summary>
        /// <param name="strings">집어넣을 문자열들</param>
        /// <returns>합쳐진 문자열</returns>
        public static string GetCombineString(params string[] strings)
        {   
            //builder.Clear();
            builder.Length = 0;

            for (int i = 0; i < strings.Length; i++)
            {
                builder.Append(strings[i]);
            }
            return builder.ToString(); //반환
        }

        /// <summary>
        /// 컴포넌트복사
        /// </summary>
        /// <typeparam name="T">컴포넌트 타입</typeparam>
        /// <param name="original">원본컴포넌트</param>
        /// <param name="target">붙여넣을게임오브젝트</param>
        /// <returns>복사한 컴포넌트</returns>
        public static T GetCopyOf<T>(T original, GameObject target) where T : Component
        {
            System.Type type = original.GetType();
            Component copy = target.AddComponent(type);

            System.Reflection.FieldInfo[] fields = type.GetFields();

            //foreach (System.Reflection.FieldInfo field in fields)
            //{
            //    field.SetValue(copy, field.GetValue(original));
            //}

            for (value = 0; value < fields.Length; value++)
            {
                System.Reflection.FieldInfo field = fields[value];
                field.SetValue(copy, field.GetValue(original));
            } 

            return copy as T;
        }

        /// <summary>
        /// 클래스복사
        /// </summary>
        /// <typeparam name="T">클래스</typeparam>
        /// <param name="original">원본타입</param>
        /// <param name="copyTarget">복사할 new한 클래스</param>
        /// <returns>값을 복사한 클래스</returns>
        public static T GetCopyOf<T>(T original, T copyTarget) where T : class
        {
            System.Type type = original.GetType();

            System.Reflection.FieldInfo[] fields = type.GetFields();

            foreach (System.Reflection.FieldInfo field in fields)
            {
                field.SetValue(copyTarget, field.GetValue(original));
            }
            
            return copyTarget as T;
        }

        //public static T GetCopyOf<T>(T target) where T : class
        //{   
        //    //작동되나 테스트해봐야됨
        //    object tempObject = target.Clone();
        //    return tempObject as T;
        //}

         public static void GetAddComponent<T>(this Component component, out T outComponent) where T : Component
         {
             GetAddComponent(component.gameObject, out outComponent);
         }

         public static void GetAddComponent<T>(this GameObject go, out T component) where T : Component
         {
             if (!go.TryGetComponent(out component))
             {
                 component = gameObject.AddComponent<T>();
             }
         }

        /// <summary>
        /// 해당 게임오브젝트에서 컴포넌트를 추가하거나 찾는 함수
        /// </summary>
        /// <typeparam name="T">추가하거나 찾을 컴포넌트타입</typeparam>
        /// <param name="go">타겟팅할 게임오브젝트</param>
        /// <returns>찾은 컴포넌트</returns>
        public static T GetAddComponent<T>(this GameObject go) where T : Component
        {
            if (!go.TryGetComponent(out T component))
            {
                component = go.AddComponent<T>();
            }
            return component;
        }

        public static T GetAddComponent<T>(this Component component) where T : Component
        {   
            return component.gameObject.GetAddComponent<T>();
        }

        /// <summary>
        /// 유니티이벤트를 체크하여 없으면 집어넣는 함수
        /// </summary>
        /// <param name="unityEvent">유니티이벤트</param>
        public static void GetAddUnitEvent(ref UnityEvent unityEvent)
        {
            //if (unityEvent == null)
            if (ReferenceEquals(unityEvent, null))
            {
                unityEvent = new UnityEvent();
            }
        }


        //https://daveoh.wordpress.com/2013/05/02/unity3d-vector3-magnitude-vs-sqrmagnitude/
        //https://answers.unity.com/questions/307612/inversetargetpoint-vs-vector3distance-which-is.html
        //https://answers.unity.com/questions/384932/best-way-to-find-distance.html
        //https://answers.unity.com/questions/125882/vectordistance-performance.html

        //PC(Intel Core i5, 10 million executions per run per function, averaged result over 10 runs)
        //sqrMagnitude: 2853 ms. (96.13% of magnitude’s time)
        //magnitude: 2968 ms. (104.03% of sqrMagnitude’s time)

        //Android(Samsung Galaxy S II, 10 million executions per run per function, averaged result over 10 runs)
        //sqrMagnitude: 6155 ms. (77.11% of magnitude’s time)
        //magnitude: 7982 ms. (129.68% of sqrMagnitude’s time)

        //명시적 형변환과 묵시적 형변환의 차이점
        //*결과적인 차이는 없다
        //*명시적 형변환의 경우 내부적으로 임시변수를 생성에 대입하는 방식으로 성능 저하를 일으킬 수 있다.
        //*묵시적 형변환의 경우 데이터 손실에 대한 경고가 발생한다.
        //CLR(공용 언어 런타임)은 값 형식을 boxing할 때 값을 System.Object 인스턴스 내부에 래핑하고 관리되는 힙에 저장합니다.
        //unboxing하면 개체에서 값 형식이 추출됩니다.
        //Boxing은 암시적이며 unboxing은 명시적입니다.
        //단순 할당에서는 boxing과 unboxing을 수행하는 데 많은 계산 과정이 필요합니다.
        //값 형식을 boxing할 때는 새로운 개체를 할당하고 생성해야 합니다. <=값 형식이 boxing되면 완전히 새로운 개체가 생성되어야 합니다.
        //정도는 약간 덜하지만 unboxing에 필요한 캐스트에도 상당한 계산 과정이 필요합니다
        //Boxing 및 unboxing은 계산을 많이 해야 하는 프로세스입니다. 
        //이 작업은 단순 참조 할당보다 20배나 오래 걸립니다.
        //unboxing 시 캐스팅 프로세스는 할당의 4배에 달하는 시간이 소요될 수 있습니다.
        //성능속도: 박싱(느림) < 언박싱 < 단순할당(빠름)
        //박싱 언박싱 참고할것
        //박싱
        //int a = 1;
        //object b = a;

        //public enum STATE{A = 1, B = 2,}
        //STATE a = STATE.A;
        //STATE b = STATE.B;
        //// Enum 비교 시 a, b가 boxing이 발생한다
        //if (a.Equals(b)) { }
        //int ia = 1;
        //int ib = 2;
        //// 단순 enum 비교로 boxing이 발생하지 않는다
        //if (ia.Equals(ib)) { }

        //boxing은 암시적으로 사용되고 있기 때문에 주의 깊게 생각하고 사용하지 않으면 생각보다 많은 곳에서 발생하게 된다.
        //꼭 필요한 곳에서 사용하는 것은 어쩔 수 없지만, 불필요한 곳에서 사용하게 되는 경우 성능 저하를 발생 시키기 때문에 유의해야한다.
        //=>언제한번 전체다 처리해버려야겠는걸. 결국 값형식으로 비교하면 Equal 할때 GC가 안쌓인다 라는것이군
        //=>ilspy로보니 Equals이 해당값과 object가 있다. object로 가면 GC가 쌓임
        //=> == 로 할시 ceq가 작동됨. equals로 할시 boxing callvirt 진행
        //ceq 스택의 두 값을 꺼내고, 같으면 1, 다르면 0을 스택에 넣는다.


        //타임.타임만 메인스레드말고는 작동안함//유니티가 동기화하면서 하면서 안되는 스레드들이 있다.//랜덤도 안됨//티슈에서 뽑아쓰듯이


        //CMP 연산자
        //0040150E 메모리주소
        //C74424 오퍼레이션코드 
        //Dword 4byte 
        //PTR 포인트
        //stack segment 
        //: []
        //오피코드 확인//exe. CPU가 적힘

        //JMP 점프
        //JNZ 두개의 연산을 해가주고 
        //SS 소스타겟

        //0040150E  |.  C74424 0C 000>MOV DWORD PTR SS:[ESP+C],0
        //00401516  |.  C74424 08 000>MOV DWORD PTR SS:[ESP+8],0

        //0040151E  |.  837C24 0C 01  CMP DWORD PTR SS:[ESP+C],1
        //00401523  |.  75 0A JNZ SHORT a.0040152F
        //00401525  |.  C74424 08 010>MOV DWORD PTR SS:[ESP+8],1
        //0040152D  |.  EB 2A JMP SHORT a.00401559

        //0040152F  |>  837C24 0C 02  CMP DWORD PTR SS:[ESP+C],2
        //00401534  |.  75 0A JNZ SHORT a.00401540
        //00401536  |.  C74424 08 020>MOV DWORD PTR SS:[ESP+8],2
        //0040153E  |.  EB 19         JMP SHORT a.00401559

        //00401540  |>  837C24 0C 00  CMP DWORD PTR SS:[ESP+C],0
        //00401545  |.  75 0A JNZ SHORT a.00401551
        //00401547  |.  C74424 08 030>MOV DWORD PTR SS:[ESP+8],3
        //0040154F  |.  EB 08         JMP SHORT a.00401559

        //00401551  |>  C74424 08 040>MOV DWORD PTR SS:[ESP+8],4


        //00401559  |>  C74424 0C 000>MOV DWORD PTR SS:[ESP+C],0
        //00401561  |.  C74424 08 000>MOV DWORD PTR SS:[ESP+8],0


        //00401569  |.  8B4424 0C MOV EAX,DWORD PTR SS:[ESP+C]
        //0040156D  |.  83F8 01       CMP EAX,1
        //00401570  |.  74 0B         JE SHORT a.0040157D
        //00401572  |.  83F8 02       CMP EAX,2
        //00401575  |.  74 10         JE SHORT a.00401587
        //00401577  |.  85C0 TEST EAX,EAX
        //00401579  |.  74 16         JE SHORT a.00401591
        //0040157B  |.  EB 1E         JMP SHORT a.0040159B
        //0040157D  |>  C74424 08 010>MOV DWORD PTR SS:[ESP+8],1
        //00401585  |.  EB 1C JMP SHORT a.004015A3
        //00401587  |>  C74424 08 020>MOV DWORD PTR SS:[ESP+8],2
        //0040158F  |.  EB 12         JMP SHORT a.004015A3
        //00401591  |>  C74424 08 030>MOV DWORD PTR SS:[ESP+8],3
        //00401599  |.  EB 08         JMP SHORT a.004015A3
        //0040159B  |>  C74424 08 040>MOV DWORD PTR SS:[ESP+8],4
        //004015A3  |>  C9 LEAVE
        //004015A4  \.  C3 RETN

        //0040150E  |.  C74424 0C 000>MOV DWORD PTR SS:[ESP+C],0   int a = 0;
        //00401516  |.  C74424 08 000>MOV DWORD PTR SS:[ESP+8],0   int b = 0;
        //0040151E  |.  837C24 0C 01  CMP DWORD PTR SS:[ESP+C],1   if (a == 1
        //00401523  |.  75 0A JNZ SHORT a.0040152F         goto ax)
        //00401525  |.  C74424 08 010>MOV DWORD PTR SS:[ESP+8],1   b = 1;
        //0040152D  |.  EB 2A JMP SHORT a.00401559         goto end
        //0040152F  |>  837C24 0C 02  CMP DWORD PTR SS:[ESP+C],2   ax, (if a == 2
        //00401534  |.  75 0A JNZ SHORT a.00401540         goto bx)
        //00401536  |.  C74424 08 020>MOV DWORD PTR SS:[ESP+8],2   b = 2;
        //0040153E  |.  EB 19         JMP SHORT a.00401559         goto end
        //00401540  |>  837C24 0C 00  CMP DWORD PTR SS:[ESP+C],0   bx, (if a == 0
        //00401545  |.  75 0A JNZ SHORT a.00401551         goto cx)
        //00401547  |.  C74424 08 030>MOV DWORD PTR SS:[ESP+8],3   b = 3;
        //0040154F  |.  EB 08         JMP SHORT a.00401559         goto end
        //00401551  |>  C74424 08 040>MOV DWORD PTR SS:[ESP+8],4   cx, b = 4;
        //00401559  |>  C74424 0C 000>MOV DWORD PTR SS:[ESP+C],0   end, a = 0;
        //00401561  |.  C74424 08 000>MOV DWORD PTR SS:[ESP+8],0   b = 0;
        //00401569  |.  8B4424 0C MOV EAX,DWORD PTR SS:[ESP+C]
        //0040156D  |.  83F8 01       CMP EAX,1
        //00401570  |.  74 0B         JE SHORT a.0040157D
        //00401572  |.  83F8 02       CMP EAX,2
        //00401575  |.  74 10         JE SHORT a.00401587
        //00401577  |.  85C0 TEST EAX,EAX
        //00401579  |.  74 16         JE SHORT a.00401591
        //0040157B  |.  EB 1E         JMP SHORT a.0040159B
        //0040157D  |>  C74424 08 010>MOV DWORD PTR SS:[ESP+8],1
        //00401585  |.  EB 1C JMP SHORT a.004015A3
        //00401587  |>  C74424 08 020>MOV DWORD PTR SS:[ESP+8],2
        //0040158F  |.  EB 12         JMP SHORT a.004015A3
        //00401591  |>  C74424 08 030>MOV DWORD PTR SS:[ESP+8],3
        //00401599  |.  EB 08         JMP SHORT a.004015A3
        //0040159B  |>  C74424 08 040>MOV DWORD PTR SS:[ESP+8],4
        //004015A3  |>  C9 LEAVE
        //004015A4  \.  C3 RETN

        /// <summary>
        /// 게임오브젝트를 씬이 변경될때 파괴되지않도록 하는 함수
        /// </summary>
        /// <param name="_targetGameObject">파괴시키지않을 게임오브젝트</param>
        public static void DontDestroyTargetObject(GameObject _targetGameObject)
        {
            Object.DontDestroyOnLoad(_targetGameObject);
        }

        /// <summary>
        /// 타겟이 될 트랜스폼을 최상단부모로 옮기
        /// </summary>
        /// <param name="target">트랜스폼</param>
        public static void DeActiveSetNullParent(Transform target)
        {
            if (target.childCount == 0)
            {
                target.SetParent(null);
                target.gameObject.SetActive(false);
            }
            else
            {
                Transform[] transformArray = target.GetComponentsInChildren<Transform>();
                for (int i = 0; i < transformArray.Length; i++)
                {
                    transformArray[i].SetParent(null);
                    transformArray[i].gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 특정부모에 타겟팅한 오브젝트를 자식으로 집어넣고 부모위치와 회전값을 집어넣음
        /// </summary>
        /// <param name="parent">집어넣을 부모</param>
        /// <param name="target">타겟팅될 객체NotNull</param>
        public static void SetParentToTarget(Transform parent, Transform target)
        {
            target.SetParent(parent);
            if (!ReferenceEquals(parent, null))
            {
                target.SetPositionAndRotation(parent.position, Quaternion.identity);
            }
        }

        /// <summary>
        /// 거리를 체크해주는 함수.유니티거리체크보다 빠름//차이는 솔직히 매우 미세함
        /// </summary>
        /// <param name="a">a위치좌표</param>
        /// <param name="b">b위치좌표</param>
        /// <param name="distance">거리</param>
        /// <returns>해당거리보다 가까운지 여부</returns>
        public static bool CheckDistance(Vector3 a, Vector3 b, float distance)
        {
            //sqrMagnitude : sqrt 함수보다 빠름
            //sqrt : 크기에 제곱근을해서 반환

            //bool check = (a - b).sqrMagnitude < distance * distance + 0.001f;//느림
            bool check = GetSqrMagnitude(a, b) < distance * distance + 0.001f;//빠름
            return check;
        }

        /// <summary>
        /// 거리를 체크해주는 함수.유니티거리체크보다 빠름//차이는 솔직히 매우 미세함
        /// </summary>
        /// <param name="a">a위치좌표</param>
        /// <param name="b">b위치좌표</param>
        /// <param name="distance">거리</param>
        /// <returns>해당거리보다 가까운지 여부</returns>
        public static bool CheckDistance(Vector2 a, Vector2 b, float distance)
        {
            //sqrMagnitude : sqrt 함수보다 빠름
            //sqrt : 크기에 제곱근을해서 반환

            //bool check = (a - b).sqrMagnitude < distance * distance + 0.001f;//느림
            bool check = GetSqrMagnitude(a, b) < distance * distance + 0.001f;//빠름
            return check;
        }

        /// <summary>
        /// 거리에 대한 크기를 가져오는 함수//비교할경우 거리의 제곱을 비교할것
        /// </summary>
        /// <param name="a">a위치좌표</param>
        /// <param name="b">b위치좌표</param>
        /// <returns>거리</returns>
        public static float GetDistance(Vector3 a, Vector3 b)
        {
            //느림
            //var value = a - b;
            //return (float)System.Math.Sqrt(value.x * value.x + value.y * value.y + value.z * value.z);
            //빠름
            return (float)System.Math.Sqrt(GetSqrMagnitude(a, b));
        }

        /// <summary>
        /// 거리에 대한 크기를 가져오는 함수//비교할경우 거리의 제곱을 비교할것
        /// </summary>
        /// <param name="a">a위치좌표</param>
        /// <param name="b">b위치좌표</param>
        /// <returns>거리</returns>
        public static float GetDistance(Vector2 a, Vector2 b)
        {
            //var value = a - b;
            //return (float)System.Math.Sqrt(value.x * value.x + value.y * value.y);
            return (float)System.Math.Sqrt(GetSqrMagnitude(a, b));
        }


        /// <summary>
        /// 벡터의 제곱 (크기)길이를 반환
        /// </summary>
        /// <param name="a">a위치좌표</param>
        /// <param name="b">b위치좌표</param>
        /// <returns>크기(길이)</returns>
        public static float GetSqrMagnitude(Vector3 a, Vector3 b)
        {
            float xDiff = a.x - b.x;
            float yDiff = a.y - b.y;
            float zDiff = a.z - b.z;
            return xDiff * xDiff + yDiff * yDiff + zDiff * zDiff;
        }

        /// <summary>
        /// 벡터의 제곱 (크기)길이를 반환
        /// </summary>
        /// <param name="a">a위치좌표</param>
        /// <param name="b">b위치좌표</param>
        /// <returns>크기(길이)</returns>
        public static float GetSqrMagnitude(Vector2 a, Vector2 b)
        {
            float xDiff = a.x - b.x;
            float yDiff = a.y - b.y;
            return xDiff * xDiff + yDiff * yDiff;
        }

        /// <summary>
        /// 스케일팩터값을 가져오는 함수 
        /// </summary>
        /// /// <param name="currentValue">현재 값</param>
        /// <param name="standardValue">기준이 될 값</param>
        /// <returns>스케일팩터값</returns>
        public static float GetScaleFactor(this float currentValue, float standardValue)
        {
            float scaleFactor = currentValue / standardValue;
            return scaleFactor;
        }

        /// <summary>
        /// 투사체가 충돌체와 충돌시 특정각도내에 있을시 투사체의 발사각을 바꾸는 함수
        /// </summary>
        /// <param name="collision">충돌한 충돌체</param>
        /// <param name="projectileObject">투사체 오브젝트</param>
        /// <param name="reflectAngle">최대 도탄각도</param>
        /// <returns>도탄됫는지 여부</returns>
        public static bool ActionProjectileReflect(Collision2D collision, Transform projectileObject, float reflectAngle)
        {   
            //반사위치체크
            //결과값=Vector2.Reflect(입사각의 위치(충돌각도), 노말값의 위치(충돌시 타겟의 각도)
            //노말값은 충돌했을시 각도를 말하는것
            Vector2 direction = Vector2.Reflect(projectileObject.transform.up, collision.contacts[0].normal);



            //테스트하기
            //Vector2 direction = Vector2.Reflect(collision.contacts[0].point, collision.contacts[0].normal);
            //Gizmos.DrawLine(hitPos, normal * size);//동일1
            //Gizmos.DrawLine(hitPos, Vector2.Reflect(transform.up, normal));//동일2


            //반사각도 확인
            float InAngle = Vector2.Angle(projectileObject.transform.up, direction);
            //Debug.Log($"{collision.collider.name} 충돌, 입사각 : {InAngle}, 입사각 위치 : { (Vector2)refectionAttackBox.transform.up} , 반사 결과값 : {direction } ");

            //왠만해선60~70각도이하부터가 볼만함//55도가 적당한것같기도하구
            if (InAngle > reflectAngle)
            {
                return false;
            }

            projectileObject.transform.up = direction;//방향동기화
            return true;
        }

      

        //사용안함
        public static Vector2 GetWorldPosToUIPos(Vector2 worldPos, Canvas canvas, Camera camera)
        {
            //미완
            RectTransform CanvasRect = canvas.GetComponent<RectTransform>();
            Vector2 ViewportPosition = camera.WorldToViewportPoint(worldPos);
            Vector2 WorldObject_ScreenPosition = new Vector2(
            ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
            ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

            //Rect의 앵커포지션으로 반환
            return WorldObject_ScreenPosition;
        } 

        /// <summary>
        /// 지정한 좌표간의 센터위치 구하는 함수
        /// </summary>
        /// <param name="_points">좌표 값들</param>
        /// <returns>중앙위치</returns>
        public static Vector3 Centroid(Vector3[] _points)
        {
            Vector3 center = Vector3.zero;
            for (int i = 0; i < _points.Length; i++)
            {
                center += _points[i];
            }
            center /= _points.Length;
            return center;
        }

        /// <summary>
        /// 지정한 좌표간의 센터위치 구하는 함수
        /// </summary>
        /// <param name="_points">좌표 값들</param>
        /// <returns>중앙위치</returns>
        public static Vector3 Centroid(Vector3Int[] _points)
        {
            Vector3 center = Vector2.zero;
            for (int i = 0; i < _points.Length; i++)
            {
                center += _points[i];
            }
            center /= _points.Length;
            return center;
        }

        /// <summary>
        /// 특정값의 퍼센트 값만큼 가져오는 함수
        /// </summary>
        /// <param name="value">특정값</param>
        /// <param name="percent">퍼센트</param>
        /// <returns>특정값의 몇퍼센트값</returns>
        public static float GetPercentForValue(float value, float percent)
        {
            return value * (percent * 0.01f);
        }

        /// <summary>
        /// enum형식에 정의된 값들을 가져오는 함수(캐싱권장)
        /// </summary>
        /// <typeparam name="T">enum형식</typeparam>
        /// <returns>enum에 정의된 값리스트</returns>
        public static List<T> GetEnumDefineData<T>() where T : struct
        {
            List<T> list = new List<T>();
            var tempArray = System.Enum.GetValues(typeof(T));
            foreach (var item in tempArray)
            {
                list.Add((T)item);
            }

            //var tempArray = System.Enum.GetNames(typeof(T));
            //foreach (var item in tempArray)
            //{
            //    if (System.Enum.TryParse(typeof(T).Name, out T result))
            //    {
            //        list.Add(result);
            //    }
            //}
            return list;
        }

        /// <summary>
        /// enum형식에 정의된 값들의 이름을 가져오는 함수
        /// </summary>
        /// <typeparam name="T">enum형식</typeparam>
        /// <returns>enum에 정의된 이름리스트</returns>
        public static List<string> GetEnumDefineStringData<T>() where T : struct
        {
            var tempArray = System.Enum.GetNames(typeof(T));
            List<string> list = new List<string>(tempArray);
            return list;
        }

        /// <summary>
        /// enum형식에 있는 최대크기를 가져오는 사람
        /// </summary>
        /// <typeparam name="T">enum형식</typeparam>
        /// <returns>enum의 최대크기</returns>
        public static int GetEnumDefineLength<T>() where T : struct
        {
            return System.Enum.GetNames(typeof(T)).Length;
        }
 
        /// <summary>
        /// 리스트로 되있는걸 하나의 로그로 띄어주는 함수(받을수도 있음)
        /// </summary>
        /// <typeparam name="T">리스트타입</typeparam>
        /// <param name="target">리스트</param>
        /// <returns>합친리스트</returns>
        public static string LogIList<T>(T target) where T : IList
        {
            string content = "";
            foreach (var item in target)
            {
                content = GetCombineString(content, $"{item}\n");
            }
            Debug.Log(content);
            return content;
        }

        //======================================================================
        //랜덤부분---------------------------------------------------------------
        //======================================================================

        /// <summary>
        /// 확률 계산
        /// </summary>
        /// <param name="probabilityNum">집어넣을 확률</param>
        /// <returns>확률에 들어갔는가 여부</returns>
        public static bool ProbabilityCal(int probabilityNum)
        {   
            int num = Random.Range(0, 101);
            return probabilityNum >= num ? true : false;
        }

        /// <summary>
        /// 랜덤한 원형위치를 가져오는 함수
        /// </summary>
        /// <param name="targetPos">타겟 위치</param>
        /// <param name="size">크기</param>
        /// <param name="isCheck">랜덤여부</param>
        /// <returns>랜덤위치</returns>
        public static Vector2 GetRandomCirclePosition(Transform targetPos, float size, bool isNormalizeRandom = false, bool isCheck = true)
        {
            return GetRandomCirclePosition(targetPos.position, size, isNormalizeRandom, isCheck);
        }

        /// <summary>
        /// 랜덤한 원형위치를 가져오는 함수
        /// </summary>
        /// <param name="targetPos">타겟 위치</param>
        /// <returns>랜덤위치</returns>
        public static Vector2 GetRandomCirclePosition(Vector2 targetPos, float size, bool isNormalizeRandom = false, bool isCheck = true)
        {
            Vector2 randomPos = isCheck ? targetPos + ((isNormalizeRandom ? Random.insideUnitCircle.normalized : Random.insideUnitCircle) * size) : targetPos;
            return randomPos;
        }

        //======================================================================
        //회전부분---------------------------------------------------------------
        //======================================================================
        /// <summary>
        /// A->B방향으로 회전값 가져오는 함수
        /// </summary>
        /// <param name="rotateTarget">회전하는 오브젝트</param>
        /// <param name="lookTarget">봐야될 위치</param>
        /// <param name="offSetAngle">필요시 따로 설정하여 각도변환하기</param>
        /// <returns>회전값</returns>
        public static Quaternion GetRotation(Vector3 rotateTarget, Vector3 lookTarget, float offSetAngle = 90)
        {   
            return GetRotation(rotateTarget, lookTarget, Vector3.forward, offSetAngle);
        }

        /// <summary>
        /// A->B방향으로 회전값 가져오는 함수//테스트후 수정
        /// </summary>
        /// <param name="rotateTarget">회전하는 오브젝트</param>
        /// <param name="lookTarget">봐야될 위치</param>
        /// <param name="offSetAngle">필요시 따로 설정하여 각도변환하기</param>
        /// <returns>회전값</returns>
        public static Quaternion GetRotation(Vector3 rotateTarget, Vector3 lookTarget, Vector3 axis, float offSetAngle = 90)
        {
            Vector2 targetDir = lookTarget - rotateTarget;
            float newangle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - offSetAngle;
            //Debug.Log("Angle :" + newangle + "angle2" + Vector2.Angle(rotateTarget, lookTarget));
            Quaternion quaternion = Quaternion.AngleAxis(newangle, axis);
            return quaternion;
        }

        /// <summary>
        /// A->B방향으로 제한된 회전값 가져오는 함수//테스트후 수정
        /// </summary>
        /// <param name="rotateTarget">회전하는 오브젝트</param>
        /// <param name="lookTarget">봐야될 위치</param>
        /// <param name="limitAngle">제한값</param>
        /// <param name="offSetAngle">필요시 따로 설정하여 각도변환하기</param>
        /// <returns>회전값</returns>
        public static Quaternion GetRotation(Transform rotateTarget, Transform lookTarget, float limitAngle, float offSetAngle = 90)
        {
            //각도 제한 테스트해야됨
            //봐야될 위치 기준으로 각도를 일정정해주고 회저

            //회전각도체크할 오브젝트 => lookTarget
            //회전시킬오브젝트 => rotateTarget;
            //볼대상의 각도에서 체크            

            Vector3 euler = lookTarget.rotation.eulerAngles;
            euler.z = euler.z > 180 ? euler.z - 360 : euler.z;

            Vector3 rotateEuler = rotateTarget.rotation.eulerAngles;
            rotateEuler.z = rotateEuler.z > 180 ? rotateEuler.z - 360 : rotateEuler.z;

            rotateEuler.z = Mathf.Clamp(rotateEuler.z, euler.z - limitAngle, euler.z + limitAngle);



            Debug.Log($"봐야되는 오브젝트각도 : {euler.z} 회전오브젝트각도  {rotateEuler.z}");
            //if (newangle > limitAngle)
            //{
            //    return targetObject.rotation;
            //}


            Vector2 targetDir = lookTarget.position - rotateTarget.position;
            float newangle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - offSetAngle;
            //Debug.Log("Angle :" + newangle + "angle2" + Vector2.Angle(rotateTarget, lookTarget));
            Quaternion quaternion = Quaternion.AngleAxis(newangle, Vector3.forward);
            quaternion *= Quaternion.Euler(rotateEuler);
            return quaternion;
        }

        /// <summary>
        /// Slerp로 회전하는 함수
        /// </summary>
        /// <param name="rotateTarget">회전하는 오브젝트</param>
        /// <param name="lookTarget">봐야될 위치</param>
        /// <param name="rotateSpeed">회전속도</param>
        public static void SlerpRotationZ(Transform rotateTarget, Vector3 lookTarget, float rotateSpeed)
        {
            Vector2 targetDir = lookTarget - rotateTarget.position;
            //float newangle = Mathf.Atan2(targetDir.a, targetDir.b) * Mathf.Rad2Deg;
            //if (newangle > 0 && newangle < 180)//정상작동
            //{
            //    newangle = newangle * -1;
            //}
            //else
            //{
            //    newangle = newangle * -1;
            //}
            float newangle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
            Quaternion quaternion = Quaternion.AngleAxis(newangle, Vector3.forward);
            //Quaternion quaternion = Quaternion.AngleAxis(newangle - 90, Vector3.forward);
            rotateTarget.rotation = Quaternion.Slerp(rotateTarget.rotation, quaternion, rotateSpeed * Time.deltaTime);//slerp     
        }

        //20190922//신규 추가 회전
        public static void RotationZTarget(Transform rotateObject, Transform lookTarget, float rotateSpeed)
        {   
            //Quaternion lookRotation = Quaternion.LookRotation(rotateTarget);
            //Debug.Log(lookRotation);
            //Vector3 tmpEuler = Quaternion.RotateTowards(rotateObject.rotation, lookRotation,speed * Time.deltaTime).eulerAngles;
            //Debug.Log(tmpEuler);
            Vector2 targetDir = lookTarget.position - rotateObject.position;
            float mouseAngle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
            float turretAngle = Quaternion.Angle(rotateObject.rotation, lookTarget.rotation);
            //if (mouseAngle > 0 && mouseAngle < 180)//정상작동
            //{
            //    mouseAngle = mouseAngle * -1;
            //}
            //else
            //{
            //    mouseAngle = mouseAngle * -1;
            //}
            //if (mouseAngle < 0)
            //{
            //    turretAngle *= -1;
            //}

            //float newangle3 = Quaternion.Dot(rotateObject.rotation, rotateTarget.rotation);
            //Debug.Log(newangle + "///" + newangle2 + "///" + newangle3);

            //테스트중
            //3가지를 고려해서 짜야함
            //마우스 각도  터렛각도 현재 각도와제일 가까운 값을향한 변수

            Debug.Log(mouseAngle + "///" + turretAngle);
            if ((int)turretAngle == (int)mouseAngle)
            {
                return;
            }
            float firResult = mouseAngle - turretAngle;
            float secResult = turretAngle - mouseAngle;

            if (firResult > secResult)
            {   
                rotateObject.Rotate(0,0,rotateSpeed * Time.deltaTime);
            }
            else
            {
                rotateObject.Rotate(0, 0, -rotateSpeed * Time.deltaTime);
            }

            //아직봉인




            //rotateObject.rotation = Quaternion.Euler(0, 0, rotateObject.rotation.z + -speed * Time.deltaTime);
            //Quaternion quaternion = Quaternion.AngleAxis(newangle, Vector3.forward);
            //rotateObject.rotation = Quaternion.Slerp(rotateObject.rotation, quaternion, speed * Time.deltaTime);//slerp
        }

        //20220914새로처리한 회전
        //slerp < Lerp가 더빠름//Slerp는 호를 일정하게//lerp는 선을 일정하게


        

        /// <summary>
        /// Slerp로 회전//구면 선형보간//호에서 일정하게 증가
        /// </summary>
        /// <param name="rotateTarget">회전하는 트랜스폼</param>
        /// <param name="lookTarget">봐야할 타겟</param>
        /// <param name="rotateSpeed">회전 속도</param>
        public static void RotateSlerp2D(Transform rotateTarget, Vector2 lookTarget, float rotateSpeed)
        {
            //Slerp회전
            Vector2 targetDir = lookTarget - (Vector2)rotateTarget.position;
            float newangle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - 90;
            Quaternion quaternion = Quaternion.AngleAxis(newangle, Vector3.forward);
            rotateTarget.rotation = Quaternion.Slerp(rotateTarget.rotation, quaternion, rotateSpeed);//slerp     
        }

        /// <summary>
        /// Lerp로 회전//선형보간//선에서 일정하게 증가
        /// </summary>
        /// <param name="rotateTarget">회전하는 트랜스폼</param>
        /// <param name="lookTarget">봐야할 타겟</param>
        /// <param name="rotateSpeed">회전 속도</param>
        public static void RotateLerp2D(Transform rotateTarget, Vector2 lookTarget, float rotateSpeed, float offSetAngle = 90)
        {
            //Lerp회전
            Vector2 targetDir = lookTarget - (Vector2)rotateTarget.position;
            float newangle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - offSetAngle;
            Quaternion quaternion = Quaternion.AngleAxis(newangle, Vector3.forward);
            rotateTarget.rotation = Quaternion.Lerp(rotateTarget.rotation, quaternion, rotateSpeed);
        }

        /// <summary>
        /// 일정한 속도로 회전
        /// </summary>
        /// <param name="rotateTarget">회전하는 트랜스폼</param>
        /// <param name="lookTarget">봐야할 타겟</param>
        /// <param name="rotateSpeed">회전 속도</param>
        public static void RotateTurret2D(Transform rotateTarget, Vector2 lookTarget, float rotateSpeed, float offSetAngle = 90)
        {
            Vector2 targetDir = lookTarget - (Vector2)rotateTarget.position;
            float newangle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - offSetAngle;
            var angleZ = rotateTarget.eulerAngles.z;
            newangle = MoveTowardsAngle(angleZ, newangle, rotateSpeed);//일정하게 움직임
            rotateTarget.rotation = Quaternion.Euler(0, 0, newangle);
        }

        /// <summary>
        /// 일정한속도로 회전(내적)
        /// </summary>
        /// <param name="rotateTarget">회전할 타겟</param>        
        /// <param name="target">회전할 위치</param>
        /// /// <param name="rotateSpeed">회전속도</param>
        public static void RotateDot(Transform rotateTarget, Vector3 target, float rotateSpeed)
        {
            //방향구하기
            Vector3 dir = (target - rotateTarget.position).normalized;

            // 내적(dot)을 통해 각도를 구함. (Acos로 나온 각도는 방향을 알 수가 없음)
            Vector3 trUp = rotateTarget.up;

            //방향과 방향을 체크하여 정면인지 아닌지 체크
            float dot = Vector3.Dot(trUp, dir);//벡터의 내적은 스칼라(스케일의 어원)으로 나옴

            //if (dot < 1.0f)//1이 정면임
            if (dot < 0.9999f)//1이 정면임
            {
                //Debug.Log($"{dot}");
                float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

                // 외적을 통해 각도의 방향을 판별.
                Vector3 cross = Vector3.Cross(trUp, dir);
                float tempSpeed = rotateSpeed * Time.deltaTime;

                //Debug.Log(cross.z);


                // 외적 결과 값에 따라 각도 반영
                if (cross.z < 0)
                {
                    angle = rotateTarget.rotation.eulerAngles.z - Mathf.Min(10, angle) * tempSpeed;
                }
                else
                {
                    angle = rotateTarget.rotation.eulerAngles.z + Mathf.Min(10, angle) * tempSpeed;
                }

                rotateTarget.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }

        /// <summary>
        /// 앵글용 클램프//Mathf.Clamp는 각도로 괜찮지가 않음
        /// </summary>
        /// <param name="curAngle">현재 각도</param>
        /// <param name="min">최소각도</param>
        /// <param name="max">최대각도</param>
        /// <returns>반환각도</returns>
        public static float ClampAngle(float curAngle, float min, float max)
        {
            //0~360도 제한이 있음
            //- 각도로 들어갈시 360이 아닌 1로 돌아감
            //FloorToInt 소수점내림
            //-360 ~ +360
            //사이에서 처리해야됨
            float startAngle = (min + max) * 0.5f - PI;
            float floor = Mathf.FloorToInt((curAngle - startAngle) / (PI * 2)) * (PI * 2);
            floor = Mathf.Clamp(curAngle, min + floor, max + floor);
            return floor;
        }

        /// <summary>
        /// 앵글용 클램프//Mathf.Clamp는 각도로 괜찮지가 않음
        /// </summary>
        /// <param name="curEulerAngles">현재 룰러각도</param>
        /// <param name="min">최소 룰러각도</param>
        /// <param name="max">최대 룰러각도</param>
        /// <returns>반환롤러각도</returns>
        public static Vector3 ClampAngle(Vector3 curEulerAngles, Vector3 min, Vector3 max)
        {
            float x = ClampAngle(curEulerAngles.x, min.x, max.x);
            float y = ClampAngle(curEulerAngles.y, min.y, max.y);
            float z = ClampAngle(curEulerAngles.z, min.z, max.z);
            return new Vector3(x, y, z);
        }

        private const float PI = 180;
        private const float addAngle = 90f;

        /// <summary>
        /// 일정한 속도로 제한된 회전
        /// </summary>
        /// <param name="rotateTarget">회전하는 트랜스폼</param>
        /// <param name="lookTarget">봐야할 타겟</param>
        /// <param name="rotateSpeed">회전 속도</param>
        /// <param name="min">최소각도-180 ~ 0</param>
        /// <param name="max">최대각도 0 ~ 180</param>
        public static void RotateLimit(Transform rotateTarget, Vector3 lookTarget, float rotateSpeed, float min, float max, AxisDirectionType axisDirectionType)
        {
            //회전자체는 잘되지만 -180~180 넘어가는 구간에서 회전이 맘에 안듬
            //몇몇구간이 반대로 돌아감=> 시야처리를 통해 작업

            Vector2 targetDir = lookTarget - rotateTarget.position;
            float newangle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;//0~360

            float zAngle = 0;
            switch (axisDirectionType)
            {
                case AxisDirectionType.X:
                    zAngle = Mathf.Clamp(newangle, min, max);//x축 기준일시
                    break;
                case AxisDirectionType.Y:
                    zAngle = Mathf.Clamp(newangle, min + addAngle, max + addAngle);//Y축 기준일시
                    break;
            }

            //Debug.Log($"Target : {(int)newangle}, Result : {(int)zAngle}");
            //20240818내부쪽 바깥으로 빼버리기
            zAngle = MoveTowardsAngle(rotateTarget.eulerAngles.z, zAngle - 90, rotateSpeed);
            rotateTarget.localRotation = Quaternion.Euler(0, 0, zAngle);
        }

        //
        // 요약:
        //     Moves a value current towards target.
        //
        // 매개 변수:
        //   current:
        //     The current value.
        //
        //   target:
        //     The value to move towards.
        //
        //   maxDelta:
        //     The maximum change that should be applied to the value.
        public static float MoveTowardsAngle(float current, float target, float maxDelta)
        {
            float num = DeltaAngle(current, target);
            if (0f - maxDelta < num && num < maxDelta)
            {
                return target;
            }

            target = current + num;
            return MoveTowards(current, target, maxDelta);
        }

        //
        // 요약:
        //     Moves a value current towards target.
        //
        // 매개 변수:
        //   current:
        //     The current value.
        //
        //   target:
        //     The value to move towards.
        //
        //   maxDelta:
        //     The maximum change that should be applied to the value.
        public static float MoveTowards(float current, float target, float maxDelta)
        {
            if (System.Math.Abs(target - current) <= maxDelta)
            {
                return target;
            }

            return current + Sign(target - current) * maxDelta;
        }
        public static float Sign(float f)
        {
            return (f >= 0f) ? 1f : (-1f);
        }
        //
        // 요약:
        //     Calculates the shortest difference between two given angles given in degrees.
        //
        //
        // 매개 변수:
        //   current:
        //
        //   target:
        public static float DeltaAngle(float current, float target)
        {
            float num = Repeat(target - current, 360f);
            if (num > 180f)
            {
                num -= 360f;
            }

            return num;
        }
        //
        // 요약:
        //     Loops the value t, so that it is never larger than length and never smaller than
        //     0.
        //
        // 매개 변수:
        //   t:
        //
        //   length:
        public static float Repeat(float t, float length)
        {
            return Clamp((float)(t - System.Math.Floor(t / length) * length), 0f, length);
        }
        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
            {
                value = min;
            }
            else if (value > max)
            {
                value = max;
            }

            return value;
        }

        /// <summary>
        /// 벡터회전용//테스트해야됨 
        /// </summary>
        /// <param name="angle">각도</param>
        /// <param name="vector">벡터</param>
        /// <returns></returns>
        public static Vector2 Rotate2D(float angle, Vector2 vector)
        {
            float x, y;
            float sinValue = Mathf.Sin(Mathf.Deg2Rad * -angle);
            float cosValue = Mathf.Cos(Mathf.Deg2Rad * -angle);

            x = cosValue + vector.y * sinValue;
            y = sinValue + vector.y * cosValue;

            return new Vector2(x, y);
        }

        /// <summary>
        /// 우주차량유닛의 회전오브젝트들을 회전하게 만드는 함수
        /// </summary>
        /// <param name="rotateTurretObject">회전할 대상들</param>
        /// <param name="targetPos">타겟위치</param>
        public static void RotateTurret(RotateTurretObject rotateTurretObject, Vector2 targetPos)
        {
            float time = rotateTurretObject.rotateSpeed * Time.deltaTime;
            Transform tempTr = rotateTurretObject.targetObject;
            switch (rotateTurretObject.rotateType)
            {
                case RotateType.Slerp:
                    RotateSlerp2D(tempTr, targetPos, time);
                    break;
                case RotateType.Lerp:
                    RotateLerp2D(tempTr, targetPos, time);
                    break;
                case RotateType.Turret:
                    RotateTurret2D(tempTr, targetPos, time);
                    break;
            }
        }

        /// <summary>
        /// 우주차량유닛의 회전오브젝트들을 회전하게 만드는 함수
        /// </summary>
        /// <param name="rotateTurretObjectArray">회전할 대상들</param>
        /// <param name="targetPos">타겟위치</param>
        public static void RotateTurretArray(RotateTurretObject[] rotateTurretObjectArray, Vector2 targetPos)
        {
            for (int i = 0; i < rotateTurretObjectArray.Length; i++)
            {
                RotateTurretObject temp = rotateTurretObjectArray[i];
                RotateTurret(temp, targetPos);
            }
        }

        public static void RotateLeadCollision(Transform rotateTarget, Transform target, float rotateSpeed, float leadDistance)
        {
            //라티오는 거리를 말함//0되면 타격이 되야됨
            float ratio = Vector2.Distance(rotateTarget.position, target.position) - leadDistance;
            ratio = Mathf.Max(0, ratio);

            //미사일관련해서는 상대방 방향쪽만 확인해도 충분해보인다           
            //Vector3 missileDir = attackBox.transform.position;// + attackBox.transform.up * curRatio;//미사일 위치
            Vector3 targetDir = target.position + target.up * ratio;//타겟의 다음방향과 현재거리체크
            RotateDot(rotateTarget.transform, targetDir, rotateSpeed);
        }

        public static Vector2 GetLeadCollision(Transform rotateTarget, Transform target, float leadDistance)
        {
            //라티오는 거리를 말함//0되면 타격이 되야됨
            float ratio = GetDistance(rotateTarget.position, target.position) - leadDistance;
            ratio = Mathf.Max(0, ratio);

            //미사일관련해서는 상대방 방향쪽만 확인해도 충분해보인다           
            //Vector3 missileDir = attackBox.transform.position;// + attackBox.transform.up * curRatio;//미사일 위치
            Vector3 targetDir = target.position + target.up * ratio;//타겟의 다음방향과 현재거리체크
            return targetDir;
        }

      

        public static void RotateRigidBody2D(Rigidbody2D rb2d, Transform rotateTarget, Transform lookTarget)
        {
            Vector3 direction = lookTarget.position - rotateTarget.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            //rb2d.rotation = angle;
            rb2d.SetRotation(angle);
        }
        public static void RotateRigidBody2D(Rigidbody2D rb2d, Vector2 lookTarget, float rotateSpeed, float offSetAngle = 90)
        {
            Vector2 targetDir = lookTarget - (Vector2)rb2d.position;
            float newangle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - offSetAngle;
            var angleZ = rb2d.rotation;
            newangle = MoveTowardsAngle(angleZ, newangle, rotateSpeed);//일정하게 움직임
            rb2d.rotation = newangle;
        }

        //아직 검증안됨//속도에 다른 회전처리
        public static void RotationObject(Rigidbody2D rb2d, float offset = 90f)
        {
            float angle = Mathf.Atan2(rb2d.linearVelocity.y, rb2d.linearVelocity.x) * Mathf.Rad2Deg + offset;
            rb2d.SetRotation(angle);// = Quaternion.AngleAxis(angle, Vector3.forward);
        }


        private static float tau = Mathf.PI * 2;
        /// <summary>
        /// Sin웨이브를 주는 
        /// </summary>
        /// <param name="amplitude"></param>
        /// <param name="frequency"></param>
        /// <returns>값</returns>
        public static float SinWave(float amplitude, float frequency)
        {
            //Time.timeSinceLevelLoad
            return amplitude * Mathf.Sin(tau * Time.time * frequency);
        }

        public static float SinWave(float amplitude, float frequency, int index)
        {
            //Time.timeSinceLevelLoad
            return amplitude * Mathf.Sin(tau * Time.time * frequency + index);
        }
     
        /// <summary>
        /// 두개의 System.object가 동일한지 체크하는 함수
        /// </summary>
        /// <param name="target1">System.Object</param>
        /// <param name="target2">System.Object</param>
        public static void CheckBothTarget(object target1, object target2)
        {
            //동일한 객체이면 트루로 반환함
            Debug.Log(ReferenceEquals(target1, target2));
        }
     

        /// <summary>
        /// 궤적포인트(타겟오브젝트)를 가져오는 함수
        /// </summary>
        /// <param name="targetObject">타겟오브젝트</param>
        /// <param name="direction">방향</param>
        /// <param name="power">파워</param>
        /// <param name="time">시간</param>
        /// <returns>궤적 포인트</returns>
        public static Vector3 GetArcPoint(Transform targetObject, Vector2 direction, Vector3 gravity, float power, float time, bool worldSpace)
        {
            //transform.position += velocity * Time.deltaTime * Mathf.Lerp(tempTime, 1, acel.Evaluate(tempTime / 1));
            //tempTime += Time.deltaTime;
            //velocity.a += (gravity.a * mass) * Time.deltaTime;
            //velocity.b -= (gravity.b * mass) * Time.deltaTime;
            //공식최적화필요//최적인듯

            Quaternion targetRotation = worldSpace ? Quaternion.identity : targetObject.rotation;
            Vector3 velocity = targetRotation * direction;
            velocity = targetObject.position + (velocity.normalized * power * time) + 0.5f * gravity * (time * time);
            //Vector2 explorePoint = (Vector2)targetObject.position + (direction.normalized * power * time) + 0.5f * Physics2D.gravity * (time * time);
            return velocity;
        }

        /// <summary>
        /// 궤적포인트(벡터0기준)을 가져오는 함수
        /// </summary>
        /// <param name="direction">방향</param>
        /// <param name="power">파워</param>
        /// <param name="time">시간</param>
        /// <returns>궤적포인트</returns>
        public static Vector3 GetArcPoint(Vector3 direction, Vector3 gravity, float power, float time)
        {
            //transform.position += velocity * Time.deltaTime * Mathf.Lerp(tempTime, 1, acel.Evaluate(tempTime / 1));
            //tempTime += Time.deltaTime;
            //velocity.a += (gravity.a * mass) * Time   .deltaTime;
            //velocity.b -= (gravity.b * mass) * Time.deltaTime;
            //공식최적화필요//최적인듯
            //이거 저항도계산이 안들어가서 위치값이 좀 틀릴거
            Vector3 pos = (direction.normalized * power * time) + 0.5f * gravity * (time * time);
            return pos;
        }

        /// <summary>
        /// 궤적포인트(3포인트)를 가져오는 함수//쓸지는 모름
        /// </summary>
        /// <param name="targetObject">a</param>
        /// <param name="direction"></param>
        /// <param name="power"></param>
        /// <param name="time"></param>
        public static Vector3[] GetArc3Point(Transform targetObject, Vector3 direction, Vector3 gravity, float power, float time)
        {
            var velocity = (targetObject.rotation * (direction.normalized * power));//속도
            var p0 = targetObject.position;//첫번쨰위치
            var p1 = p0 + 0.5f * velocity * time;//중간위치
            var p2 = velocity * time + gravity * time * time * 0.5f;//마지막위치
            Vector3[] temp = { p0,p1,p2 };
            return temp;
        }

        /// <summary>
        /// B=>A 방향을 구함
        /// </summary>
        /// <param name="targetAPos">A 위치</param>
        /// <param name="targetBPos">B 위치</param>
        /// <returns>단위벡터</returns>
        public static Vector2 CalDirection2D(Vector2 targetAPos, Vector2 targetBPos)
        {
            Vector3 direction = targetAPos - targetBPos;
            //float newAngle = Mathf.Atan2(direction.b, direction.a) * Mathf.Rad2Deg;
            //Debug.Log(newAngle);

            ////사분명처리를 위해 라디안사용해야됨
            ////c= 3.14*r
            //float radian = newangle * Mathf.PI / 180; //라디안값//동일값
            //float radian = newAngle * Mathf.Deg2Rad; //라디안값//동일값

            float radian = Mathf.Atan2(direction.y, direction.x);

            //수평속도 = 투사속도 * cos(각도)
            //수직속도 = 투사속도 * sin(각도)
            direction = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
            //Vector2 unitDir;
            //unitDir.x = Mathf.Cos(radian);
            //unitDir.y = Mathf.Sin(radian);

            //수평일시 속도:(-1.0, 0.0)각도:180라디안:3.141593
            //각을 주었을시 속도:(-0.9, -0.4)각도:-153.4669라디안:-2.678504
            //Debug.Log("속도:" + unitDir + "각도:" + newAngle + "라디안:" + radian);
            return direction;
        }

        /// <summary>
        /// 최대최소 정규화 함수
        /// </summary>
        /// <param name="min">최소값</param>
        /// <param name="max"><최대값/param>
        /// <param name="value">현재값</param>
        /// <returns>0~1사이의 값</returns>
        public static float MinMaxNormalize(float min, float max, float value)
        {
            return (value - min) / (max - min);
        }        

        /// <summary>
        /// Z점수 정규화 함수
        /// </summary>
        /// <param name="mean"></param>
        /// <param name="stdDev"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float ZScoreNormalize(float mean, float stdDev, float value)
        {
            return (value - mean) / stdDev;
        }

        /// <summary>
        /// 정규화 복구 함수
        /// </summary>
        /// <param name="min">최소</param>
        /// <param name="max">최대</param>
        /// <param name="normalizedValue">노말라이즈값</param>
        /// <returns></returns>
        public static float RestoreNormalize(float min, float max, float normalizedValue)
        {
            return Mathf.Lerp(min, max, normalizedValue);
        }

        /// <summary>
        /// 리니어 기능
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float LinearFunc(float start, float end, float t)
        {
            return (1 - t) * start + t * end;
        }

        /// <summary>
        /// 리니어 기능
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector3 LinearFunc(Vector3 start, Vector3 end, float t)
        {
            return (1 - t) * start + t * end;
        }

        public static Vector3 TwoPointBezier(Vector3 p0, Vector3 p1, Vector3 p2 , float t)
        {
            float t2 = (1 - t) * (1 - t);
            return t2 * p0 + 2 * (1 - t) * t * p1 + t * t * p2;
        }

        /// <summary>
        /// 3개의 점으로 만드는 곡선(2차 베지어 곡선)
        /// </summary>
        /// <param name="aPoint">첫번째 위치</param>
        /// <param name="handle">핸들</param>
        /// <param name="bPoint">두번째 위치</param>
        /// <param name="time">0에서 1사이의 시간값</param>
        /// <returns>시간에 따른 곡선값</returns>
        public static float ThreePointBezier(float aPoint, float handle, float bPoint, float time)
        {
            //B(t) = P1 + (1 - t)² (P0 - P1) + t²(P2 - P1)
            //B(t) = 시간에 따른 최종값
            //P0 = aPoint
            //P1 = aPointHandle
            //P2 = bPoint
            //t = 시간(0 <= t <= 1)

            return handle + Mathf.Pow(1 - time, 2) * (aPoint - handle) + Mathf.Pow(time, 2) * (bPoint - handle);
        }

        /// <summary>
        /// 3개의 점으로 만드는 곡선(2차 베지어 곡선)
        /// </summary>
        /// <param name="aPoint">첫번째 위치</param>
        /// <param name="handle">핸들</param>
        /// <param name="bPoint">두번째 위치</param>
        /// <param name="time">0에서 1사이의 시간값</param>
        /// <returns>시간에 따른 곡선값</returns>
        public static Vector3 ThreePointBezier(Vector3 aPoint, Vector3 handle, Vector3 bPoint, float time)
        {
            //B(t) = P1 + (1 - t)² (P0 - P1) + t²(P2 - P1)
            //B(t) = 시간에 따른 최종값
            //P0 = aPoint
            //P1 = aPointHandle
            //P2 = bPoint
            //t = 시간(0 <= t <= 1)

            return handle + Mathf.Pow(1 - time, 2) * (aPoint - handle) + Mathf.Pow(time, 2) * (bPoint - handle);
        }

        /// <summary>
        /// 4개의 점으로 만드는 곡선(3차 베지어 곡선)
        /// </summary>
        /// <param name="aPoint">첫번째 위치</param>
        /// <param name="aPointHandle">첫번째 핸들</param>
        /// <param name="bPointHandle">두번째 핸들</param>
        /// <param name="bPoint">두번째 위치</param>
        /// <param name="time">0에서 1사이의 시간값</param>
        /// <returns>시간에 따른 곡선값</returns>
        public static float FourPointBezier(float aPoint, float aPointHandle, float bPointHandle, float bPoint, float time)
        {
            //B(t) = (1 - t)³ P0 + 3(1 - t)² t P1 + 3(1 - t)t² P2 + t³ P3
            //B(t) = 시간에 따른 최종값
            //P0 = aPoint
            //P1 = aPointHandle
            //P2 = bPointHandle
            //P3 = bPoint
            //t = 시간(0 <= t <= 1)

            return Mathf.Pow((1 - time), 3) * aPoint + Mathf.Pow((1 - time), 2) * 3 * time * aPointHandle
                    + Mathf.Pow(time, 2) * 3 * (1 - time) * bPointHandle + Mathf.Pow(time, 3) * bPoint;
        }

        /// <summary>
        /// 4개의 점으로 만드는 곡선(3차 베지어 곡선)
        /// </summary>
        /// <param name="aPoint">첫번째 위치</param>
        /// <param name="aPointHandle">첫번째 핸들</param>
        /// <param name="bPointHandle">두번째 핸들</param>
        /// <param name="bPoint">두번째 위치</param>
        /// <param name="time">0에서 1사이의 시간값</param>
        /// <returns>시간에 따른 곡선값</returns>
        public static Vector3 FourPointBezier(Vector3 aPoint, Vector3 aPointHandle, Vector3 bPointHandle, Vector3 bPoint, float time)
        {
            //B(t) = (1 - t)³ P0 + 3(1 - t)² t P1 + 3(1 - t)t² P2 + t³ P3
            //B(t) = 시간에 따른 최종값
            //P0 = aPoint
            //P1 = aPointHandle
            //P2 = bPointHandle
            //P3 = bPoint
            //t = 시간(0 <= t <= 1)

            return Mathf.Pow((1 - time), 3) * aPoint + Mathf.Pow((1 - time), 2) * 3 * time * aPointHandle
                    + Mathf.Pow(time, 2) * 3 * (1 - time) * bPointHandle + Mathf.Pow(time, 3) * bPoint;
        }


        private static void TestFourPointBezier(Vector3 aPoint, Vector3 aPointHandle, Vector3 bPointHandle, Vector3 bPoint, float time)
        {
            float t4 = Mathf.Pow(1 - time, 4);
            float t3 = Mathf.Pow(1 - time, 3);
            float t2 = Mathf.Pow(1 - time, 2);
            float t1 = 1 - time;


            
            //return t4 * aPoint+
            //    4 * t3
        }

        //각지점을 통과하는 스플라인
        //4개의 점이면 6개의 점이 필요함
        //잘하면 그거 만들수도
        //경로랑 같이 써야할듯
        //맘에듬 대신 각 포인트끼리의 거리는 좀 생각해야할듯하다



        //캣멀룸 
        public static Vector3 CatmullRomSpline(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float t2 = t * t;
            float t3 = t * t * t;

            return 0.5f * ((2.0f * p1) + (-p0 + p2) * t + (2.0f * p0) - 5.0f * p1 +4 *p2 - p3 * t2 + (-p0 + 3.0f * p1 - 3.0f *p2 + p3) * 3);
        }



        /// <summary>
        /// 물리스프링힘을 계산하여 다음위치로 반환해주는 함수
        /// </summary>
        /// <param name="stiffness">스프링파워</param>
        /// <param name="damping">댐퍼상수(탄성)</param>
        /// <param name="curPoint">현재위치</param>
        /// <param name="anchorPoint">앵커위치</param>
        /// <param name="velocity">속도</param>
        /// <param name="maintainDistance">거리조절</param>
        /// <param name="deltaTime">시간</param>
        /// <param name="weight">가중치</param>
        /// <param name="airDrag">공기저항</param>
        /// <returns>다음 위치</returns>
        public static Vector3 SimulateSpringForce(float stiffness, float damping, Vector3 curPoint, Vector3 anchorPoint, ref Vector3 velocity, float maintainDistance, float deltaTime, float weight = 1, float airDrag = 1)
        {
            //방향크기//변위
            Vector3 displacement = anchorPoint - curPoint;

            // 스프링 힘 계산//반대방향으로 인한 힘
            //Vector3 springForce = stiffness * displacement;

            //스프링의 방향과 길이 계산
            //스프링에 작용하는 힘 계산 (후크의 법칙)
            float springForceMagnitude = stiffness * (displacement.magnitude - maintainDistance);//일정거리 유지용
            Vector3 springForce = springForceMagnitude * displacement.normalized;

            //댐퍼 힘 계산//크기 감소처리
            //훅의 법칙 계산
            //복원력 = 탄성 * 변형량
            //k == 스프링상수 탄성계수
            //a == 스프링이 변형된 길이
            //f = -k a
            Vector3 dampingForce = -damping * velocity;

            // 총 힘 계산
            Vector3 totalForce = springForce + dampingForce;




            // 가속도 계산 (F = ma)//a = f/m
            //Vector3 acceleration = totalForce / mass;

            // 속도 갱신//공기저항처리
            //velocity += acceleration * deltaTime;
            //velocity += totalForce * (0.5f * airDrag * velocity.magnitude) * deltaTime;//질량제거버전
            velocity += totalForce * weight * 0.5f * airDrag * deltaTime;//질량제거버전//공기저항추가

            //스프링힘으로 인한 다음위치
            curPoint += velocity * deltaTime;
            return curPoint;
        }


        /// <summary>
        /// 물리스프링힘을 계산하여 다음위치로 반환해주는 함수
        /// </summary>
        /// <param name="stiffness">스프링파워</param>
        /// <param name="damping">댐퍼상수(탄성)</param>
        /// <param name="curPoint">현재값</param>
        /// <param name="anchorPoint">앵커값</param>
        /// <param name="velocity">속도</param>
        /// <param name="deltaTime">시간</param>
        /// <param name="weight">가중치</param>
        /// <param name="airDrag">공기저항</param>
        /// <returns>다음값</returns>
        public static float SimulateSpringForce(float stiffness, float damping, float curValue, float anchorValue, ref float velocity, float deltaTime, float weight = 1, float airDrag = 1)
        {
            float displacement = anchorValue - curValue;
            float springForceMagnitude = stiffness * displacement;
            float dampingForce = -damping * velocity;
            float totalForce = springForceMagnitude + dampingForce;

            velocity += totalForce * weight * 0.5f * airDrag * deltaTime;
            curValue += velocity * deltaTime;
            return curValue;
        }















        /// <summary>
        /// 벡터3 배열을 벡터2 배열로 변환시켜주는 함수
        /// </summary>
        /// <param name="vector3Array">벡터3 배열</param>
        /// <returns>벡터2 배열</returns>
        public static Vector2[] ConvertVector2Array(this Vector3[] vector3Array)
        {
            return System.Array.ConvertAll<Vector3, Vector2>(vector3Array, ConvertVector3ToVector2);
        }

        /// <summary>
        /// 벡터int3 배열을 벡터2 배열로 변환시켜주는 함수
        /// </summary>
        /// <param name="vector3Array">벡터int3 배열</param>
        /// <returns>벡터2 배열</returns>
        public static Vector2[] ConvertVector2Array(this Vector3Int[] vector3Array)
        {
            return System.Array.ConvertAll<Vector3Int, Vector2>(vector3Array, ConvertVectorInt3fromVector2);
        }

        /// <summary>
        /// 벡터2 배열을 벡터3 배열로 변환시켜주는 함수
        /// </summary>
        /// <param name="vector2Array">벡터2 배열</param>
        /// <returns>벡터3 배열</returns>
        public static Vector3[] ConvertVector3Array(this Vector2[] vector2Array)
        {
            return System.Array.ConvertAll<Vector2, Vector3>(vector2Array, ConvertVector2ToVector3);
        }

        /// <summary>
        /// 벡터2를 벡터3로 변환시켜주는함수
        /// </summary>
        /// <param name="vector2">벡터2</param>
        /// <returns>벡터3</returns>
        public static Vector3 ConvertVector2ToVector3(Vector2 vector2)
        {
            return new Vector3(vector2.x, vector2.y, 0);
        }

        /// <summary>
        /// 벡터3를 벡터2로 변환시켜주는함수
        /// </summary>
        /// <param name="vector3">벡터3</param>
        /// <returns>벡터2</returns>
        public static Vector2 ConvertVector3ToVector2(Vector3 vector3)
        {
            return new Vector2(vector3.x, vector3.y);
        }

        public static Vector3 ConventXYToXZ(Vector2 vector2)
        {
            return new Vector3(vector2.x, 0, vector2.y);
        }
        public static Vector3 ConventXZToXY(Vector3 vector3)
        {
            return new Vector3(vector3.x, vector3.z, 0);
        }

        /// <summary>
        /// 벡터Int3를 벡터2로 변환시켜주는함수
        /// </summary>
        /// <param name="vector3"></param>
        /// <returns></returns>
        public static Vector2 ConvertVectorInt3fromVector2(Vector3Int vector3)
        {
            return new Vector2(vector3.x, vector3.y);
        }

        public static Vector3Int ConvertVectorXYFromXZ(Vector3Int vector3Int)
        {
            return new Vector3Int(vector3Int.x, 0, vector3Int.y);
        }

        public static Vector3Int ConvertVectorXZFromXY(Vector3Int vector3Int)
        {
            return new Vector3Int(vector3Int.x, vector3Int.z);
        }

        /// <summary>
        /// 각도를 방향으로 변경해주는 함수
        /// </summary>
        /// <param name="angle">각도</param>
        /// <param name="axisDirectionType"></param>
        /// <returns>방향</returns>
        public static Vector3 AngleToDirection(float angle, AxisDirectionType axisDirectionType)
        {
            //전거
            //Vector3 direction = GetSightDirectionType(axisDirectionType);
            //var quaternion = Quaternion.Euler(0, 0, angle);
            //Vector3 newDirection = quaternion * direction;

            //신규
            Vector3 direction = GetAxisDirectionType(axisDirectionType);
            var quaternion = Quaternion.AngleAxis(angle, direction);
            Vector3 newDirection = quaternion.eulerAngles;
            return newDirection;
        }

        /// <summary>
        /// 축 방향타입 설정에 따른 방향을 가져오는 함수
        /// </summary>
        /// <param name="axisDirectionType">축 방향타입</param>
        /// <returns>방향</returns>
        public static Vector3 GetAxisDirectionType(AxisDirectionType axisDirectionType)
        {
            Vector3 direction = Vector3.zero;
            switch (axisDirectionType)
            {
                case AxisDirectionType.X:
                    //direction = sightTrigger.transform.right;
                    direction = Vector2.right;
                    break;
                case AxisDirectionType.Y:
                    //direction = sightTrigger.transform.up;
                    direction = Vector3.up;
                    break;
                case AxisDirectionType.Z:
                    direction = Vector3.forward;
                    break;
            }
            return direction;
        }

        /// <summary>
        /// 축 방향타입 설정에 따른 방향을 가져오는 함수
        /// </summary>
        /// <param name="axisDirectionType">축 방향타입</param>
        /// <param name="targetTr">타겟Tr</param>
        /// <returns>방향</returns>
        public static Vector3 GetAxisDirectionType(AxisDirectionType axisDirectionType, Transform targetTr)
        {
            Vector3 direction = Vector3.zero;
            switch (axisDirectionType)
            {
                case AxisDirectionType.X:
                    //direction = sightTrigger.transform.right;
                    direction = targetTr.right;
                    break;
                case AxisDirectionType.Y:
                    //direction = sightTrigger.transform.up;
                    direction = targetTr.up;
                    break;
                case AxisDirectionType.Z:
                    direction = targetTr.forward;
                    break;
            }
            return direction;
        }

        /// <summary>
        /// 리스트에서 번호 두가지를 스왑하기 위한 함수
        /// </summary>
        /// <typeparam name="T">타입</typeparam>
        /// <param name="list">리스트</param>
        /// <param name="changeIndexA">변경할 인덱스1</param>
        /// <param name="changeIndexB">변경할 인덱스2</param>
        public static void Swap<T>(this List<T> list, int changeIndexA, int changeIndexB)
        {
            T temp = list[changeIndexA];
            list[changeIndexA] = list[changeIndexB];
            list[changeIndexB] = temp;
        }

        /// <summary>
        /// 하이어라키 순서를 인덱스순서를 정하는 함수
        /// </summary>
        /// <param name="component">컴포넌트</param>
        /// <param name="index">인덱스</param>
        public static void SetSibling(this Component component, int index)
        {
             component.transform.SetSibling(index);
        }

        /// <summary>
        /// 하이어라키 순서를 인덱스순서를 정하는 함수
        /// </summary>
        /// <param name="tr">트랜스폼</param>
        /// <param name="index">인덱스</param>
        public static void SetSibling(this Transform tr, int index)
        {
            tr.SetSiblingIndex(index);
        }

        /// <summary>
        /// 트랜스폼의 위치, 회전, 부모, 월드인지에 따라 설정하는 함수
        /// </summary>
        /// <param name="tr">트랜스폼</param>
        /// <param name="pos">위치</param>
        /// <param name="quaternion">회전</param>
        /// <param name="parent">부모</param>
        /// <param name="isWorld">월드좌표계여부</param>
        public static void InitTrObjPrefab(this Transform tr, Vector3 pos, Quaternion quaternion, Transform parent = null, bool isWorld = true)
        {
            //프리팹을 주기적으로 초기화하기때문에 만든 함수
            //순서
            //1. 부모정의
            //2. 이동 회전
            //3. 비활성화 키기체크
            tr.SetParent(parent);//디폴트가 true
            //true인 경우 개체가 이전과 동일한 월드 공간 위치, 회전 및 크기를 유지하도록 상위 상대적 위치, 크기 및 회전이 수정
            //tr.SetParent(parent, false);//참일 경우. 이전에 있던걸 유지하고 옮김
            if (isWorld)
            {
                //tr.position = explorePoint;
                //tr.rotation = quaternion;
                tr.SetPositionAndRotation(pos, quaternion);
            }
            else
            {
                //tr.localPosition = explorePoint;
                //tr.localRotation = quaternion;
                tr.SetLocalPositionAndRotation(pos, quaternion);
            }

            if (tr.gameObject.activeSelf == false)
            {
                tr.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 트랜스폼의 위치, 회전, 부모, 월드인지에 따라 설정하는 함수
        /// </summary>
        /// <param name="component">컴포넌트</param>
        /// <param name="pos">위치</param>
        /// <param name="quaternion">회전</param>
        /// <param name="parent">부모</param>
        /// <param name="isWorld">월드좌표계여부</param>
        public static void InitTrObjPrefab(this Component component, Vector3 pos, Quaternion quaternion, Transform parent = null, bool isWorld = true)
        {
            InitTrObjPrefab(component.transform, pos, quaternion, parent, isWorld);
        }

        /// <summary>
        /// 트랜스폼의 위치, 회전, 부모, 월드인지에 따라 설정하는 함수
        /// </summary>
        /// <param name="tr">트랜스폼</param>
        /// <param name="targetTr">변경시킬 위치와 회전</param>
        /// <param name="parent">부모</param>
        public static void InitTrObjPrefab(this Component component, Transform targetTr , Transform parent = null)
        {
            InitTrObjPrefab(component.transform, targetTr.position, targetTr.rotation, parent);
        }

        /// <summary>
        /// 트랜스폼의 위치, 부모, 월드인지에 따라 설정하는 함수(회전제외)
        /// </summary>
        /// <param name="tr">트랜스폼</param>
        /// <param name="pos">위치</param>
        /// <param name="parent">부모</param>
        /// <param name="isWorld">월드좌표계여부</param>
        public static void InitTrObjPrefab(this Transform tr, Vector3 pos, Transform parent = null, bool isWorld = true)
        {   
            InitTrObjPrefab(tr, pos, tr.rotation, parent, isWorld);
        }

        /// <summary>
        /// 트랜스폼의 위치, 부모, 월드인지에 따라 설정하는 함수(회전제외)
        /// </summary>
        /// <param name="component">컴포넌트</param>
        /// <param name="pos">위치</param>
        /// <param name="parent">부모</param>
        /// <param name="isWorld">월드좌표계여부</param>
        public static void InitTrObjPrefab(this Component component, Vector3 pos, Transform parent = null, bool isWorld = true)
        {
            Transform tr = component.transform;
            InitTrObjPrefab(tr, pos, tr.rotation, parent, isWorld);
        }


        /// 렉트트랜스폼 자기자신의 앵커의 기준으로 위치를 새롭게 배치해주는 함수
        /// </summary>
        /// <param name="rect">렉트트랜스폼</param>
        /// <param name="norDirVec">방향노말벡터</param>
        /// <param name="spacing">간격</param>
        /// <param name="index">인덱스</param>
        public static void FocusRectAnchorPos(this RectTransform rect, Vector2 norDirVec, float spacing, int index)
        {
            //피봇은 건들이유 없음
            //앵커임
            //a, b//좌우, 높낮이//최소 최대 0~1
            Vector2 anchorMin = rect.anchorMin;
            Vector2 anchorMax = rect.anchorMax;
            float width = rect.sizeDelta.x;
            float height = rect.sizeDelta.y;            

            //따로 계산법이 있을만한데
            //1000 20
            //minx maxx explorePoint 
            //0 0 500
            //1 1 -500
            //0.5 0.5 0

            //x끼리
            //500 0 -500
            // 0 0.5  1

            //앵커위치만처리할려고하니 둘중하나만 사용하면 됨
            if (anchorMin == anchorMax)
            {  
                //1000*0.5f;

                //0 => -0.5
                //0.5 => 0
                //1 => 0.5
                //역수처리
                float xPos = -(anchorMin.x - 0.5f) * width;
                float yPos = -(anchorMin.y - 0.5f) * height;
                Vector2 newPos = new Vector2(xPos, yPos);//인덱스가 0 일때의 값

                //방향과 스페이싱처리
                if (index != 0)
                {
                    xPos = norDirVec.x * index * (width + spacing);
                    yPos = norDirVec.y * index * (height + spacing);
                    newPos += new Vector2(xPos, yPos);
                }

                rect.anchoredPosition = newPos;
            }

        }

        //안될텐데 확인만 해보자
        public static void SetAlpha(this Color color, float a)
        {
            Color tempColor = color;
            tempColor.a = a;
            color = tempColor;



            //Color color = fill.color;
            //color.a = a;
            //fill.color = color;
        }

        /// <summary>
        /// 컬러256세팅 함수
        /// </summary>
        /// <param name="color">컬러</param>
        /// <param name="r">빨간</param>
        /// <param name="g">초록</param>
        /// <param name="b">블루</param>
        /// <param name="a">알파</param>
        public static Color GetColor256(this Color color, float r, float g, float b, float a = 256)
        {
            //컬러구조체의 각요소는 0~1값을 사용
            //0쯤이면 나누기 하지말고 그대로 처리
            r = r < 0.001f ? 0 : Mathf.Clamp(r, 0, 256) / 256f;
            g = g < 0.001f ? 0 : Mathf.Clamp(g, 0, 256) / 256f;
            b = b < 0.001f ? 0 : Mathf.Clamp(b, 0, 256) / 256f;
            a = a < 0.001f ? 0 : Mathf.Clamp(a, 0, 256) / 256f;

            return new Color(r,g,b,a);
        }


        /// <summary>
        /// 컬러256세팅 함수
        /// </summary>        
        /// <param name="r">빨간</param>
        /// <param name="g">초록</param>
        /// <param name="b">블루</param>
        /// <param name="a">알파</param>
        public static Color GetColor256(float r, float g, float b, float a = 256)
        {
            //컬러구조체의 각요소는 0~1값을 사용
            //0쯤이면 나누기 하지말고 그대로 처리
            r = r < 0.001f ? 0 : Mathf.Clamp(r, 0, 256) / 256f;
            g = g < 0.001f ? 0 : Mathf.Clamp(g, 0, 256) / 256f;
            b = b < 0.001f ? 0 : Mathf.Clamp(b, 0, 256) / 256f;
            a = a < 0.001f ? 0 : Mathf.Clamp(a, 0, 256) / 256f;

            return new Color(r, g, b, a);
        }

        /// <summary>
        /// HSV + A에 대한 컬러를 가져오는 함수
        /// </summary>
        /// <param name="h">H</param>
        /// <param name="s">S</param>
        /// <param name="v">V</param>
        /// <param name="a">A</param>
        /// <returns>컬러</returns>
        public static Color GetHSVAColor(float h, float s, float v, float a)
        {
            var color = Color.HSVToRGB(h / 360,s / 100, v / 100);
            color.a = a / 100;
            return color;
        }

        /// <summary>
        /// 게임오브젝트를 활성화/비활성화시켜주는 함수
        /// </summary>
        /// <param name="component">컴포넌트</param>
        /// <param name="value">활성화/비활성화</param>
        public static void SetActive(this Component component, bool value)
        {
            component.gameObject.SetActive(value);
        }

        /// <summary>
        /// 컴포넌트를 활성화/비활성화시켜주는 함수
        /// </summary>
        /// <param name="behaviour">컴포넌트</param>
        /// <param name="value">활성화/비활성화</param>
        public static void SetEnable(this Behaviour behaviour, bool value)
        {
            behaviour.enabled = value;
        }

        /// <summary>
        /// vector3로 회전처리하되 짐벌락부분을 처리한 회전함수
        /// </summary>
        /// <param name="tr">트랜스폼</param>
        /// <param name="rotateEuler">회전될 각</param>
        public static void RotateEuler(this Transform tr, Vector3 rotateEuler)
        {
            //오일러의 짐벌락문제를 해결하기 위해 쿼터니언을 사용한 vector3처리
            //이걸로 해야지만 축의 고정이 안풀림
            tr.rotation = Quaternion.AngleAxis(rotateEuler.x, Vector3.right) * Quaternion.AngleAxis(rotateEuler.y, Vector3.up) * Quaternion.AngleAxis(rotateEuler.z, Vector3.forward);
        }

        /// <summary>
        /// vector3로 회전처리하되 짐벌락부분을 처리한 회전함수
        /// </summary>
        /// <param name="tr">트랜스폼</param>
        /// <param name="rotateEuler">회전될 각</param>
        /// <param name="targetAxisTr">타겟이될 축</param>
        public static void RotateEuler(this Transform tr, Vector3 rotateEuler , Transform targetAxisTr)
        {   
            tr.rotation = Quaternion.AngleAxis(rotateEuler.x, targetAxisTr.right) * Quaternion.AngleAxis(rotateEuler.y, targetAxisTr.up) * Quaternion.AngleAxis(rotateEuler.z, targetAxisTr.forward);
        }

        /// <summary>
        /// 특정 위치를 중심으로 회전된 벡터를 반환하는 함수
        /// </summary>
        /// <param name="originPos">회전할</param>
        /// <param name="pivot">회전중심</param>
        /// <param name="rotation">회전</param>
        /// <returns>회전된 위치</returns>
        public static Vector3 GetRotatePosForPivot(Vector3 originPos, Vector3 pivot, Quaternion rotation)
        {
            // 특정 위치를 중심으로 회전된 벡터를 반환
            return rotation * (originPos - pivot) + pivot;
        }

#if Util3D


        //3d용


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hitInfo"></param>
        /// <param name="target"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        private static bool CheckCast<T>(RaycastHit hitInfo, ref T target, string tag = null)
        {
            //태그 컴포넌트 체크구역 
            //태그체크
            if (!string.IsNullOrEmpty(tag))
            {
                if (!hitInfo.collider.CompareTag(tag))
                {
                    return false;
                }
            }

            //컴포넌트체크
            if (!hitInfo.collider.TryGetComponent(out target))
            {
                return false;
            }
            return true;
        }

        public static bool RayCast<T>(this Component component, out T target, Ray ray, LayerMask checkLayer, float distance = Mathf.Infinity, string tag = null) where T : Component
        {            
            target = null;
            if (!Physics.Raycast(ray, out RaycastHit hitInfo, distance, checkLayer))
            {
                return false;
            }
            return CheckCast(hitInfo, ref target, tag);
        }
        public static bool LineCast<T>(this Component component, out T target, Vector3 startPos, Vector3 endPos, LayerMask checkLayer, string tag = null) where T : Component
        {
            target = null;
            if (!Physics.Linecast(startPos, endPos, out RaycastHit hitInfo, checkLayer))
            {
                return false;
            }
            return CheckCast(hitInfo, ref target, tag);
        }
        public static bool SphereCast<T>(this Component component, out T target, Vector3 pos, float size, LayerMask layerMask, float distance = 0, string tag = null) where T : Component
        {
            target = null;
            if (!Physics.SphereCast(pos, size, Vector3.up, out RaycastHit hitInfo, distance, layerMask))
            {
                return false;
            }
            return CheckCast(hitInfo, ref target, tag);
        }

        public static bool BoxCast<T>(this Component component, out T target, Vector3 pos, Vector3 size, Quaternion quaternion, LayerMask layerMask, float distance = 0, string tag = null) where T : Component
        {
            target = null;
            if (Physics.BoxCast(pos, size * 0.5f, Vector3.up, out RaycastHit hitInfo, quaternion,distance,layerMask))
            {
                return false;
            }
            return CheckCast(hitInfo, ref target, tag);
        }

        public static bool CapsuleCast<T>(this Component component, out T target, Vector3 posDown, Vector3 posUp, float size, LayerMask layerMask, float distance = 0, string tag = null) where T : Component
        {
            target = null;
            if (!Physics.CapsuleCast(posDown, posUp, size, Vector3.up, out RaycastHit hitInfo, distance, layerMask))
            {
                return false;
            }
            return CheckCast(hitInfo, ref target, tag);
        }
        

        public static bool CylinderCast<T>(this Component component, out T target, Vector3 pos, Vector3 size, Quaternion quaternion, LayerMask layerMask, float cylinderDistance, bool isYAxis, float distance = 0, string tag = null) where T : Component
        {
            target = null;
            if (Physics.BoxCast(pos, size * 0.5f, Vector3.up, out RaycastHit hitInfo, quaternion, distance, layerMask))
            {
                return false;
            }

            Gizmos.DrawWireCube(pos, size);
            Vector3 originTr = quaternion * pos;
            originTr.y = hitInfo.point.y;//

            //Y축기준으로 거리를 잰다면
            Vector3 newPos = isYAxis ? new Vector3(originTr.x, hitInfo.point.y) : new Vector3(hitInfo.point.x, originTr.y);

            //거리 체크구역
            if (!CheckDistance(originTr, newPos, cylinderDistance))
            {
                return false;
            }

            return CheckCast(hitInfo, ref target, tag);
        }

#endif

        //사용법 : CrossVec(transform.up, transform.forward, target.position)
        /// <summary>
        /// 외적구해서 우측인지 좌측인지 알수 있는 함수
        /// </summary>
        /// <param name="norVecUp">노말벡터 위</param>
        /// <param name="norVecForward">노말벡터 전방</param>
        /// <param name="targetPos">타겟위치</param>
        /// <returns>-1 좌측, +1 우측</returns>
        public static float CrossVec(Vector3 norVecUp, Vector3 norVecForward, Vector3 targetPos)
        {
            Vector3 p = norVecForward - norVecUp;
            Vector3 q = targetPos - norVecForward;
            return Vector3.Cross(p, q).y;
        }

        public static bool CheckRight(Vector3 norVecUp, Vector3 norVecForward, Vector3 targetPos)
        {
            float yValue = CrossVec(norVecUp, norVecForward, targetPos);
            return yValue > 0;
        }


        //(1,3) (2,4)
        //내적
        //1*2 + 3*4 = 14(길이값, 크기) //살펴보니까 삼각함수의 코싸인과 같다

        public static float GetDot2DValue(Transform curTr, Transform targetTr)
        {
             return GetDotValue(curTr.up, targetTr.up);
        }

        public static float GetDotValue(Vector3 trNorVec, Vector3 targetNorVec)
        {
            float scale = Vector2.Dot(trNorVec, targetNorVec);
            return scale;
        }

        public static bool CheckForward(Vector3 trNorVec, Vector3 targetNorVec)
        {
            float scale = GetDotValue(trNorVec, targetNorVec);
            //0 초과이면 전방(1)  이하이면 후방(-1)
            return scale > 0;
        }

        /// <summary>
        /// 두라인이 겹쳤을때 
        /// </summary>
        /// <param name="pointAStart"></param>
        /// <param name="pointAEnd"></param>
        /// <param name="pointBStart"></param>
        /// <param name="pointBEnd"></param>
        /// <param name="intersection"></param>
        /// <returns></returns>
        public static bool LineSegmentsIntersection(Vector2 pointAStart, Vector2 pointAEnd, Vector2 pointBStart, Vector2 pointBEnd, out Vector2 intersection)
        {
            intersection = Vector2.zero;

            var d = (pointAEnd.x - pointAStart.x) * (pointBEnd.y - pointBStart.y) - (pointAEnd.y - pointAStart.y) * (pointBEnd.x - pointBStart.x);

            if (d == 0.0f)
            {
                return false;
            }

            var u = ((pointBStart.x - pointAStart.x) * (pointBEnd.y - pointBStart.y) - (pointBStart.y - pointAStart.y) * (pointBEnd.x - pointBStart.x)) / d;
            var v = ((pointBStart.x - pointAStart.x) * (pointAEnd.y - pointAStart.y) - (pointBStart.y - pointAStart.y) * (pointAEnd.x - pointAStart.x)) / d;

            if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f)
            {
                return false;
            }

            intersection.x = pointAStart.x + u * (pointAEnd.x - pointAStart.x);
            intersection.y = pointAStart.y + u * (pointAEnd.y - pointAStart.y);

            return true;
        }


        //이방식은 현재 rigidbody2D와 프로젝트 세팅의 physics2d time탭에
        //설정된 값을 사용해서 물리적인 연산처리를 한결과.
        //어떤물체와 어떤 충돌할지는 예측할수 없는 코드.
        //리지드바디에 힘이 주어져 발사하게 되었을때 궤도를 얻기위한 함수

        public static Vector2[] Physics2DSimurate(Vector2 pos, Vector2 velocity, int frameCount, Rigidbody2D rb2d)
        {
            Vector2[] pointArray = new Vector2[frameCount];

            //Time.fixedDeltaTime : 한프레임당 소비되는 물리시간
            //Physics2D.velocityIterations : 물리연산의 반복횟수
            //0.02 / 8
            float timeStep = Time.fixedDeltaTime / Physics2D.velocityIterations;


            //한프레임당 적용할 중력값
            Vector2 gravityAccel = Physics2D.gravity * rb2d.gravityScale * timeStep * timeStep;

            //한프레임당 적용할 저항력을 구함
            float drag = 1 - timeStep * rb2d.linearDamping;

            //시작스텝을 구하고 시뮬레이션
            Vector2 moveStep = velocity * timeStep;
            for (int i = 0; i < frameCount; i++)
            {
                moveStep += gravityAccel;//속도
                moveStep *= drag;//저항력
                pos += moveStep;//위치값에 누적처리
                pointArray[i] = pos;//지정
            }

            //리턴
            return pointArray;
        }




        //3차원 좌표계를 UI좌표계로 변경하는 함수 (비율계산)
        //주의점 월드맵사이즈를 제대로 맞추어줘야됨//맞추어주는 기능을 가진 무언가가 필요
        //월드 맵
        public static Vector2 WorldPosToMapPos(Vector3 wolrdPos, float worldWidth, float worldDepth, float uiMapWidth, float uiMapHeight)
        {
            Vector2 result = Vector2.zero;
            result.x = (wolrdPos.x * uiMapWidth) / worldWidth;
            result.y = (wolrdPos.z * uiMapHeight) / worldDepth;

            return result;
        }

        //맵에서 월드
        public static Vector3 MapPosToWorldPos(Vector3 uiPos, float worldWidth, float worldDepth, float uiMapWidth, float uiMapHeight)
        {
            Vector3 result = Vector3.zero;
            result.x = (uiPos.x * worldWidth) / uiMapWidth;
            result.z = (uiPos.y * worldDepth) / uiMapHeight;

            return result;
        }

        //월드 방향을 맵 방향으로
        public static void MapLookAt(Transform worldPlayer, Transform uiPlayer)
        {
            float angleZ = Mathf.Atan2(worldPlayer.forward.z, worldPlayer.forward.x) * Mathf.Rad2Deg;
            uiPlayer.rotation = Quaternion.Euler(0, 0, angleZ - 90);
            //uiPlayer.eulerAngles = new Vector3 (0, 0, angleZ - 90);
        }

        /// <summary>
        /// HEX컬러를 RGBA 컬러로 변환해주는 함수
        /// </summary>
        /// <param name="hexColor">HEX컬러</param>
        /// <param name="rgbaColor">RGBA컬러</param>
        /// <returns></returns>
        public static bool ConvertHexColorToRGBAColor(string hexColor, out Color rgbaColor)
        {   
            return ColorUtility.TryParseHtmlString(hexColor, out rgbaColor);
        }

        /// <summary>
        ///RGBA컬러를 HEX컬러로 변환해주는 함수
        /// </summary>
        /// <param name="rgbaColor">RGBA컬러</param>
        /// <returns>HEX컬러</returns>
        public static string ConvertRGBAColorToHEXColor(Color rgbaColor)
        {
            return ColorUtility.ToHtmlStringRGBA(rgbaColor);
        }

        /// <summary>
        /// 일정값을 특정값으로 반올림시켜주는 함수.
        /// RoundToInt는 int로 만되니 대신만드는것
        /// </summary>
        /// <param name="value">현재 값</param>
        /// <param name="checkValue">기준이 될 위치값</param>
        /// <param name="range">범위</param>
        /// <returns>라운드될 값안에 들어왔는지 여부</returns>
        public static bool CheckRoundToNear(float value, float checkValue, float range)
        {
            float lowerBound = checkValue - range;
            float upperBound = checkValue + range;

            //라운드 범위체크
            //0 ~ 0.4, 0.5~0.9
            //0<=0.5, 0.5 < (1 == 0.9999..)
            return lowerBound <= value && value < upperBound;
        }

        /// <summary>
        /// 일정값을 특정값으로 반올림시켜주는 함수.
        /// RoundToInt는 int로 만되니 대신만드는것
        /// </summary>
        /// <param name="value">현재 값</param>
        /// <param name="checkValue">기준이 될 위치값</param>
        /// <param name="range">범위</param>
        /// <returns>라운드된 최종값</returns>
        public static float RoundToNear(float value, float checkValue, float range)
        {
            //범위체크
            if (CheckRoundToNear(value, checkValue, range))
            {
                //안이면 체크할밸류로 넘김
                return checkValue;
            }
            else
            {
                //밖이면 그대로 넘김
                return value;
            }
        }

        /// <summary>
        /// 반올림함수 (마이너스도 됨)
        /// </summary>
        /// <param name="value">값</param>
        /// <returns>반올림된 값</returns>
        public static int RoundToInt(this float value)
        {   
            int result = (int)(value + 0.5f * System.Math.Sign(value));
            //FuncTracer.TraceFunctionCheckPoint();
            return result;
        }

        /// <summary>
        /// 소수점 반올림함수
        /// </summary>
        /// <param name="value">값</param>
        /// <param name="decimalPlace">반올림할 소수점위치</param>
        /// <returns>반올림 값</returns>
        public static float RoundToFloat(this float value, int decimalPlace)
        {
            //float mul = (float)System.Math.Pow(10, decimalPlace);
            //return (float)(System.Math.Round(value * mul) / mul);

            var mul = Pow(10f, decimalPlace);
            return (float)(System.Math.Round(value * mul) / mul);
        }

        /// <summary>
        /// 거듭제곱
        /// </summary>
        /// <param name="value">값</param>
        /// <param name="powCount">거듭제곱횟수</param>
        /// <returns>거듭</returns>
        public static float Pow(float value, int powCount)
        {
            //Math.Pow가 생각보다 느리다. Math.sign은 빠른데
            float factor = 1f;
            for (int i = 0; i < powCount; i++)
            {
                factor *= value;
            }
            return factor;
        }

        /// <summary>
        /// 최대최소로 제한된 값으로 변환하는 함수
        /// </summary>
        /// <param name="cur">현재값</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        /// <returns>변환값</returns>
        public static float GetLimitAmount(this float cur, float min, float max)
        {
            cur = System.Math.Clamp(cur, min, max);
            return cur;
        }

        /// <summary>
        /// 최대최소로 제한된 값으로 변환하는 함수
        /// </summary>
        /// <param name="cur">현재값</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        /// <returns>변환값</returns>
        public static int GetLimitAmount(this int cur,in int min, in int max)
        {
            cur = System.Math.Clamp(cur, min, max);
            return cur;
        }

        /// <summary>
        /// 최소제한량을 체크후 변환하는 함수
        /// </summary>
        /// <param name="cur">현재값</param>
        /// <param name="min">최소값</param>
        /// <returns>변환값</returns>
        public static float GetLimitMin(this float cur, float min)
        {
            cur = cur < min ? min : cur;
            return cur;
        }

        /// <summary>
        /// 최소제한량을 체크후 변환하는 함수
        /// </summary>
        /// <param name="cur">현재값</param>
        /// <param name="min">최소값</param>
        /// <returns>변환값</returns>
        public static int GetLimitMin(this int cur, int min)
        {
            cur = cur < min ? min : cur;
            return cur;
        }

        /// <summary>
        /// 최대제한량을 체크후 변환하는 함수
        /// </summary>
        /// <param name="cur">현재값</param>
        /// <param name="max">최대값</param>
        /// <returns>변환값</returns>
        public static float GetLimitMax(this float cur, float max)
        {
            cur = cur < max ? max : cur;
            return cur;
        }

        /// <summary>
        /// 최대제한량을 체크후 변환하는 함수
        /// </summary>
        /// <param name="cur">현재값</param>
        /// <param name="max">최대값</param>
        /// <returns>변환값</returns>
        public static int GetLimitMax(this int cur, int max)
        {
            cur = cur > max ? max : cur;
            return cur;
        }

        /// <summary>
        /// 인덱스를 받으면 콜렉션 크기를 확인후 제한선내인지 체크하는 함수
        /// </summary>        
        /// <param name="collection">콜렉션</param>
        /// <param name="index">인덱스</param>
        /// <returns>제한선내 여부</returns>
        public static bool GetLimitCollectionLength(this ICollection collection, int index)
        {
            index = index.GetLimitMin(0);
            return index < collection.Count;
        }

        //현재 아래함수는 불필요하게 작업이 많음. 나중에 체크후 분리하거나 제거하기

        /// <summary>
        /// 트리거함수(딸칵)
        /// </summary>
        /// <param name="checkAction">상태체크액션</param>
        /// <param name="stateTrueTriggerAction">트루상태트리거액션</param>
        /// <param name="stateFalseTriggerAction">폴스상태트리거액션</param>
        /// <param name="stateTrueUpdateAction">트루상태업데이트액션</param>
        /// <param name="state">상태</param>
        /// <param name="prevState">이전상태</param>
        public static void ActionTrigger(in System.Func<bool> checkAction, in System.Action stateTrueTriggerAction, in System.Action stateFalseTriggerAction, in System.Action stateTrueUpdateAction, ref bool state, ref bool prevState)
        {
            state = checkAction();
            if (state != prevState)
            {
                //다를시에만 작동
                if (prevState)
                {
                    stateFalseTriggerAction?.Invoke();
                    //Debug.Log("Exit");
                }
                else
                {
                    stateTrueTriggerAction?.Invoke();
                    //Debug.Log("Enter");
                }
                prevState = state;
            }

            if (!state)
            {
                return;
            }

            //Debug.Log("Update");
            stateTrueUpdateAction?.Invoke();
        }

        /// <summary>
        /// 인풋상태값이 변했을시 특정액션을 작동되게 해주는 함수
        /// </summary>
        /// <param name="prevState">이전상태</param>
        /// <param name="newState">신규상태</param>
        /// <param name="stateAction">상태액션</param>        
        public static void UpdateStateTrigger(ref bool prevState, in bool newState, in System.Action<bool> stateAction)
        {
            if (prevState == newState)
            {
                return;
            }

            prevState = newState;
            stateAction?.Invoke(newState);
        }

        /// <summary>
        /// 넉백후 방향위치
        /// </summary>
        /// <param name="explorePoint">터지는 위치</param>
        /// <param name="targetPoint">대상위치</param>
        /// <param name="range">범위</param>
        /// <param name="power">파워</param>
        /// <param name="sizeThreshold">오프셋임계값</param>
        /// <returns></returns>

        public static Vector3 GetNuckBackDirectionPos(Vector3 explorePoint, Vector3 targetPoint, float range, float power, float sizeThreshold = 0f, bool isReverse = false)
        {
            var targetPos = targetPoint;
            var distance = GetDistance(explorePoint, targetPos);

            //현재 거리가 범위와 임계값보다 크면 작동되게
            if (distance > range + sizeThreshold)
            {
                return Vector3.zero;
            }

            //가까우면 가까울수록 멀리 밀림//멀면 별로 안밀림
            //최대거리에서 현재거리를 뺀다음
            var dir = targetPos - explorePoint;
            var powerDistance = range - distance;
            dir = dir.normalized * ((powerDistance) * power) * (isReverse ? -1 : 1);
            return dir;
        }

        public static void ActionScreenShot(string screenShotName)
        {
            RenderTexture renderTexture = Camera.main.targetTexture;
            Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            RenderTexture.active = renderTexture;
            texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture2D.Apply();

            File.WriteAllBytes($"{Application.dataPath}/{screenShotName}.png", texture2D.EncodeToPNG());

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }


        /// <summary>
        /// 리터널(상수폴)에 등록
        /// </summary>
        /// <param name="content">컨텐츠</param>
        /// <returns>컨텐츠</returns>
        public static string CheckAndAddReternal(in string content)
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

    }

    //※비교자 제작시 확인할것

    //Sort() 메서드를 실행하면 내부적으로 해당 리스트 안에 있는 객체의 IComparable 인터페이스의 함수, CompareTo(T)를 실행.
    //CompareTo(T)는 비교할 다른 객체 T를 인자로 받아 해당 객체가 비교할 객체보다 크면 양수를 반환, 비교할 객체가 더 크면 음수를 반환
    //자기가 크면 양수
    //남이 크면 음수


    //이를 토대로 List는 정렬을 진행하게 되는 것이죠.


    //IComparable//객체에 정렬기능을 집어넣어 객체를 정렬할수 있게
    //IComparer//원하는부분들을 정렬하기 위해 존재//정렬하는 방법만 정의하는 객체

    //Using문 체크
    //IComparer => using System.Collections;
    //IComparer<T> => using System.Collections.Generic;

    //차순관련
    //디폴트(기본)는 오름차순
    //ASC//Ascending(오름차순)//작은 값부터 큰 값 쪽으로의 순서
    //DESC//Descending(내림차순)//큰 값부터 작은 값 쪽으로의 순서


    /// <summary>
    /// 정렬기초
    /// </summary>
    public abstract class SortBase
    {
        protected bool isAsc;

        public SortBase(bool isAsc)
        {
            this.isAsc = isAsc;
        }

        /// <summary>
        /// 정렬타입 설정함수
        /// </summary>
        /// <param name="isAsc">오름차순 여부</param>
        public void SetAsc(bool isAsc)
        {
            this.isAsc = isAsc;
        }

        /// <summary>
        /// ASC//Ascending(오름차순)//작은 값부터 큰 값 쪽으로의 순서
        /// DESC//Descending(내림차순)//큰 값부터 작은 값 쪽으로의 순서
        /// </summary>
        /// <param name="isASCState">오름차순 여부</param>
        /// <returns>정렬할 수</returns>
        public static int GetASC(bool isASCState)
        {
            return isASCState ? 1 : -1;
        }
    }

    /// <summary>
    /// 거리비교자
    /// </summary>
    public class DistanceSort : SortBase, IComparer<Vector3>, IComparer<Transform>
    {
        private Vector3 compareVector;

        public DistanceSort(bool isAsc, Vector3 value) : base(isAsc)
        {
            compareVector = value;
        }

        /// <summary>
        /// 위치 지정함수
        /// </summary>
        /// <param name="value">위치</param>
        public void SetPoint(Vector3 value)
        {
            compareVector = value;
        }

        public int Compare(Vector3 posX, Vector3 posY)
        {
            Vector3 offset = posX - compareVector;
            float xDistance = offset.sqrMagnitude;

            offset = posY - compareVector;
            float yDistance = offset.sqrMagnitude;

            //정렬
            int value = GetASC(isAsc);
            return xDistance.CompareTo(yDistance) * value;
        }

        public int Compare(Transform trX, Transform trY)
        {
            return Compare(trX.position, trY.position);
        }
    }

    /// <summary>
    /// 값비교자
    /// </summary>
    public class ValueSort : SortBase, IComparer<float>, IComparer<int>
    {
        public ValueSort(bool isAsc) : base(isAsc) { }

        public int Compare(float x, float y)
        {
            int value = GetASC(isAsc);
            return x.CompareTo(y) * value;
        }

        public int Compare(int x, int y)
        {
            int value = GetASC(isAsc);
            return x.CompareTo(y) * value;
        }
    }

    [System.Serializable]
    public class RotateTurretObject
    {
        public RotateType rotateType;
        public float rotateSpeed;
        public Transform targetObject;
    }


    /// <summary>
    /// 회전타입
    /// </summary>
    public enum RotateType
    {
        Slerp,
        Lerp,
        Turret,
    }

    /// <summary>
    /// 각 축에 대한 기준을 위한 타입
    /// </summary>
    public enum AxisDirectionType
    {
        X,//X 방향
        Y,//Y 방향
        Z,//Z 방향
    }
}
