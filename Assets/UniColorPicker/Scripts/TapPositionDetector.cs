using System;
using UnityEngine;

namespace UniColorPicker
{
    public class TapPositionDetector : MonoBehaviour
    {
        private const float TapMargin = 0.1f;
            
        [SerializeField] private RectTransform _tapTarget = null;
        [SerializeField] private Canvas _canvas = null;
        
        private Camera _camera;
        private float _height;
        private float _width;
        private Action<Vector2> _onValueChanged;
        
        public void SetOnValueChanged(Action<Vector2> onValueChanged)
        {
            _onValueChanged = onValueChanged;
        }
        
        void Awake()
        {
            _camera = _canvas.worldCamera;
            var sizeDelta = _tapTarget.sizeDelta;
            _height = sizeDelta.y;
            _width = sizeDelta.x;
        }
        
        void Update()
        {
            if (!Input.GetMouseButton(0))
            {
                return;
            }
            
            var screenPoint = Input.mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_tapTarget, screenPoint, _camera,
                out var localPoint);

            var progressX = localPoint.x / _width + 0.5f;
            var progressY = localPoint.y / _height + 0.5f;

            if (progressX < -TapMargin || progressX > 1 + TapMargin || progressY < -TapMargin || progressY > 1 + TapMargin)
            {
                return;
            }

            progressX = Mathf.Clamp(progressX, 0, 1);
            progressY = Mathf.Clamp(progressY, 0, 1);
            
            _onValueChanged?.Invoke(new Vector2(progressX, progressY));
        }
    }
}
