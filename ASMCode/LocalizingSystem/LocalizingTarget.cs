using UnityEngine;
using TMPro;

namespace lLCroweTool.Localize
{
    public class LocalizingTarget : MonoBehaviour
    {
        private TextMeshProUGUI text;
        [LocalizeGroup]public string string_ID;
        
        private void Awake()
        {
            text = GetComponent<TextMeshProUGUI>();
        }

        public void SetText(string _text)
        {
            text.text = _text;
        }

        public string GetStringID()
        {
            return string_ID; 
        }
    }
}