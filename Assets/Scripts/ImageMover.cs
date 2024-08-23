using UnityEngine;
using UnityEngine.UI;

namespace ResizableCapturedSource
{
    [RequireComponent(typeof(RawImage))]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Resize))]
    [RequireComponent(typeof(VideoDisplayer))]
    public class ImageMover : MonoBehaviour
    {
        private RawImage _rawImage;
        private RectTransform _moveHandle;

        private bool _isMoving = false;

        private Vector2 _previousMouthLocalPoint;
        private Vector2 _mouthLocalPoint;

        private float _moveArea = 50.0f;

        private Resize _resize;
        private VideoDisplayer _videoDisplayer;

        void Start()
        {
            _rawImage = GetComponent<RawImage>();
            _moveHandle = _rawImage.GetComponent<RectTransform>();
            _resize = GetComponent<Resize>();
            _videoDisplayer = GetComponent<VideoDisplayer>();

            _moveArea = _resize.ResizeArea();
        }

        void Update()
        {
            if (!_resize.IsOperable) return;

            // Start Moving
            if (Input.GetMouseButtonDown(0))
            {
                var screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, Input.mousePosition);

                RectTransformUtility.ScreenPointToLocalPointInRectangle(_moveHandle, screenPos, Camera.main, out _mouthLocalPoint);

                if (IsMouseNearCenter(_mouthLocalPoint))
                {
                    _previousMouthLocalPoint = _mouthLocalPoint;
                    _isMoving = true;
                }
            }

            // Stop Moving
            if (Input.GetMouseButtonUp(0))
            {
                _isMoving = false;
                _previousMouthLocalPoint = _mouthLocalPoint;
            }

            // Continue Moving
            if (_isMoving && Input.GetMouseButton(0))
            {
                if (_videoDisplayer.IsFlipped())
                {
                    Vector3 scale = _rawImage.rectTransform.localScale;
                    scale.x *= -1;
                    _rawImage.rectTransform.localScale = scale;
                }

                var screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, Input.mousePosition);

                RectTransformUtility.ScreenPointToLocalPointInRectangle(_moveHandle, screenPos, Camera.main, out _mouthLocalPoint);

                // Obtain the current RawImage's location.
                var rawImage_x = _moveHandle.localPosition.x;
                var rawImage_y = _moveHandle.localPosition.y;

                // Obtain the difference mouse position.
                var differenceMouthPosition_x = _mouthLocalPoint.x - _previousMouthLocalPoint.x;
                var differenceMouthPosition_y = _mouthLocalPoint.y - _previousMouthLocalPoint.y;

                // Determine new location
                var newPosition_x = rawImage_x + differenceMouthPosition_x;
                var newPosition_y = rawImage_y + differenceMouthPosition_y;

                if (!_resize.CanStickOutScreen)
                {
                    float screenWidth = Screen.width;
                    float screenHeight = Screen.height;

                    var rawImageWidth = _moveHandle.rect.width;
                    var rawImageHeight = _moveHandle.rect.height;

                    if (newPosition_x - rawImageWidth * 0.5f <  - screenWidth * 0.5f)
                    {
                        newPosition_x = (rawImageWidth - screenWidth) * 0.5f;
                    }
                    else if (newPosition_x + rawImageWidth * 0.5f > screenWidth * 0.5f)
                    {
                        newPosition_x = (screenWidth - rawImageWidth) * 0.5f;
                    }

                    if (newPosition_y - rawImageHeight * 0.5f < - screenHeight * 0.5f)
                    {
                        newPosition_y = (rawImageHeight - screenHeight) * 0.5f;
                    }
                    else if (newPosition_y + rawImageHeight * 0.5f > screenHeight * 0.5f)
                    {
                        newPosition_y = (screenHeight - rawImageHeight) * 0.5f;
                    }
                }

                Vector3 newCenterPos = new(newPosition_x, newPosition_y, 0);
                _moveHandle.localPosition = newCenterPos;

                if (_videoDisplayer.IsFlipped())
                {
                    Vector3 scale = _rawImage.rectTransform.localScale;
                    scale.x *= -1;
                    _rawImage.rectTransform.localScale = scale;
                }
            }
        }

        private bool IsMouseNearCenter(Vector2 localMousePosition)
        {
            Rect rect = _moveHandle.rect;

            return localMousePosition.x > rect.xMin + _moveArea &&
                   localMousePosition.x < rect.xMax - _moveArea &&
                   localMousePosition.y > rect.yMin + _moveArea &&
                   localMousePosition.y < rect.yMax - _moveArea;
        }
    }
} // namespace ResizableCapturedSource
