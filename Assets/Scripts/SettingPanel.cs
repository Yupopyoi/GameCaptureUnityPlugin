using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.ShaderKeywordFilter;
using System.Text.RegularExpressions;

namespace ResizableCapturedSource
{
    public class SettingPanel : MonoBehaviour
    {
        [Header("User Settings")]
        [Tooltip("Specify the font.This font will be applied to all text in the panel.")]
        [SerializeField] TMP_FontAsset _tmpFont;

        [Header("System Settings")]
        [SerializeField] Canvas _canvas;
        [SerializeField] GameObject _parentRawImage;
        [SerializeField] RectTransform _menuRectTransform;
        [SerializeField] GameObject _mainPanel;
        [SerializeField] TMP_Dropdown _videoDevicesDropdown;
        [SerializeField] TMP_Dropdown _audioDevicesDropdown;
        [SerializeField] TMP_Dropdown _samplingRateDropdown;
        [SerializeField] TMP_Dropdown _channelDropdown;
        [SerializeField] Toggle _lockToggle;
        [SerializeField] Toggle _stickOutToggle;
        [SerializeField] Toggle _flipToggle;
        [SerializeField] Toggle _muteToggle;
        [SerializeField] int _orderMax = 20;
        [SerializeField] Slider _orderSlider;
        [SerializeField] TextMeshProUGUI _orderValueText;
        [SerializeField] Slider _volumeSlider;
        [SerializeField] TextMeshProUGUI _volumeValueText;

        VideoDisplayer _videoDisplayer;

        private bool _isDropdownVaild = false;
        private bool _isToggleVaild = false;
        private bool _isSliderVaild = false;

        void Start()
        {
            _videoDisplayer = _parentRawImage.GetComponent<VideoDisplayer>();

            TextMeshProUGUI[] texts = _parentRawImage.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (var text in texts)
            {
                text.font = _tmpFont;
            }

            Transform template_video = _videoDevicesDropdown.transform.Find("Template");
            if (template_video != null)
            {
                Transform item = template_video.Find("Viewport/Content/Item");
                if (item != null)
                {
                    TMP_Text itemText = item.GetComponentInChildren<TMP_Text>();
                    if (itemText != null)
                    {
                        itemText.font = _tmpFont;
                    }
                }
            }

            Transform template_audio = _audioDevicesDropdown.transform.Find("Template");
            if (template_audio != null)
            {
                Transform item = template_audio.Find("Viewport/Content/Item");
                if (item != null)
                {
                    TMP_Text itemText = item.GetComponentInChildren<TMP_Text>();
                    if (itemText != null)
                    {
                        itemText.font = _tmpFont;
                    }
                }
            }

            Transform template_sample = _samplingRateDropdown.transform.Find("Template");
            if (template_sample != null)
            {
                Transform item = template_sample.Find("Viewport/Content/Item");
                if (item != null)
                {
                    TMP_Text itemText = item.GetComponentInChildren<TMP_Text>();
                    if (itemText != null)
                    {
                        itemText.font = _tmpFont;
                    }
                }
            }

            Transform template_channel = _channelDropdown.transform.Find("Template");
            if (template_channel != null)
            {
                Transform item = template_channel.Find("Viewport/Content/Item");
                if (item != null)
                {
                    TMP_Text itemText = item.GetComponentInChildren<TMP_Text>();
                    if (itemText != null)
                    {
                        itemText.font = _tmpFont;
                    }
                }
            }
        }

        private static string ExtractValidCharacters(string input)
        {
            string pattern = @"[0-9A-Za-z!@#\$%\^&\*\(\)\-_=\+\[\]\{\};:'"",<>\.\?\/\\|`~]";

            string result = string.Concat(Regex.Matches(input, pattern));

            return result;
        }

