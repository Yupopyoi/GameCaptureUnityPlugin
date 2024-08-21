using System;
using UnityEngine;
using UnityEngine.UI;

namespace ResizableCapturedSource
{
    [RequireComponent(typeof(RawImage))]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(AspectRatioFitter))]
    public class Resize : MonoBehaviour
    {
        private RawImage _rawImage;
        private RectTransform _resizeHandle;

        private bool _isResizing = false;

        private Vector2 _previousMouthLocalPoint;
        private Vector2 _mouthLocalPoint;

        private AspectRatioFitter _aspectRatioFitter;

        private float _aspectRatio = 16.0f / 9.0f;

        [Tooltip("Mouse sensitivity. Recommended value is 0.85")]
        [SerializeField, Range(0.5f, 1.2f)] float _mouseSensitivity = 0.85f;
        [Tooltip("Specifies the range within which you can start resizing. Defined by distance from the edge.")]
        [SerializeField, Range(1.0f, 100.0f)] float _resizeMargin = 50.0f;

        void Start()
        {
            _rawImage = GetComponent<RawImage>();
            _resizeHandle = _rawImage.GetComponent<RectTransform>();
            _aspectRatioFitter = GetComponent<AspectRatioFitter>();

            _aspectRatioFitter.aspectRatio = _aspectRatio;
        }

        void Update()
        {
            // Start Resizing
            if (Input.GetMouseButtonDown(0))
            {
                var screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, Input.mousePosition);

                RectTransformUtility.ScreenPointToLocalPointInRectangle(_resizeHandle, screenPos, Camera.main, out _mouthLocalPoint);
               
                if (IsMouseNearEdge(_mouthLocalPoint))
                {
                    _previousMouthLocalPoint = _mouthLocalPoint;
                    _isResizing = true;
                }
            }

            // Stop Resizing
            if (Input.GetMouseButtonUp(0))
            {
                _isResizing = false;
            }

            // Continue Resizing
            if (_isResizing && Input.GetMouseButton(0))
            {
                var screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, Input.mousePosition);

                RectTransformUtility.ScreenPointToLocalPointInRectangle(_resizeHandle, screenPos, Camera.main, out _mouthLocalPoint);

                var rawImageWidth = _resizeHandle.rect.width;
                var rawImageHeight = _resizeHandle.rect.height;
                var rawImage_x = _resizeHandle.localPosition.x;
                var rawImage_y = _resizeHandle.localPosition.y;

                var mouthLocalPoint_x = _mouthLocalPoint.x;
                var mouthLocalPoint_y = _mouthLocalPoint.y;

                // Specify the corner to be fixed.
                bool isLeftSideFixed = true;
                bool isBottomSideFixed = true;
                if (mouthLocalPoint_x < rawImage_x)
                {
                    isLeftSideFixed = false;
                }
                if (mouthLocalPoint_y < rawImage_y)
                {
                    isBottomSideFixed = false;
                }

                // Obtain the coordinates of the corner to be fixed.
                Vector3 fixedCorners = new Vector3(rawImage_x, rawImage_y, 0);

                if(isLeftSideFixed){
                    fixedCorners.x -= rawImageWidth * 0.5f;
                }
                else{
                    fixedCorners.x += rawImageWidth * 0.5f;
                }

                if (isBottomSideFixed){
                    fixedCorners.y -= rawImageHeight * 0.5f;
                }
                else{
                    fixedCorners.y += rawImageHeight * 0.5f;
                }

                // Obtain the difference between the left and right mouse position.
                var differenceMouthPosition = Math.Abs(_mouthLocalPoint.x) - Math.Abs(_previousMouthLocalPoint.x);

                _resizeHandle.sizeDelta = new Vector2(
                    rawImageWidth + differenceMouthPosition * 2 * _mouseSensitivity,
                    rawImageHeight
                );

                var resizedWidth = _resizeHandle.rect.width;
                var resizedHeight = _resizeHandle.rect.height;
                var resized_x = fixedCorners.x;
                var resized_y = fixedCorners.y;

                if (isLeftSideFixed)
                {
                    resized_x += resizedWidth * 0.5f;
                }
                else
                {
                    resized_x -= resizedWidth * 0.5f;
                }

                if (isBottomSideFixed)
                {
                    resized_y += resizedHeight * 0.5f;
                }
                else
                {
                    resized_y -= resizedHeight * 0.5f;
                }

                Vector3 newCenterPos = new Vector3(resized_x, resized_y, 0);
                _resizeHandle.localPosition = newCenterPos;

                _previousMouthLocalPoint = _mouthLocalPoint;
            }
        }

        public void SetAspectRatio(int[] Resolution)
        {
            if (Resolution.Length != 2) return;
            if (Resolution[1] == 0) return;

            _aspectRatio = (float)Resolution[0] / Resolution[1];

            _aspectRatioFitter.aspectRatio = _aspectRatio;
        }

        private bool IsMouseNearEdge(Vector2 localMousePosition)
        {
            Rect rect = _resizeHandle.rect;

            // 左右または上下のエッジに近い場合は true を返す
            return Mathf.Abs(localMousePosition.x - rect.xMin) <= _resizeMargin ||
                   Mathf.Abs(localMousePosition.x - rect.xMax) <= _resizeMargin ||
                   Mathf.Abs(localMousePosition.y - rect.yMin) <= _resizeMargin ||
                   Mathf.Abs(localMousePosition.y - rect.yMax) <= _resizeMargin;
        }
    }
} // namespace ResizableCapturedSource
