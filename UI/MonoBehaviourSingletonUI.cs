using UnityEngine;

namespace lLCroweTool.SingletonUI
{

    public class MonoBehaviourSingletonUI<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (ReferenceEquals(instance, null))
                {
                    instance = FindObjectOfType<T>();
                    if (ReferenceEquals(instance, null))
                    {
#if lLcroweLogSystem
                        lLCroweTool.LogSystem.LogManager.Register("MonoBehaviourSingletonUIView", "MonoBehaviourSingletonUIView", true, true);
                        lLCroweTool.LogSystem.LogManager.Log("MonoBehaviourSingletonUIView", "UI싱글턴을 사용하는 친구들은 미리세팅되어야합니다.", null, lLCroweTool.LogSystem.LogManager.LogType.Error);
#endif
                        Debug.LogError("MonoBehaviourSingletonUIView_UI싱글턴을 사용하는 친구들은 미리세팅되어야합니다.");
                    }
                }
                return instance;
            }
        }

        protected virtual void Awake()
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
            OffUIView();
        }

        /// <summary>
        /// UI 뷰 숨기기
        /// </summary>
        public virtual void ShowUIView()
        {
            if (!gameObject.activeSelf) 
            {
                this.SetActive(true);
            }
        }

        /// <summary>
        /// UI 뷰 숨기기
        /// </summary>
        public virtual void OffUIView()
        {
            if (gameObject.activeSelf)
            {
                this.SetActive(false);
            }
        }

        protected virtual void OnDestroy()
        {
            instance = null;
        }
    }
}