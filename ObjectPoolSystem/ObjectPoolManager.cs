using UnityEngine;
using lLCroweTool.Singleton;
using lLCroweTool.PoolBible;

namespace lLCroweTool
{
    public class ObjectPoolManager : MonoBehaviourSingleton<ObjectPoolManager>
    {
        //Test결과//해쉬값으로 변경시키고 보는것보다 그냥 string으로 보는게 오히려빠르다
        //좀더 자세히 살펴보니 스트링을 헤쉬코드로 변경후 딕셔너리가 더빠름

        //그냥 쓰자
        //유니티 오브젝트용
        [System.Serializable]
        public class DynamicPoolBible: CustomPoolBible<Component>{}
        public DynamicPoolBible dynamicPoolBible = new DynamicPoolBible();

        protected override void Init()
        {
            dynamicPoolBible.Clear();
        }

        /// <summary>
        /// 오브젝트를 요청하는 함수
        /// </summary>
        /// <typeparam name="T">컴포넌트타입 상속</typeparam>
        /// <param name="target">타겟이 될 프리팹</param>
        /// <returns>오브젝트</returns>
        public T RequestDynamicComponentObject<T>(T target) where T : Component
        {   
            return dynamicPoolBible.RequestPrefab(target) as T;
        }

        public T RequestDynamicComponentObject<T>(T target, Vector3 pos, Quaternion rot, Transform parent = null) where T : Component
        {
            var prefab = dynamicPoolBible.RequestPrefab(target) as T;
            prefab.InitTrObjPrefab(pos, rot, parent);
            return prefab;
        }



        /// <summary>
        /// 오브젝트를 반환하는 함수
        /// </summary>
        /// <typeparam name="T">컴포넌트 타입</typeparam>
        /// <param name="target">프리팹오브젝트</param>
        public void ReturnDynamicComponentObject<T>(T target) where T : Component
        {
            dynamicPoolBible.ReturnPrefab(target);
        }
    }
}