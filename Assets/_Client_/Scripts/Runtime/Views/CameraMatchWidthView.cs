using SFramework.Core.Runtime;
using UnityEngine;

namespace _Client_.Scripts.Runtime.Views
{
    [RequireComponent(typeof(Camera))]
    public class CameraMatchWidthView : SFView
    {
        [SerializeField]
        private float sceneWidth = 10;

        protected override void Init()
        {
            var _camera = GetComponent<Camera>();
            float unitsPerPixel = sceneWidth / Screen.width;
            float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;
            _camera.orthographicSize = desiredHalfHeight;
        }
    }
}