        public void Load()
        {
            _isDropdownVaild = false;

            var videoDevices = _videoDisplayer.VideoDeviceNames;
            var audioDevices = _videoDisplayer.AudioDeviceNames;

            var videoDevicesList = new List<string>();
            var audioDevicesList = new List<string>();

            videoDevicesList.Add("--Select Video Device--");
            audioDevicesList.Add("--Select Audio Device--");

            _videoDevicesDropdown.ClearOptions();
            _audioDevicesDropdown.ClearOptions();

            foreach(var device in videoDevices)
            {
                videoDevicesList.Add(ExtractValidCharacters(device));
            }
            foreach (var device in audioDevices)
            {
                audioDevicesList.Add(ExtractValidCharacters(device));
            }

            _videoDevicesDropdown.AddOptions(videoDevicesList);
            _audioDevicesDropdown.AddOptions(audioDevicesList);

            var currentVideoIndex = _videoDisplayer.VideoDeviceIndex();
            var currentAudioIndex = _videoDisplayer.AudioDeviceIndex();

            if (currentVideoIndex < videoDevices.Length)
            {
                _videoDevicesDropdown.value = currentVideoIndex + 1;
            }
            else
            {
                _videoDevicesDropdown.value = 0;
            }

            if (currentAudioIndex < audioDevices.Length)
            {
                _audioDevicesDropdown.value = currentAudioIndex + 1;
            }
            else
            {
                _audioDevicesDropdown.value = 0;
            }

            _videoDevicesDropdown.RefreshShownValue();
            _audioDevicesDropdown.RefreshShownValue();


            // ----- Audio Config -----

            VideoDisplayer.SampleRate samplingRate = _videoDisplayer.SamplingRate();
            if(samplingRate == VideoDisplayer.SampleRate.Rate_44100Hz)
            {
                _samplingRateDropdown.value = 0;
            }
            else
            {
                _samplingRateDropdown.value = 1;
            }

            VideoDisplayer.Channel channel = _videoDisplayer.AudioChannel();
            int channel_index = 0;
            if (channel == VideoDisplayer.Channel.Mono) channel_index = 0;
            else if (channel == VideoDisplayer.Channel.Stereo) channel_index = 1;
            else if (channel == VideoDisplayer.Channel.Quadraphonic) channel_index = 2;
            else if (channel == VideoDisplayer.Channel.Surround_5_1) channel_index = 3;
            else if (channel == VideoDisplayer.Channel.Surround_7_1) channel_index = 4;
            _channelDropdown.value = channel_index;

            _isDropdownVaild = true;

            // ----- Toggles -----

            _isToggleVaild = false;

            _lockToggle.isOn = _videoDisplayer.IsLocked;
            _stickOutToggle.isOn = _videoDisplayer.CanStickOut;
            _flipToggle.isOn = _videoDisplayer.IsFlipped();
            _muteToggle.isOn = _videoDisplayer.IsMute();

            var transform = _mainPanel.GetComponent<RectTransform>();
            Vector3 pos = _menuRectTransform.anchoredPosition;
            Vector3 scale = transform.localScale;

            if (scale.x > 0 && _flipToggle.isOn)
            {
                scale.x *= -1;
                pos.x *= -1;
            }
            else if (scale.x < 0 && !_flipToggle.isOn)
            {
                scale.x *= -1;
                pos.x *= -1;
            }
            transform.localScale = scale;
            _menuRectTransform.anchoredPosition = pos;

            _isToggleVaild = true;

            // ----- Sliders ------

            _isSliderVaild = false;

            int order = _canvas.sortingOrder;
            if (order < 0) order = 0;
            else if(order > _orderMax) order = _orderMax;
            _canvas.sortingOrder = order;

            _orderSlider.value = order;
            _orderValueText.text = order.ToString();

            int volume = _videoDisplayer.Volume;
            _volumeSlider.value = volume;
            _volumeValueText.text = volume.ToString();

            _isSliderVaild = true;
        }

        public void TemporaryLock(bool isLocked)
        {
            _videoDisplayer.IsTemporaryLocked = isLocked;
        }

        public void OnVideoDeviceChanged()
        {
            if (!_isDropdownVaild) return;

            int deviceIndex = _videoDevicesDropdown.value - 1;
            
            _videoDisplayer.VideoStop();

            if (deviceIndex == -1)
            {
                return;
            }

            _videoDisplayer.PlayUsingSelectedDevice(deviceIndex);
        }

        public void OnAudioDeviceChanged()
        {
            if (!_isDropdownVaild) return;

            int deviceIndex = _audioDevicesDropdown.value - 1;

            _videoDisplayer.AudioStop();

            if (deviceIndex == -1)
            {
                return;
            }

            _videoDisplayer.PlayMicrophone(deviceIndex);
        }

        public void OnSamplingRateChanged()
        {
            if (!_isDropdownVaild) return;

            int deviceIndex = _samplingRateDropdown.value;
            int samplingRate = 44100;
            if(deviceIndex == 1) samplingRate = 48000;
            else if(deviceIndex == 2) samplingRate = 96000;

            _videoDisplayer.AudioStop();

            _videoDisplayer.SetSamplingRate(samplingRate);

            if (_audioDevicesDropdown.value == 0) return;

            _videoDisplayer.PlayMicrophone(_audioDevicesDropdown.value - 1);
        }

        public void OnChannelChanged()
        {
            if (!_isDropdownVaild) return;

            int deviceIndex = _samplingRateDropdown.value;

            _videoDisplayer.AudioStop();

            _videoDisplayer.SetChannel(deviceIndex);

            _videoDisplayer.PlayMicrophone(_audioDevicesDropdown.value - 1);
        }

        public void OnLockToggleClicked()
        {
            if (!_isToggleVaild) return;

            _videoDisplayer.IsLocked = _lockToggle.isOn;
        }

        public void OnCanStickOutToggleClicked()
        {
            if (!_isToggleVaild) return;

            _videoDisplayer.CanStickOut = _stickOutToggle.isOn;
        }

        public void OnFlipToggleClicked()
        {
            if (!_isToggleVaild) return;

            _videoDisplayer.Reverse();

            var transform = _mainPanel.GetComponent<RectTransform>();
            Vector3 pos = _menuRectTransform.anchoredPosition;
            Vector3 scale = transform.localScale;

            if (scale.x > 0 && _flipToggle.isOn)
            {
                scale.x *= -1;
                pos.x *= -1;
            }
            else if (scale.x < 0 && !_flipToggle.isOn)
            {
                scale.x *= -1;
                pos.x *= -1;
            }
            transform.localScale = scale;
            _menuRectTransform.anchoredPosition = pos;
        }

        public void OnMuteToggleClicked()
        {
            if (!_isToggleVaild) return;

            if(_muteToggle.isOn)
            {
                _videoDisplayer.Mute();
            }
            else
            {
                _videoDisplayer.Unmute();
            }
        }

        public void OnOrderSliderValueChanged()
        {
            if (!_isSliderVaild) return;

            _orderValueText.text = _orderSlider.value.ToString();
            _canvas.sortingOrder = (int)_orderSlider.value;
        }

        public void OnVolumeChanged()
        {
            if (!_isSliderVaild) return;

            _volumeValueText.text = _volumeSlider.value.ToString();
            _videoDisplayer.Volume = (int)_volumeSlider.value;

            _videoDisplayer.AudioStop();
            _videoDisplayer.PlayMicrophone(_audioDevicesDropdown.value - 1);
        }
    }

}// ResizableCapturedSource
