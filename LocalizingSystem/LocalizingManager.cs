#if MEC && Localize
using UnityEngine;
using System;
using TMPro;
using System.Collections.Generic;
using MEC;
using lLCroweTool.Localize;

namespace lLCroweTool
{
    public class LocalizingManager : MonoBehaviour
    {
        private static LocalizingManager instance;
        public static LocalizingManager Instance
        {
            get
            {
                if (ReferenceEquals(instance, null))
                {
                    instance = FindObjectOfType<LocalizingManager>();
                    //if (ReferenceEquals(instance, null))
                    if (ReferenceEquals(instance, null))
                    {
                        GameObject gameObject = new GameObject();
                        instance = gameObject.AddComponent<LocalizingManager>();
                        gameObject.name = "-=LocalizingManager=-";
                        //localizeDataSheet = Resources.Load();로드
                        // ReSharper disable once ArrangeStaticMemberQualifier
                        //_instance = (MasterAudio)GameObject.FindObjectOfType(typeof(MasterAudio));
                        //return _instance;
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 번역정보를 찾을때 없으면 등록시키기위한 변수//어차피 리드온리라서 에디터에서만 사용가능
        /// </summary>
        public bool isUseRegister = false;

        /// <summary>
        /// 현지화할 언어타입
        /// </summary>
        public LanguageType languageType;

        /// <summary>
        /// 현지화 데이터시트
        /// </summary>
        public LocalizeDBObjectScript localizeDBData;//번역정보가 들어가있는 데이터시트
        //private Hashtable localizeDataHashTable;//해쉬테이블로 바꾸어보자//딕셔너리가 더좋음//딕셔너리로
        //20221106 나중에 어드레서블로 바꿔보자

        /// <summary>
        /// 드랍다운
        /// </summary>
        public TMP_Dropdown dropdown;

        /// <summary>
        /// 캐싱할 텍스트
        /// </summary>
        private string text;

        private CoroutineHandle coroutineHandle;

        private void Awake()
        {
            instance = this;


            if (dropdown != null)
            {
                List<string> options = new List<string>();
                dropdown.options.Clear();
                var tempArray = Enum.GetNames((typeof(LanguageType)));
                for (int i = 0; i < tempArray.Length; i++)
                {
                    options.Add(tempArray[i]);
                }
                dropdown.AddOptions(options);

                dropdown.value = (int)languageType;
                dropdown.RefreshShownValue();
            }
        }

        private void Start()
        {
            UpdateLocalize();
        }

        /// <summary>
        /// 씬상에 있는 로컬라이징 타겟을 업데이트시켜주는 함수
        /// </summary>
        public void UpdateLocalize()
        {
            if (!coroutineHandle.IsRunning)
            {
                coroutineHandle = Timing.RunCoroutine(UpdateLocalizeCoroutine());
            }
        }

        public IEnumerator<float> UpdateLocalizeCoroutine()
        {
            LocalizingTarget[] localizingTargetArray = FindObjectsOfType<LocalizingTarget>();
            
            for (int i = 0; i < localizingTargetArray.Length; i++)
            {
                localizingTargetArray[i].SetText(GetLocalLizeText(localizingTargetArray[i].GetStringID()));
                yield return Timing.WaitForOneFrame;
            }
        }

        /// <summary>
        /// 현지화한 텍스트를 가져오는 함수
        /// </summary>
        /// <param name="localize_ID">현지화할 텍스트아이디</param>
        /// <returns>현지화된 텍스트</returns>
        public string GetLocalLizeText(string localize_ID)
        { 
            text = localize_ID + "-=Null of Data=-";
            //검색후 해당 텍스트가 있으면 그걸 반환시킴
            if (localizeDBData != null)
            {
                //if (localizeDBData.localizeDataBible.ContainsKey(localize_ID))
                if (localizeDBData.localizeDataBible.TryGetValue(localize_ID, out LocalizeData localizeData))
                {
                    //LocalizeData localizeData = localizeDBData.localizeDataBible[localize_ID];
                    switch (languageType)
                    {
                        case LanguageType.Kor:
                            text = localizeData.kor;
                            break;
                        case LanguageType.Eng:
                            text = localizeData.eng;
                            break;
                    }
                    if (text.Length == 0)
                    {
                        text = localize_ID + "NullofData for Language";
                    }
                }
                else if (isUseRegister)
                {
                    //새로등록
                    LocalizeData tempData = new LocalizeData();
                    tempData.string_ID = localize_ID;
                    tempData.kor = localize_ID + "NullofData for Language";
                    tempData.eng = localize_ID + "NullofData for Language";
                    localizeDBData.localizeDataBible.Add(tempData.string_ID, tempData);
                }
            }
            return text;
        }

        /// <summary>
        /// 드랍박스(UI)를 세팅해주는 함수
        /// </summary>
        /// <param name="LanguageTypeIndex">언어타입 정수</param>
        public void SetLanguageType(int LanguageTypeIndex)
        {
            languageType = (LanguageType)LanguageTypeIndex;
            UpdateLocalize();
        }

        public enum LanguageType
        {
            Kor,
            Eng,
        }
    }
}
#endif