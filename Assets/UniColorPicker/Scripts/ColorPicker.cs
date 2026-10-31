using UnityEngine;
using UnityEngine.UI;

namespace UniColorPicker
{
    public class ColorPicker : MonoBehaviour
    {
        private static readonly int Hue = Shader.PropertyToID("_Hue");

        [SerializeField] private TapPositionDetector _hueTapDetector= null;
        [SerializeField] private Cursor _hueCursor = null;
        [SerializeField] private TapPositionDetector _valueSaturationTapDetector = null;
        [SerializeField] private Cursor _valueSaturationCursor = null;
        [SerializeField] private Image _valueSaturationSelectorImage = null;
        [SerializeField] private Image _outputImage = null;

        private float _hue;
        private float _value;
        private float _saturation;

        public Color Color
        {
            get => Color.HSVToRGB(_hue, _saturation, _value);
            set
            {
                Color.RGBToHSV(value, out _hue, out _value, out _saturation);
                _hueCursor.UpdatePosition(new Vector2(0.5f, _hue));
                _valueSaturationCursor.UpdatePosition(new Vector2(_saturation, _value));
                UpdateValueSaturationImage();
                UpdateOutputImage();
            }
        }

        void Awake()
        {
            _hueTapDetector.SetOnValueChanged(OnHueChange);
            _valueSaturationTapDetector.SetOnValueChanged(OnValueSaturationChange);
        }

        void OnHueChange(Vector2 position)
        {
            _hue = position.y;
            _hueCursor.UpdatePosition(new Vector2(0.5f, _hue));
            UpdateValueSaturationImage();
            UpdateOutputImage();
        }

        void OnValueSaturationChange(Vector2 position)
        {
            _value = position.y;
            _saturation = position.x;
            _valueSaturationCursor.UpdatePosition(position);
            UpdateOutputImage();
        }

        void UpdateOutputImage()
        {
            _outputImage.color = Color.HSVToRGB(_hue, _saturation, _value);
        }

        void UpdateValueSaturationImage()
        {
            _valueSaturationSelectorImage.material.SetFloat(Hue, _hue);
        }

        private void OnDestroy()
        {
            _valueSaturationSelectorImage.material.SetFloat(Hue, 0);
        }
    }
}
