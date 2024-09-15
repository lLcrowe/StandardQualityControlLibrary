using lLCroweTool;
using lLCroweTool.TimerSystem;
using UnityEngine;

namespace Assets.StandardQualityControlLibary.TestModule.Ecosystem
{
    public class EcoSystemSpawnArea : MonoBehaviour
    {   
        //생태계제작
        //렉트랑 범위로 랜덤 생성 구역 처리
        [Header("Random")]
        public bool isUseRandomRotate;//배치할시 랜덤회전
        public bool isFollowAreaRotate;//현재

        [Header("Circle")]
        public float radius;

        [Header("Rect")]
        public float width;
        public float height;

        public AreaType areaType;
        public int maxAmount;
        public int curAmount;
        public EcosystemSpawnObject spawnPrefab;
        public TimerModule_Element timer;

        public enum AreaType
        {
            Rect,
            Circle,
        }

        private void Awake()
        {
            timer.SetTimer(0.2f);
        }

        private void OnEnable()
        {
            EcoSystemSpawnAreaGroupManager.Instance.AddGroup(this);
        }

        private void OnDisable()
        {
            EcoSystemSpawnAreaGroupManager.Instance.RemoveGroup(this);
        }

        public void UpdateArea()
        {
            if (!timer.CheckTimer())
            {
                return;
            }

            if (curAmount >= maxAmount)
            {
                return;
            }

            var target = ObjectPoolManager.Instance.RequestDynamicComponentObject(spawnPrefab);
            target.Init(this);

            var pos = transform.position;
            var rot = (isUseRandomRotate ? Quaternion.AngleAxis(Random.Range(-180, 180), Vector3.forward) : isFollowAreaRotate ? transform.rotation : Quaternion.identity);
            switch (areaType)
            {
                case AreaType.Rect:
                    pos += new Vector3(Random.Range(-width, width), Random.Range(-height, height), 0);                    
                    break;
                case AreaType.Circle:
                    pos = (Vector3)lLcroweUtil.GetRandomCirclePosition(transform, radius);
                    break;
            }
            
            pos = lLcroweUtil.GetRotatePosForPivot(pos, transform.position, transform.rotation);
            target.transform.SetPositionAndRotation(pos, rot);
        }

        private void OnDrawGizmos()
        {
            var pos = transform.position;
            var rot = transform.rotation;
            Gizmos.color = Color.green;
            switch (areaType)
            {
                case AreaType.Rect:
                    var rotationMatrix = Matrix4x4.TRS(pos, rot, transform.lossyScale);
                    Gizmos.matrix = rotationMatrix;
                    Gizmos.DrawWireCube(Vector2.zero, new Vector3(width, height) * 2);
                    break;
                case AreaType.Circle:
                    Gizmos.DrawWireSphere(pos, radius);
                    break;
            }
        }
    }
}