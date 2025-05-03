using UnityEngine;

namespace lLCroweTool.RaderSystem
{
    public class RaderPingTarget : MonoBehaviour
    {
        [SerializeField] private int pingTag;

        public int GetPingTag()
        {
            return pingTag;
        }
    }
}