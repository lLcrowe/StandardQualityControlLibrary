#if UNITY_EDITOR && Doozy
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;
#pragma warning disable 0618
namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(PortraitUICard), true)]
    public class PortraitUICardInspectorEditor : Editor
    {
        private PortraitUICard targetPortraitUICard;

        private void OnEnable()
        {
            targetPortraitUICard = (PortraitUICard)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //초상화세팅 체크
            if (targetPortraitUICard.targetAnimObject != null)           
            {
                if (!targetPortraitUICard.targetAnimObject.TryGetComponent(out PortraitUICard portraitUICard))
                {
                    EditorGUILayout.HelpBox("초상화 오브젝트가 필요합니다.", MessageType.Warning);
                }
            }
           
            bool isDone = true;

            //UI체크
            if (targetPortraitUICard.portraitNameText == null)
            {
                isDone = false;
            }
            if (targetPortraitUICard.portraitNameBackGroundImageObject == null)
            {
                isDone = false;
            }
            if (targetPortraitUICard.borderImageObject == null)
            {
                isDone = false;
            }
            if (targetPortraitUICard.targetAnimObject == null)
            {
                isDone = false;
            }
            if (targetPortraitUICard.backGroundImageObject == null)
            {
                isDone = false;
            }

            if (!isDone)
            {
                //경고
                EditorGUILayout.HelpBox("세팅이 필요합니다.", MessageType.Warning);

                if (GUILayout.Button("세팅하기"))
                {
                    GameObject go;                   
                    if (targetPortraitUICard.portraitNameBackGroundImageObject == null)
                    {
                        go = new GameObject();
                        targetPortraitUICard.portraitNameBackGroundImageObject = go.AddComponent<Image>();
                        go.transform.parent = targetPortraitUICard.transform;
                        go.transform.position = targetPortraitUICard.transform.position;
                        go.name = "초상화이름뒷배경 이미지";
                    }
                    if (targetPortraitUICard.portraitNameText == null)
                    {
                        go = new GameObject();
                        targetPortraitUICard.portraitNameText = go.AddComponent<TextMeshProUGUI>();
                        go.transform.parent = targetPortraitUICard.portraitNameBackGroundImageObject.transform;
                        go.transform.position = targetPortraitUICard.transform.position;
                        go.name = "초상화이름 텍스트";
                    }
                    if (targetPortraitUICard.borderImageObject == null)
                    {
                        go = new GameObject();
                        targetPortraitUICard.borderImageObject = go.AddComponent<Image>();
                        go.transform.parent = targetPortraitUICard.transform;
                        go.transform.position = targetPortraitUICard.transform.position;
                        go.name = "초상화테두리 이미지";
                    }
                    if (targetPortraitUICard.backGroundImageObject == null)
                    {
                        go = new GameObject();
                        targetPortraitUICard.backGroundImageObject = go.AddComponent<Image>();
                        targetPortraitUICard.backGroundImageObject.raycastTarget = false;
                        targetPortraitUICard.backGroundImageObject.gameObject.AddComponent<Mask>().showMaskGraphic = false;
                        go.transform.parent = targetPortraitUICard.transform;
                        go.transform.position = targetPortraitUICard.transform.position;
                        go.name = "초상화뒷배경 이미지(Mask)";
                    }
                    if (targetPortraitUICard.targetAnimObject == null)
                    {
                        go = new GameObject();
                        targetPortraitUICard.targetAnimObject = go;
                        go.transform.parent = targetPortraitUICard.backGroundImageObject.transform;
                        go.transform.position = targetPortraitUICard.transform.position;
                        go.name = "초상화 오브젝트 위치";
                    }
                    
                }
            }


        }
    }
}
#endif