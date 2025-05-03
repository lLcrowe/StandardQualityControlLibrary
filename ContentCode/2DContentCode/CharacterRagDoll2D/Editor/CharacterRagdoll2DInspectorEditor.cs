using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using lLCroweTool.Ragdoll2DSystem;
#if UNITY_EDITOR
using UnityEditor;
#pragma warning disable 0618
namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(CharacterRagdoll2D))]
    public class CharacterRagdoll2DInspectorEditor : Editor
    {

        private CharacterRagdoll2D targetRagDoll;
    
        private List<Transform> transformList = new List<Transform>();
        private List<AnchoredJoint2D> anchoredJoint2DList = new List<AnchoredJoint2D>();
        private List<Collider2D> collider2DList = new List<Collider2D>();
        private List<Rigidbody2D> rigidbody2DList = new List<Rigidbody2D>();        

        private LayerMask targetLayer;//세팅할 레이어

        //카운트체크하는 캐싱구역
        private Collider2DType collider2DType;
        private int trCount = 0;//총 게임 오브젝트 수랑 동일해야함
        private int colliderCount = 0;//총 게임오브젝트 수랑 동일해야함
        private int rb2dCount = 0;//총 게임오브젝트 수랑 동일해야함
        private int jointCount = 0;//조인트는 총 게임오브젝트 수보다 1적어야됨
        private Vector3 snap = Vector3.one * 0.5f;
        private Vector3 offSet = Vector2.zero;
        private bool isCheck = false;
        private bool isCanRotate = false;
        private bool isCanMove = false;
        private bool isCanAnchorMove = false;
        private bool ragOn = false;

        private void OnEnable()
        {
            targetRagDoll = (CharacterRagdoll2D)target;
            //hingeJoint2Ds = targetRagDoll.hingeJoint2Ds;
            //collider2Ds = targetRagDoll.collider2Ds;
            //rb2ds = targetRagDoll.rb2ds;
            transformList.Clear();
            Transform[] transforms = targetRagDoll.transform.GetComponentsInChildren<Transform>();
            trCount = transforms.Length;
            transformList.Clear();
            anchoredJoint2DList.Clear();
            collider2DList.Clear();
            rigidbody2DList.Clear();
            transformList = transforms.ToList();
            colliderCount = 0;
            rb2dCount = 0;
            jointCount = 0;

            for (int i = 0; i < transformList.Count; i++)
            {
                if (transformList[i].TryGetComponent(out AnchoredJoint2D anchoredJoint2D))
                {
                    ++jointCount;
                    anchoredJoint2DList.Add(anchoredJoint2D);
                }
                if (transformList[i].TryGetComponent(out Collider2D collider2D))
                {
                    ++colliderCount;
                    collider2DList.Add(collider2D);
                }
                if (transformList[i].TryGetComponent(out Rigidbody2D rigidbody))
                {
                    ++rb2dCount;
                    rigidbody2DList.Add(rigidbody);
                }
            }
            isCheck = false;
            isCanRotate = false;
            isCanMove = false;
            ragOn = false;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.HelpBox("위치 수정상태 : " + isCanMove + "회전 수정상태 : " + isCanRotate + "앵커위치수정하기" + isCanAnchorMove, MessageType.Info);
            if (GUILayout.Button("위치수정하기"))
            {
                isCanMove = !isCanMove;
                SceneView.RepaintAll();
            }
            if (GUILayout.Button("회전 수정하기"))
            {
                isCanRotate = !isCanRotate;
                SceneView.RepaintAll();
            }
            if (GUILayout.Button("앵커위치수정하기"))
            {
                isCanAnchorMove = !isCanAnchorMove;
                SceneView.RepaintAll();
            }


            if (GUILayout.Button("새로고침"))
            {
                //Transform[] transforms = targetRagDoll.transform.GetComponentsInChildren<Transform>();
                //trCount = transforms.Length;
                //transformList.Clear();
                //transformList = transforms.ToList();

                //초기화
                colliderCount = 0;
                rb2dCount = 0;
                jointCount = 0;
                anchoredJoint2DList.Clear();
                collider2DList.Clear();
                rigidbody2DList.Clear();

                for (int i = 0; i < transformList.Count; i++)
                {
                    if (transformList[i].TryGetComponent(out AnchoredJoint2D anchoredJoint2D))
                    {
                        ++jointCount;
                        anchoredJoint2DList.Add(anchoredJoint2D);
                    }
                    if (transformList[i].TryGetComponent(out Collider2D collider2D))
                    {
                        ++colliderCount;
                        collider2DList.Add(collider2D);
                    }
                    if (transformList[i].TryGetComponent(out Rigidbody2D rigidbody))
                    {
                        ++rb2dCount;
                        rigidbody2DList.Add(rigidbody);
                    }
                }
                isCheck = true;
            }      

            //세팅 체크
           
            bool isDone = true;
            bool isDoneCollider = true;
            bool isDoneRb2d = true;
            bool isDoneJoint = true;

            //충돌체 수 체크
            if (colliderCount != trCount)
            {
                isDone = false;
                isDoneCollider = false;
            }

            //강체 수 체크
            if (rb2dCount != trCount)
            {
                isDone = false;
                isDoneRb2d = false;
            }

            //조인트 수 체크
            if (jointCount != trCount - 1)
            {
                isDone = false;
                isDoneJoint = false;
            }

            EditorGUILayout.HelpBox("레그돌 자식과 부모 수" + trCount.ToString() + "\n" +
                "충돌체 정상여부 : " + isDoneCollider + "\n" + "강체 정상여부 : " + isDoneRb2d + "\n" + "조인트 정상여부" + isDoneJoint
                , MessageType.Info);

            //정상 확인
            if (!isDone)
            {
                if (!isDoneCollider)
                {
                    collider2DType = (Collider2DType)EditorGUILayout.EnumPopup("추가할 콜라이더", collider2DType);

                    if (GUILayout.Button(collider2DType.ToString() + " 콜라이더 추가"))
                    {
                        for (int i = 0; i < transformList.Count; i++)
                        {
                            if (!transformList[i].TryGetComponent(out Collider2D collider2D))
                            {
                                switch (collider2DType)
                                {
                                    case Collider2DType.Box:
                                        transformList[i].gameObject.AddComponent<BoxCollider2D>();
                                        break;
                                    case Collider2DType.Capsule:
                                        transformList[i].gameObject.AddComponent<CapsuleCollider2D>();
                                        break;
                                    case Collider2DType.Circle:
                                        transformList[i].gameObject.AddComponent<CircleCollider2D>();
                                        break;
                                }
                            }
                        }
                    }
                }

                if (!isDoneRb2d)
                {
                    if (GUILayout.Button("강체 추가"))
                    {
                        for (int i = 0; i < transformList.Count; i++)
                        {
                            if (!transformList[i].TryGetComponent(out Rigidbody2D rigidbody2D))
                            {
                                transformList[i].gameObject.AddComponent<Rigidbody2D>();
                            }
                        }
                    }
                }

                if (!isDoneJoint)
                {
                    if (GUILayout.Button("힌지조인트 추가"))
                    {
                        for (int i = 0; i < transformList.Count; i++)
                        {
                            if (transformList[i] != targetRagDoll.transform)
                            {
                                if (!transformList[i].TryGetComponent(out HingeJoint2D hingeJoint2D))
                                {
                                    hingeJoint2D = transformList[i].gameObject.AddComponent<HingeJoint2D>();

                                    //힌지가 자동으로 연결
                                    //부모한테 연결
                                    //래그돌오브잭트 깊이 0

                                    if (hingeJoint2D.transform.parent == targetRagDoll.transform)
                                    {
                                        //부모가 래그돌본체이면
                                        //현재 오브젝트가 -1 깊이
                                        if (targetRagDoll.TryGetComponent(out Rigidbody2D rigidbody2D))
                                        {
                                            hingeJoint2D.connectedBody = rigidbody2D;
                                        }                                       
                                    }
                                    else
                                    {
                                        //부모가 래그돌 본체가 아니고 다른것이면
                                        //현재오브젝트가 -2깊이 이하
                                        if (hingeJoint2D.transform.parent.TryGetComponent(out Rigidbody2D rigidbody2D))
                                        {
                                            hingeJoint2D.connectedBody = rigidbody2D;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (isDone && isCheck)
            {
             
                if (GUILayout.Button("해당 래그돌에 세팅 등록하기"))
                {   
                    targetRagDoll.anchoredJoint2Ds = anchoredJoint2DList.ToArray();
                    targetRagDoll.collider2Ds = collider2DList.ToArray();
                    targetRagDoll.rb2ds = rigidbody2DList.ToArray();
                }

                if (GUILayout.Button("레그돌 싱테" + ragOn + "세팅"))
                {
                    if (ragOn)
                    {
                        targetRagDoll.RagDollOn();
                    }
                    else
                    {
                        targetRagDoll.RagDollOff();
                    }
                    ragOn = !ragOn;
                }
                
                /// <summary>
                /// 조인트에 연결되있는 강체끼리는 콜라이더충돌이 작동안됨
                /// 그렇다하면 조인트후 다음 조인트의 리지드바디는 상호작용 레이어가 달라야됨
                /// 1번과 2번 연결 하면 두개는 따로 충돌을 안함
                /// 2번과 3번을 연결하면 두개는 따로 충돌을 안함
                /// 1번과 3번은 충돌을 함 하지만 충돌하기 싫으면
                /// 해당 번호에 있는 게임오브젝트의 레이어를 변경시킴
                /// 월드레이어 친구한테만 작동시킬것
                /// </summary>       
                //래그돌 레이러를 제작후 해당 레이어에서 상호작용할 레이어들만 체크후 작업하면 따로 세팅해줄 이유가 사라짐


                //레그돌 레이어 설정
                targetLayer = EditorGUILayout.LayerField("세팅해줄 래그돌레이어", targetLayer);
                if (GUILayout.Button("래그돌 레이어 재설정"))
                {
                    //로직제작
                    for (int i = 0; i < transformList.Count; i++)
                    {
                        transformList[i].gameObject.layer = targetLayer;
                    }
                }


                string st = "-=오브젝트 이름=-\n";
                for (int i = 0; i < transformList.Count; i++)
                {
                    st += transformList[i].name + " (" + LayerMask.LayerToName(transformList[i].gameObject.layer) + ")\n";
                }
                EditorGUILayout.HelpBox(st, MessageType.Info);
            }
        }

        private void OnSceneGUI()
        {
            //씬상에 보여줄것
            //힌지들의 위치 + 리지드바디2D 무게등 시각적으로 볼수 있게

            //기즈모는 오직 두가지 유니티 이벤트함수에서 작동됨
            //핸들로만 작성
            float size = HandleUtility.GetHandleSize(targetRagDoll.transform.position) * 0.5f;
            Handles.color = Color.gray;   
            Vector2 pos = Handles.FreeMoveHandle(targetRagDoll.transform.position, Quaternion.identity, size, snap, Handles.CircleHandleCap);
            targetRagDoll.transform.position = pos;
            offSet = Vector3.zero;
            //조인트
            for (int i = 0; i < anchoredJoint2DList.Count; i++)
            {
                //앵커표시관련해서는 다른에셋이 대신함
                //여기서는 위치회전 변경관련과 어태치먼트 존재여부 체크

                if (anchoredJoint2DList[i].connectedBody == null)
                {
                    Handles.Label(anchoredJoint2DList[i].transform.position + offSet, "연결된 강체없음");
                }
                else
                {
                    Transform jointTr = anchoredJoint2DList[i].transform;

                  

                    Handles.Label(jointTr.position + offSet, "연결된 강체있음.");
                    offSet += new Vector3(0, size * 0.3f, 0);
                    Handles.Label(jointTr.position + offSet, anchoredJoint2DList[i].connectedBody.name);



                    //앵커위치를 제대로 보여줄수 있게
                    //힌지의 위치를 체크해줘야함
                    //앵커위치변경
                    //autoconfigureconnectedanchor //연결된 앵커 자동 구성을 켜놓고 작업하기

                   

                    if (isCanAnchorMove)
                    {
                        //이게맞음
                        pos = Handles.DoPositionHandle(jointTr.position + jointTr.TransformDirection(anchoredJoint2DList[i].anchor), jointTr.rotation);
                        //Debug.Log("월드위치 : " + pos + ", 앵커위치 : " + anchoredJoint2DList[i].anchor+ ",tt" + jointTr.InverseTransformPoint(pos));
                        anchoredJoint2DList[i].anchor = jointTr.InverseTransformPoint(pos);
                        //anchoredJoint2DList[i].anchor = pos - jointTr.position + jointTr.TransformDirection(anchoredJoint2DList[i].anchor);
                    }

                    //Handles.color = Color.gray;
                    Handles.DrawWireDisc(jointTr.position + jointTr.TransformDirection(anchoredJoint2DList[i].anchor), Vector3.forward, 0.02f, 10f);
                    Handles.DrawDottedLine(jointTr.position + jointTr.TransformDirection(anchoredJoint2DList[i].anchor), anchoredJoint2DList[i].connectedBody.transform.position, 10f);



                    Vector3 anchorOffset = Vector3.zero;
                    Handles.Label(jointTr.position + jointTr.TransformDirection(anchoredJoint2DList[i].anchor) + anchorOffset, "앵커위치" );
                    anchorOffset += new Vector3(0, size * 0.3f, 0);
                    Handles.Label(jointTr.position + jointTr.TransformDirection(anchoredJoint2DList[i].anchor) + anchorOffset, "연결된 바디 :" + anchoredJoint2DList[i].connectedBody.name);
                }

                //위치변경
                if (isCanMove)
                {
                    pos = Handles.DoPositionHandle(anchoredJoint2DList[i].transform.position, Quaternion.identity);
                    anchoredJoint2DList[i].transform.position = pos;
                }

                //회전변경
                if (isCanRotate)
                {
                    Quaternion rotate = Handles.DoRotationHandle(anchoredJoint2DList[i].transform.rotation, anchoredJoint2DList[i].transform.position);
                    anchoredJoint2DList[i].transform.rotation = Quaternion.Euler(0, 0, rotate.eulerAngles.z);
                }
            }
            offSet += new Vector3(0, size * 0.3f, 0);

            //충돌체
            for (int i = 0; i < collider2DList.Count; i++)
            {
                //트리거 상태 표시
                bool isTrigger = collider2DList[i].isTrigger;
                if (isTrigger)
                {
                    Handles.Label(collider2DList[i].transform.position + offSet, "트리거 상태");
                }
                else
                {
                    Handles.Label(collider2DList[i].transform.position + offSet, "콜라이더 상태");
                }
            }
            offSet += new Vector3(0, size * 0.3f, 0);

            //강체
            for (int i = 0; i < rigidbody2DList.Count; i++)
            {
                //질량 체크
                Handles.Label(rigidbody2DList[i].transform.position + offSet, "질량 : " + rigidbody2DList[i].mass.ToString());                
            }
            offSet += new Vector3(0, size * 0.3f, 0);

            //트랜스폼
            for (int i = 0; i < transformList.Count; i++)
            {
                //레이어체크
                Handles.Label(transformList[i].position + offSet, "레이어 : " + LayerMask.LayerToName(transformList[i].gameObject.layer));
            }
        }

        private enum Collider2DType
        {
            Box,
            Capsule,
            Circle,
        }
    }
}
#endif