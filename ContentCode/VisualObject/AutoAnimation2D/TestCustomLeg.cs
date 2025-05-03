#if Sensor && MEC
using UnityEngine;
using Micosmo.SensorToolkit;
using MEC;
using System.Collections.Generic;

namespace lLCroweTool.AutoAnimation2D
{
    public class TestCustomLeg : MonoBehaviour
    {
        public RaySensor2D[] raySensor2DArray;

        public Vector2[] currentPositionArray;
        public Vector2[] newPositionArray;
        public Vector2[] oldPositionArray;
        private bool[] legMoving;

        public Transform body;
    

        public float footSpacing;
        public float stepDistance;
        public float lerp;
        
        public float moveSpeed;
        public int smoothness = 8;
     
        public float stepSize = 0.15f;
        
        public float stepHeight = 0.15f;
        public bool bodyOrientation = true;

        private Vector2 velocity;
        private Vector2 lastVelocity;
        private Vector2 lastBodyPos;

        private float velocityMultiplier = 7f;

        // Use this for initialization
        void Awake()
        {
            body = transform;
            raySensor2DArray = GetComponentsInChildren<RaySensor2D>();
            currentPositionArray = new Vector2[raySensor2DArray.Length];


            oldPositionArray = new Vector2[raySensor2DArray.Length];
            newPositionArray = new Vector2[raySensor2DArray.Length];
            legMoving = new bool[raySensor2DArray.Length];

            for (int i = 0; i < oldPositionArray.Length; i++)
            {
                oldPositionArray[i] = raySensor2DArray[i].transform.position;
                newPositionArray[i] = raySensor2DArray[i].transform.position;
                legMoving[i] = false;
            }
            lastBodyPos = transform.position;
        }

        private void FixedUpdate()
        {
            lastBodyPos = (Vector2)body.position - lastBodyPos;
            velocity = (velocity + smoothness * lastVelocity) / (smoothness + 1f);

            if (velocity.magnitude < 0.000025f)
                velocity = lastVelocity;
            else
                lastVelocity = velocity;

            int nbLegs = raySensor2DArray.Length;
            Vector2[] desiredPositions = new Vector2[nbLegs];
            int indexToMove = -1;
            float maxDistance = stepSize;
            for (int i = 0; i < nbLegs; ++i)
            {
                desiredPositions[i] = transform.TransformPoint(newPositionArray[i]);

                float distance = lLcroweUtil.GetDistance(desiredPositions[i] + velocity * velocityMultiplier - oldPositionArray[i], (Vector2)transform.up);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    indexToMove = i;
                }
            }
            for (int i = 0; i < nbLegs; ++i)
                if (i != indexToMove)
                    raySensor2DArray[i].transform.position = oldPositionArray[i];


            //
            if (indexToMove != -1 && !legMoving[0])
            {
                //Vector2 targetPoint = desiredPositions[indexToMove] + Mathf.Clamp(velocity.magnitude * velocityMultiplier, 0.0f, 1.5f) * (desiredPositions[indexToMove] -
                //    (Vector2)raySensor2DArray[indexToMove].transform.position) + velocity * velocityMultiplier;

                //Vector2[] positionAndNormalFwd = MatchToSurfaceFromAbove(targetPoint + velocity * velocityMultiplier,
                //    ((Vector2)transform.parent.up - velocity * 10).normalized);

                //Vector2[] positionAndNormalBwd = MatchToSurfaceFromAbove(targetPoint + velocity * velocityMultiplier, ,
                //    ((Vector2)transform.parent.up + velocity * 10).normalized);

                //legMoving[0] = true;

                //if (positionAndNormalFwd[1] == Vector2.zero)
                //{
                //    Timing.RunCoroutine(PerformStep(indexToMove, positionAndNormalBwd[0]));
                //}
                //else
                //{
                //    Timing.RunCoroutine(PerformStep(indexToMove, positionAndNormalFwd[0]));
                //}
            }

            lastBodyPos = transform.position;
        }

        public void LegPos()
        {

        }


        //스탭 진행
        IEnumerator<float> PerformStep(int index, Vector3 targetPoint)
        {
            Vector3 startPos = oldPositionArray[index];
            for (int i = 1; i <= smoothness; ++i)
            {
                raySensor2DArray[index].transform.position = Vector3.Lerp(startPos, targetPoint, i / (float)(smoothness + 1f));
                raySensor2DArray[index].transform.position += transform.up * Mathf.Sin(i / (float)(smoothness + 1f) * Mathf.PI) * stepHeight;
                yield return Timing.WaitForSeconds(0.02f);
            }
            raySensor2DArray[index].transform.position = targetPoint;
            oldPositionArray[index] = raySensor2DArray[index].transform.position;
            legMoving[0] = false;
        }
    }
}
#endif