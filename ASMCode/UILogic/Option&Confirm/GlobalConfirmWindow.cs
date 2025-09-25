using UnityEngine;

namespace lLCroweTool.UI.Confirm
{
    public class GlobalConfirmWindow : ConfirmWindow
    {

        private static GlobalConfirmWindow instance;
        public static GlobalConfirmWindow Instance
        {
            get
            {
                if (ReferenceEquals(instance, null))
                {
                    instance = FindObjectOfType<GlobalConfirmWindow>();
                    //if (ReferenceEquals(instance, null))
                    if (ReferenceEquals(instance, null))
                    {
                        GameObject gameObject = new GameObject();
                        instance = gameObject.AddComponent<GlobalConfirmWindow>();
                        gameObject.name = "-=GlobalConfirmWindow=-";
                        //localizeDataSheet = Resources.Load();로드
                        // ReSharper disable once ArrangeStaticMemberQualifier
                        //_instance = (MasterAudio)GameObject.FindObjectOfType(typeof(MasterAudio));
                        //return _instance;
                    }
                }
                return instance;
            }
        }
        protected override void Awake()
        {
            base.Awake();
            if (instance != null)
            {
                if (instance.gameObject != gameObject)
                {
                    Destroy(instance.gameObject);
                }
            }
            //Application.isPlaying//종료할때도 작동되있음//20230326
            //instance = transform.GetComponent<T>();                        
            instance = this;
            //루트오브젝트가 돈디스트로이가 걸린다면 그자식오브젝트는 안먹는다            
            DontDestroyOnLoad(gameObject);//루트 오브젝트일시에만 작동됨//루트오브젝트가 아닐시 따로 작동은 안됨
            //instance = FindObjectOfType<T>();
        }
    }
}