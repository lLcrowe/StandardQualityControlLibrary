//using System.Collections;
//using UnityEngine;
//using UnityEditor;
//using lLCroweTool.AutoAnimation2D.GunRecoilAnim;
//using System.Collections.Generic;

//namespace lLCroweTool.QC.EditorOnly
//{
//    [CustomEditor(typeof(GunRecoilAnim))]
//    [CanEditMultipleObjects]
//    public class GunRecoilAnimInspectorEditor : Editor
//    {
//        private List<GunRecoilAnim> gunRecoilAnimList = new List<GunRecoilAnim>();
//        private static bool isOnlyThisParentChildFind = false;//뭐지?

//        private void OnEnable()
//        {
//            gunRecoilAnimList.Clear();
//            for (int i = 0; i < targets.Length; i++)
//            {
//                gunRecoilAnimList.Add(targets[i] as GunRecoilAnim);
//            }
//        }

//        public override void OnInspectorGUI()
//        {
//            base.OnInspectorGUI();

//            if (GUILayout.Button("리코일작동"))
//            {
//                for (int i = 0; i < gunRecoilAnimList.Count; i++)
//                {
//                    GunRecoilAnim gunRecoilAnim = gunRecoilAnimList[i];
//                    gunRecoilAnim.ActionRecoil();
//                }
                
//            }

//            for (int i = 0; i < gunRecoilAnimList.Count; i++)
//            {
//                GunRecoilAnim gunRecoilAnim = gunRecoilAnimList[i];

                


//                ActionFunc(gunRecoilAnim);
//            }
//        }

//        private static void ActionFunc(GunRecoilAnim gunRecoilAnim)
//        {
//            for (int i = 0; i < gunRecoilAnim.gunRecoilTargetArray.Length; i++)
//            {
//                GunRecoilAnim.GunRecoilTarget gunRecoilTarget = gunRecoilAnim.gunRecoilTargetArray[i];
//                //뭘해야되지?


//            }



//        }
//    }
//}