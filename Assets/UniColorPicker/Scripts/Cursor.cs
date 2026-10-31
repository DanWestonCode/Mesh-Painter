using UnityEngine;

namespace UniColorPicker
{
    public class Cursor : MonoBehaviour
    {
        [SerializeField] private RectTransform _tapTargetTransform = null;
        
        private float _height;
        private float _width;
        
        void Awake()
        {
            var sizeDelta = _tapTargetTransform.sizeDelta;
            _height = sizeDelta.y;
            _width = sizeDelta.x;
        }

        public void UpdatePosition(Vector2 position)
        {
            var x = (position.x - 0.5f) * _width;
            var y = (position.y - 0.5f) * _height;
            transform.localPosition = new Vector3(x, y);
        }
    }
}
