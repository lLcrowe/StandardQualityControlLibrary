#if Doozy
using lLCroweTool.QuestSystem;
using TMPro;
using UnityEngine;

namespace lLCroweTool.DialogueSystem
{
    [CreateAssetMenu(fileName = "New DialogueDBData", menuName = "lLcroweTool/DialogueDBData")]
    public class DialogueDBObjectScript : DBObjectScriptBase
    {
        //다이어로그 데이터베이스
        //대화들이 들어가 있는 데이터 배열들
        public QuestDBObjectScript questDBData;//참조할 퀘스트DB데이터
        public DialogueData[] dialogueDataArray = new DialogueData[0];
        public TMP_FontAsset defaultFontAsset = null;//대화에 사용할 기본폰트
    }
}
#endif