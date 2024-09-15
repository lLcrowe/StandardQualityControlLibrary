using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace lLCroweTool.UI.ShaderCanvas
{
    public class ShaderUICanvas : MonoBehaviour
    {
        //임시
        public Canvas canvas;

        private void Awake()
        {
            //URP쉐이더그래프를 사용할때 
            //랜더모드에 문제가 있음
            //앰플리파이 그래프를 사용해볼까 생각중


            canvas = GetComponent<Canvas>();
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                var camera = canvas.worldCamera;
                canvas.planeDistance = camera.nearClipPlane;
            }
            
        }


    }
}
