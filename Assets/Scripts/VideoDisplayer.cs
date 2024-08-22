using UnityEngine;
using UnityEngine.UI;

#if UNITY_STANDALONE_WIN
using NAudio.Wave;
#endif

namespace ResizableCapturedSource
{
    [RequireComponent(typeof(RawImage))]
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Resize))]
    public class VideoDisplayer : MonoBehaviour
    {
        private WebCamTexture _webCamTexture;
        private WebCamDevice[] _videoDevices;

        private RawImage _rawImage;
        private bool _isFlipped = false;

        private AudioSource _audioSource;
        string _microphoneDevice;

        private Resize _resize;

        [SerializeField] AudioSource _subAudioSource;

        [Header("Initial state configurations")]
        [Tooltip("Specify the camera device index to use by default. Set to 0 if you have no reason.")]
        [SerializeField, Range(0, 10)] int _defaultVideoDeviceIndex = 0;
        [Tooltip("Specify the audio device index to use by default. Set to 0 if you have no reason.")]
        [SerializeField, Range(0, 10)] int _defaultAudioDeviceIndex = 0;
        [Tooltip("Display the image with the left and right sides flipped from the start.")]
        [SerializeField] bool _isFlippedByDefault = false;

        #region Properties

        public bool IsFlipped() => _isFlipped;

        #endregion
        void Initialize()
        {
            _audioSource.Stop();
            Microphone.End(_microphoneDevice);

            _audioSource = null;
            _audioSource = GetComponent<AudioSource>();

            FastPlayMicrophone();
        }

        void Start()
        {
            InvokeRepeating("Initialize", 120f, 120f);
            _rawImage = GetComponent<RawImage>();
            _audioSource = GetComponent<AudioSource>();
            _resize = GetComponent<Resize>();

            _isFlipped = _isFlippedByDefault;
            Vector3 scale = _rawImage.rectTransform.localScale;
            scale.x = _isFlipped ? -1 : 1;
            _rawImage.rectTransform.localScale = scale;

            LoadDevices();
            PlayUsingSelectedDevice(_defaultVideoDeviceIndex);
            PlayMicrophone(_defaultAudioDeviceIndex);
        }

        #region Properties

        public string[] VideoDeviceNames
        {
            get
            {
                string[] devicesNames = new string[_videoDevices.Length];
                for (int i = 0; i < _videoDevices.Length; i++)
                {
                    devicesNames[i] = _videoDevices[i].name;
                }
                return (string[])devicesNames.Clone();
            }
        }

        public string[] AudioDeviceNames
        {
            get
            {
                string[] devicesNames = new string[Microphone.devices.Length];
                for (int i = 0; i < Microphone.devices.Length; i++)
                {
                    devicesNames[i] = Microphone.devices[i];
                }
                return (string[])devicesNames.Clone();
            }
        }

        public int[] CameraResolution()
        {
            int[] resolution = new int[2];
            if (_webCamTexture != null)
            {
                resolution[0] = _webCamTexture.width;
                resolution[1] = _webCamTexture.height;
            }
            return resolution;
        }

        #endregion

        public void LoadDevices()
        {
            _videoDevices = WebCamTexture.devices;
        }

        public void PlayUsingSelectedDevice(int deviceIndex)
        {
            if (deviceIndex < 0) return;
            if (WebCamTexture.devices.Length <= deviceIndex) return;

            _webCamTexture = new WebCamTexture(_videoDevices[deviceIndex].name);
            _rawImage.texture = _webCamTexture;

            _webCamTexture.Play();

            _resize.SetAspectRatio(CameraResolution());
        }


        public void PlayMicrophone(int deviceIndex)
        {
            if (deviceIndex < 0) return;
            if (Microphone.devices.Length <= deviceIndex) return;

            _microphoneDevice = Microphone.devices[deviceIndex];
            _audioSource.clip = Microphone.Start(_microphoneDevice, true, 120, 48000);

            while (!(Microphone.GetPosition(_microphoneDevice) > 0)) { }

            Unmute();

            _audioSource.Play();
        }

        private void FastPlayMicrophone()
        {
            _audioSource.clip = null;
            Destroy(_audioSource.clip);
            _audioSource.clip = Microphone.Start(_microphoneDevice,true, 120, 48000);
            while (!(Microphone.GetPosition(_microphoneDevice) > 0)) { }
            
            _audioSource.Play();
        }

        public void Mute()
        {
            _audioSource.mute = true;
        }

        public void Unmute()
        {
            _audioSource.mute = false;
        }

        public void Reverse()
        {
            if (_rawImage != null)
            {
                _isFlipped = !_isFlipped;

                Vector3 scale = _rawImage.rectTransform.localScale;
                scale.x = _isFlipped ? -1 : 1;
                _rawImage.rectTransform.localScale = scale;
            }
        }

        public void Stop()
        {
            if (_webCamTexture != null)
            {
                _webCamTexture.Stop();
            }

            if (_audioSource.isPlaying)
            {
                _audioSource.Stop();
                _audioSource.clip = null;
            }
            if (Microphone.IsRecording(_microphoneDevice))
            {
                Microphone.End(_microphoneDevice);
            }
        }

        void OnDestroy()
        {
            Stop();
            CancelInvoke("Initialize");
        }
    }
}// namespace ResizableCapturedSource
