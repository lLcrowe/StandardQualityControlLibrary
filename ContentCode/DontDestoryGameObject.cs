using UnityEngine;

namespace lLCroweTool.QC.DontDestroy
{
    public class DontDestoryGameObject : MonoBehaviour
    {
        private void Awake()
        {
            lLcroweUtil.DontDestroyTargetObject(gameObject);
            Destroy(this);
        }
    }
}