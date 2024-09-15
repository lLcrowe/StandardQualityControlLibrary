using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace lLCroweTool.Visual.Nozzle
{
    public class CustomNozzle : MonoBehaviour
    {
        //커스텀 노즐
        //우주선에 쓰일 노즐을 커스텀마이징하는 클래스
        private Vector2 size;//원본사이즈
        private Transform tr;//현재 트랜스폼 //캐싱
        [Range(0.0f, 3.0f)]
        [SerializeField] private float nozzlePower;//노즐파워
        [Tooltip("아무리 낮아도 원본사이즈보다는 작거나 같아야됨")]
        [Range(0.0f, 3.0f)]
        [SerializeField] private float min = 0.2f;//아무리 낮아도 원본사이즈보다는 작거나 같아야됨
        [Range(0.0f, 3.0f)]
        [SerializeField] private float max = 0.1f;

        private void Awake()
        {
            StartSettingCustomNozzle();
        }

        //시작시 세팅하는 함수
        public void StartSettingCustomNozzle()
        {
            //캐싱
            size = transform.localScale;
            tr = transform;
        }

        private void FixedUpdate()
        {
            tr.localScale = Vector2.one * Random.Range(size.x - min, size.y + max) * nozzlePower;
            //targetNozzle.localScale = Vector3.one * Random.Range(1.0f - 0.2f, 1.0f + 0.1f) * 1;
        }

        //외부에서 사용하는 함수(업데이트같은 종류)
        //
        public void SetNozzlePower(float _targetValue)
        {
            if (_targetValue < 0)
            {
                _targetValue = Mathf.Abs(_targetValue);
            }
            nozzlePower = _targetValue;
        }

        public Transform GetTransform()
        {
            return tr;
        }
    }

}

