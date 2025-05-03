using lLCroweTool;
using UnityEngine;


public class Spring : MonoBehaviour
{
    [System.Serializable]
    public class SpringTarget 
    {
        public Transform moveTargetTr;
        public Transform anchorPoint;
        public Vector3 offSet;
        public Vector3 velocity = Vector3.zero;
        public float weight = 1;
    }

    public SpringTarget target;

    public SpringTarget[] targetArray = new SpringTarget[0];

    public float maxDistance = 5.0f;    //최대거리
    public float maintainDistance = 0f;
    public float maxSpeed = 50f;        
    public float stiffness = 10.0f;   // 스프링의 강성 상수
    public float damping = 0.5f;      // 댐퍼 상수
    public float mass = 1.0f;         // 물체의 질량
    public float airDrag = 1.0f;


    // Update is called once per frame
    void Update()
    {
        var deltaTime = Time.deltaTime;

        foreach (var item in targetArray)
        {
            var targetTr = item.moveTargetTr;
            var anchonPoint = item.anchorPoint;

            var pos = targetTr.position;
            var newPos = lLcroweUtil.SimulateSpringForce(stiffness, damping, pos, anchonPoint.position + item.offSet, ref item.velocity, maintainDistance, deltaTime, item.weight, airDrag) + (mass * Physics.gravity * deltaTime);
           
            targetTr.position = newPos;
        }
    }
}