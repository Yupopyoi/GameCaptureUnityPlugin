using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class VideoDisplayer : MonoBehaviour
{
    private WebCamTexture _webCamTexture;
    private WebCamDevice[] _devices;
    private RawImageÅ@_rawImage;

    [Tooltip("Specify the camera device index to use by default. Set to 0 unless you have no reason.")]
    [SerializeField] int _defaultDeviceIndex = 0;

    void Start()
    {
        _rawImage = GetComponent<RawImage>();
        LoadDevices();
        PlayUsingSelectedDevice(_defaultDeviceIndex);
    }

    #region Properties

    public string[] DeviceNames
    {
        get
        {
            string[] devicesNames = new string[_devices.Length];
            for (int i = 0; i < _devices.Length; i++)
            {
                devicesNames[i] = _devices[i].name;
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

    public int[] CameraAspectRatio()
    {
        int[] aspectRatio = new int[2];
        if (_webCamTexture != null)
        {
            aspectRatio[0] = _webCamTexture.width;
            aspectRatio[1] = _webCamTexture.height;
        }

        while (aspectRatio[0] % 2 == 0 && aspectRatio[1] % 2 == 0)
        {
            aspectRatio[0] /= 2;
            aspectRatio[1] /= 2;
        }

        while (aspectRatio[0] % 3 == 0 && aspectRatio[1] % 3 == 0)
        {
            aspectRatio[0] /= 3;
            aspectRatio[1] /= 3;
        }

        while (aspectRatio[0] % 5 == 0 && aspectRatio[1] % 5 == 0)
        {
            aspectRatio[0] /= 5;
            aspectRatio[1] /= 5;
        }

        return aspectRatio;
    }

    #endregion

    public void LoadDevices()
    {
        _devices = WebCamTexture.devices;
    }

    public void PlayUsingSelectedDevice(int deviceIndex)
    {
        if (WebCamTexture.devices.Length <= deviceIndex) return;

        _webCamTexture = new WebCamTexture(_devices[deviceIndex].name);
        _rawImage.texture = _webCamTexture;

        _webCamTexture.Play();
        CameraAspectRatio();
    }

    public void Stop()
    {
        if (_webCamTexture != null)
        {
            _webCamTexture.Stop();
        }
    }

    void OnDestroy()
    {
        Stop();
    }
}