using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ResizableCapturedSource
{
    public class RightClickMenu : MonoBehaviour, IPointerClickHandler
    {
        private GameObject _menuPanel;
        private RectTransform _menuRectTransform;
        private SettingPanel _settingPanel;

        private float _panelWidth = 800;
        private float _panelHeight = 1000;
        private float _displayOffset = 0.8f;

        private float[] _panelPosition = new float[2];

        void Start()
        {
            _menuPanel = GameObject.Find("RightClickMenu");
            _menuRectTransform = _menuPanel.GetComponent<RectTransform>();
            _settingPanel = _menuPanel.GetComponent<SettingPanel>();

            _panelWidth = _menuRectTransform.rect.width;   
            _panelHeight = _menuRectTransform.rect.height;

            _settingPanel.TemporaryLock(false);
            _menuPanel.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    (RectTransform)transform,
                    eventData.position,
                    null,
                    out Vector2 localPosition);

                localPosition.x += _panelWidth * 0.5f * _displayOffset;
                localPosition.y -= _panelHeight * 0.5f * _displayOffset;

                _panelPosition[0] = localPosition.x;
                _panelPosition[1] = localPosition.y;

                _menuRectTransform.anchoredPosition = localPosition;
                _menuPanel.SetActive(true);

                _settingPanel.TemporaryLock(true);
                _settingPanel.Load();
            }
            else if(eventData.button == PointerEventData.InputButton.Left)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    (RectTransform)transform,
                    eventData.position,
                    null,
                    out Vector2 localPosition);

                bool hideFlag = false;
                if (localPosition.x > _panelPosition[0] + _panelWidth * 0.5f) hideFlag = true;
                else if (localPosition.x < _panelPosition[0] - _panelWidth * 0.5f) hideFlag = true;
                if (localPosition.y > _panelPosition[1] + _panelHeight * 0.5f) hideFlag = true;
                else if (localPosition.y < _panelPosition[1] - _panelHeight * 0.5f) hideFlag = true;

                if (!hideFlag) return;

                _settingPanel.TemporaryLock(false);
                _menuPanel.SetActive(false);
            }
            else
            {
                _settingPanel.TemporaryLock(false);
                _menuPanel.SetActive(false);
            }
        }
    }
} // namespace ResizableCapturedSource
