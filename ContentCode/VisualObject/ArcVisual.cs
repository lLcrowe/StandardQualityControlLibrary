using System.Collections;
using UnityEngine;
using lLCroweTool.ObjectPool;
using System.Collections.Generic;

namespace lLCroweTool.Visual.Arc
{
    public class ArcVisual : MonoBehaviour
    {
        //궤적을 보여주는 비쥬얼 오브젝트

        public int count = 10;//보여줄 궤적량
        public float distance = 1f;//궤적량마다의 거리
        public Vector2 dir;
        public Vector3 gravity;
        public float power;
        public bool isWorld;

        public List<SpriteRenderer> srList = new List<SpriteRenderer>();

       


        private void Update()
        {
            for (int i = 0; i < srList.Count; i++)
            {
                int index = i;
                Vector2 pos = lLcroweUtil.GetArcPoint(transform, dir, gravity, power, index * distance, isWorld);
                srList[i].transform.position = pos;
            }
        }

        //[ButtonMethod]
        //public void ShowArcVisual()
        //{
        //    for (int i = 0; i < count; i++)
        //    {
        //        int index = i;
        //        Vector2 pos = lLcroweUtil.GetArcPoint(transform, dir, power, index * distance , isWorld);
        //        //월드위치 + 궤적
        //        SpriteRenderer sr = ObjectPoolManager.Instance.RequestDynamicComponentObject();
        //        srList.Add(sr);
        //        sr.gameObject.SetActive(true);
        //        sr.transform.position = pos;
        //    }
        //}
    }
}
