#if Doozy
using lLCroweTool.DialogueSystem;
using TMPro;
using UnityEngine;

namespace lLCroweTool.QuestSystem
{
    [CreateAssetMenu(fileName = "New QuestBookDBData", menuName = "lLcroweTool/QuestBookDBData")]
    public class QuestDBObjectScript : DBObjectScriptBase
    {
        //퀘스트 데이터베이스
        //퀘스트들이 들어가 있는 배열
        public DialogueDBObjectScript dialogueDBData;//참고할 대화DB데이터
        public QuestBookData[] questBookDataArray = new QuestBookData[0];
        public TMP_FontAsset defaultFontAsset = null;//퀘스트에 사용할 기본폰트
    }
}
#endif