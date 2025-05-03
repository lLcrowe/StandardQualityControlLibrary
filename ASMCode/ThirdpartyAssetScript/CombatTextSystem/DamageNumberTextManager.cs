#if DamageText
using UnityEngine;
using lLCroweTool.Singleton;
using DamageNumbersPro;

namespace lLCroweTool.CombatTextSystem
{
    public class DamageNumberTextManager : MonoBehaviourSingleton<DamageNumberTextManager>
    {
        [Header("DamageNumbersPro")]
        public DamageNumber normalCombatTextPrefab;
        public DamageNumber alertCombatTextPrefab;
        public DamageNumber damageCombatTextPrefab;
        public DamageNumber repairCombatTextPrefab;
        public DamageNumber healCombatTextPrefab;

        [Space]
        //랜덤한 텍스트 위치를 해주고 싶을떄 쓰는 변수들
        public bool isRandomDirection = false;//랜덤적인 이동방향을 가질것인가
        public Vector2 minRandomVecter2 = new Vector2(-1, 1);//최소 방향
        public Vector2 maxRandomVecter2 = new Vector2(1, 1);//최대방향

        [Space]
        public float normalTextSize = 1f;
        public float critTextSize = 1.2f;

        protected override void Init()
        {
            alertCombatTextPrefab.PrewarmPool();
            damageCombatTextPrefab.PrewarmPool();
            healCombatTextPrefab.PrewarmPool();
            normalCombatTextPrefab.PrewarmPool();
            repairCombatTextPrefab.PrewarmPool();
        }

        /// <summary>
        /// 컴뱃텍스트보여주기
        /// </summary>
        /// <param name="targetTr">팔로우할 위치</param>
        /// <param name="text">텍스트</param>
        /// <param name="combatTextCategory">컴뱃텍스트 카테고리</param>
        /// <param name="textType">텍스트크기 타입</param>
        public void ShowCombatText(Transform targetTr, string text, CombatTextCategory combatTextCategory, CombatTextType textType)
        {
            Vector3 offSet = CheckRandomTextOffSet();
            DamageNumber targetObject = null;

            switch (combatTextCategory)
            {
                case CombatTextCategory.Alert:                    
                    targetObject = alertCombatTextPrefab.Spawn(targetTr.position + offSet, text, targetTr);
                    targetObject.SetColor(Color.red);
                    break;
                case CombatTextCategory.Damage:
                    targetObject = damageCombatTextPrefab.Spawn(targetTr.position + offSet, text, targetTr);
                    targetObject.SetColor(Color.yellow);
                    break;
                case CombatTextCategory.Heal:
                    targetObject = healCombatTextPrefab.Spawn(targetTr.position + offSet, text, targetTr);
                    targetObject.SetColor(Color.green);
                    break;
                case CombatTextCategory.Normal:
                    targetObject = normalCombatTextPrefab.Spawn(targetTr.position + offSet, text, targetTr);
                    targetObject.SetColor(Color.white);
                    break;
                case CombatTextCategory.Repair:
                    targetObject = repairCombatTextPrefab.Spawn(targetTr.position + offSet, text, targetTr);
                    targetObject.SetColor(Color.green);
                    break;
            }
            targetObject.SetScale(GetTextSizeType(textType));
        }

        /// <summary>
        /// 컴뱃텍스트보여주기
        /// </summary>
        /// <param name="targetTr">팔로우할 위치</param>
        /// <param name="number">넘버</param>
        /// <param name="combatTextCategory">컴뱃텍스트 카테고리</param>
        /// <param name="textType">텍스트크기 타입</param>
        public void ShowCombatText(Transform targetTr, float number, CombatTextCategory combatTextCategory, CombatTextType textType)
        {
            Vector3 offSet = CheckRandomTextOffSet();
            DamageNumber targetObject = null;

            switch (combatTextCategory)
            {
                case CombatTextCategory.Alert:
                    targetObject = alertCombatTextPrefab.Spawn(targetTr.position + offSet, number, targetTr);
                    targetObject.SetColor(Color.red);
                    break;
                case CombatTextCategory.Damage:
                    targetObject = damageCombatTextPrefab.Spawn(targetTr.position + offSet, number, targetTr);
                    targetObject.SetColor(Color.yellow);
                    break;
                case CombatTextCategory.Heal:
                    targetObject = healCombatTextPrefab.Spawn(targetTr.position + offSet, number, targetTr);
                    targetObject.SetColor(Color.green);
                    break;
                case CombatTextCategory.Normal:
                    targetObject = normalCombatTextPrefab.Spawn(targetTr.position + offSet, number, targetTr);
                    targetObject.SetColor(Color.white);
                    break;
                case CombatTextCategory.Repair:
                    targetObject = repairCombatTextPrefab.Spawn(targetTr.position + offSet, number, targetTr);
                    targetObject.SetColor(Color.green);
                    break;
            }
            targetObject.SetScale(GetTextSizeType(textType));
        }

        //컴뱃텍스쳐의 타입을 반환해줌
        private float GetTextSizeType(CombatTextType combatTextType)
        {
            float size;
            switch (combatTextType)
            {
                case CombatTextType.normal:
                    size = normalTextSize;
                    break;
                case CombatTextType.Crit:
                    size = critTextSize;
                    break;
                default:
                    size = normalTextSize;
                    break;
            }
            return size;
        }

        //텍스트의 랜덤방향을 생성해주는 함수
        private Vector2 CheckRandomTextOffSet()
        {
            Vector2 offSet = Vector2.zero;
            if (isRandomDirection)
            {
                offSet.x = Random.Range(minRandomVecter2.x, maxRandomVecter2.x);
                offSet.y = Random.Range(minRandomVecter2.y, maxRandomVecter2.y);
            }
            return offSet;
        }
    }
}
#endif