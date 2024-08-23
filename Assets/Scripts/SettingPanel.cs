using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEditor.Progress;

namespace ResizableCapturedSource
{
    public class SettingPanel : MonoBehaviour
    {
        [Header("User Settings")]
        [Tooltip("Specify the font.This font will be applied to all text in the panel.")]
        [SerializeField] TMP_FontAsset _tmpFont;

        [Header("System Settings")]
        [SerializeField] GameObject _parentRawImage;
        [SerializeField] TMP_Dropdown _videoDevicesDropdown;
        [SerializeField] TMP_Dropdown _audioDevicesDropdown;
        [SerializeField] TMP_Dropdown _samplingRateDropdown;
        [SerializeField] TMP_Dropdown _channelDropdown;
        [SerializeField] Toggle _muteToggle;

        VideoDisplayer _videoDisplayer;

        private bool _isDropdownVaild = false;
        private bool _isMuteToggleVaild = false;

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
                        itemText.fontSize = 30;
                    }
                    if (item.TryGetComponent<RectTransform>(out var itemRect))
                    {
                        itemRect.sizeDelta = new Vector2(0, 40);
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
                        itemText.fontSize = 20;
                    }
                    if (item.TryGetComponent<RectTransform>(out var itemRect))
                    {
                        itemRect.sizeDelta = new Vector2(0, 50);
                    }
                }
            }
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
                videoDevicesList.Add(device);
            }
            foreach (var device in audioDevices)
            {
                audioDevicesList.Add(device);
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

            _isDropdownVaild = true;

            // ----- Mute -----

            _isMuteToggleVaild = false;
            if (_videoDisplayer.IsMute())
            {
                _muteToggle.isOn = true;
            }
            else
            {
                _muteToggle.isOn = false;
            }

            _isMuteToggleVaild = true;
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
            int samplingRate = deviceIndex == 0 ? 44100 : 48000;

            _videoDisplayer.AudioStop();

            _videoDisplayer.SetSamplingRate(samplingRate);

            if (_audioDevicesDropdown.value == 0) return;

            _videoDisplayer.PlayMicrophone(_audioDevicesDropdown.value - 1);
        }

        public void OnMuteToggleClicked()
        {

            if (!_isMuteToggleVaild) return;

            if(_muteToggle.isOn)
            {
                _videoDisplayer.Mute();
            }
            else
            {
                _videoDisplayer.Unmute();
            }
        }
    }

}// ResizableCapturedSource
