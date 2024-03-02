using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.Video;
#if KLAK_NDI_AVAILABLE
using Klak.Ndi;
#endif

namespace Klak.TestTools {

partial class ImageSource
{
    #region Project asset reference

    [SerializeField, HideInInspector, FormerlySerializedAs("_shader")]
    Shader _generatorShader = null;

    #if KLAK_NDI_AVAILABLE
    [SerializeField, HideInInspector]
    NdiResources _ndiResources;
    #endif

    #endregion

    #region Private helper property

    bool IsSourceUrlGiven => !string.IsNullOrEmpty(SourceUrl);

    RenderTexture OutputBuffer =>
      OutputDestination != null ? OutputDestination : InternalOutputBuffer;

    #endregion

    #region Blit methods

    void BlitToOutput(Texture source, bool vflip = false)
    {
        if (source == null) return;

        var aspect1 = (float)source.width / source.height;
        var aspect2 = (float)OutputBuffer.width / OutputBuffer.height;

        var scale = new Vector2(aspect2 / aspect1, aspect1 / aspect2);
        scale = Vector2.Min(Vector2.one, scale);
        if (vflip) scale.y *= -1;

        var offset = (Vector2.one - scale) / 2;

        Graphics.Blit(source, OutputBuffer, scale, offset);
    }

    void BlitToOutputWithCardGenerator(Vector2 resolution)
    {
        GeneratorMaterial.SetVector("_Resolution", resolution);
        Graphics.Blit(null, OutputBuffer, GeneratorMaterial, 0);
    }

    void BlitToOutputWithGradientGenerator()
      => Graphics.Blit(null, OutputBuffer, GeneratorMaterial, 1);

    #endregion

    #region Common objects with lazy initialization

    Material _generatorMaterial;
    RenderTexture _internalOutputBuffer;

    Material GeneratorMaterial =>
      _generatorMaterial != null ? _generatorMaterial :
        _generatorMaterial = new Material(_generatorShader);

    RenderTexture InternalOutputBuffer =>
      _internalOutputBuffer != null ? _internalOutputBuffer :
        _internalOutputBuffer = new RenderTexture(OutputResolution.x,
                                                  OutputResolution.y, 0);

    void DestroyLazyObjects()
    {
        if (_generatorMaterial != null)
        {
            Destroy(_generatorMaterial);
            _generatorMaterial = null;
        }

        if (_internalOutputBuffer != null)
        {
            Destroy(_internalOutputBuffer);
            _internalOutputBuffer = null;
        }
    }

    #endregion

    #region Per-source intermediate objects

    // Webcam source
    WebCamTexture _webcam;

    WebCamTexture InitWebcam()
    {
        Application.RequestUserAuthorization(UserAuthorization.WebCam);
        _webcam = new WebCamTexture(SourceName, DeviceResolution.x,
                                    DeviceResolution.y, DeviceFrameRate);
        return _webcam;
    }

    // Video source
    VideoPlayer _videoPlayer;

    VideoPlayer AttachVideoPlayer(VideoClip clip, string url)
    {
        if (clip == null && string.IsNullOrEmpty(url)) return null;
        _videoPlayer = gameObject.AddComponent<VideoPlayer>();
        _videoPlayer.source = clip != null ? VideoSource.VideoClip : VideoSource.Url;
        _videoPlayer.clip = clip;
        _videoPlayer.url = url;
        _videoPlayer.isLooping = true;
        _videoPlayer.renderMode = VideoRenderMode.APIOnly;
        return _videoPlayer;
    }

    #if KLAK_NDI_AVAILABLE
    // NDI source
    NdiReceiver _ndiReceiver;

    NdiReceiver AttachNdiReceiver(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        _ndiReceiver = gameObject.AddComponent<NdiReceiver>();
        _ndiReceiver.SetResources(_ndiResources);
        _ndiReceiver.ndiName = name;
        return _ndiReceiver;
    }
    #endif

    // Web texture (image from URL)
    UnityWebRequest _webTexture;

    UnityWebRequest RequestWebTexture()
    {
        if (string.IsNullOrEmpty(SourceUrl)) return null;
        _webTexture = UnityWebRequestTexture.GetTexture(SourceUrl);
        return _webTexture;
    }

    void BlitAndDestroyWebTexture()
    {
        var texture = DownloadHandlerTexture.GetContent(_webTexture);
        _webTexture.Dispose();
        _webTexture = null;
        BlitToOutput(texture);
        Destroy(texture);
    }

    // Common finalizer
    void DestroyIntermediateObjects()
    {
        if (_webcam != null)
        {
            Destroy(_webcam);
            _webcam = null;
        }

        if (_videoPlayer != null)
        {
            Destroy(_videoPlayer);
            _videoPlayer = null;
        }

        #if KLAK_NDI_AVAILABLE
        if (_ndiReceiver != null)
        {
            Destroy(_ndiReceiver);
            _ndiReceiver = null;
        }
        #endif

        if (_webTexture != null)
        {
            _webTexture.Dispose();
            _webTexture = null;
        }
    }

    #endregion

    #region Source accessors

    void InitializeSource()
    {
        // Video source initialization
        if (SourceType == ImageSourceType.Video)
            AttachVideoPlayer(SourceVideo, null)?.Play();

        // Webcam source initialization
        if (SourceType == ImageSourceType.Webcam)
            InitWebcam()?.Play();

        // Card source initialization
        if (SourceType == ImageSourceType.Card)
            BlitToOutputWithCardGenerator(new Vector2(OutputBuffer.width,
                                                      OutputBuffer.height));

        #if KLAK_NDI_AVAILABLE
        // NDI source type
        if (SourceType == ImageSourceType.Ndi)
            AttachNdiReceiver(SourceName);
        #endif

        // Texture URL source type
        if (SourceType == ImageSourceType.TextureUrl && IsSourceUrlGiven)
            RequestWebTexture().SendWebRequest();

        // Video (URL) source initialization
        if (SourceType == ImageSourceType.VideoUrl)
            AttachVideoPlayer(null, SourceUrl)?.Play();
    }

    void UpdateSource()
    {
        // Texture source update
        if (SourceType == ImageSourceType.Texture)
            BlitToOutput(SourceTexture);

        // Video source update (including Video URL source)
        if (_videoPlayer != null)
            BlitToOutput(_videoPlayer.texture);

        // Webcam source update
        if (_webcam != null && _webcam.didUpdateThisFrame)
            BlitToOutput(_webcam, _webcam.videoVerticallyMirrored);

        // Gradient source update
        if (SourceType == ImageSourceType.Gradient)
            BlitToOutputWithGradientGenerator();

        // Camera source update
        if (SourceType == ImageSourceType.Camera &&
            SourceCamera != null && !SourceCamera.enabled)
        {
            SourceCamera.targetTexture = OutputBuffer;
            SourceCamera.Render();
        }

        #if KLAK_NDI_AVAILABLE
        // NDI source update
        if (_ndiReceiver != null)
            BlitToOutput(_ndiReceiver.texture);
        #endif

        // Texture (URL) update
        if (_webTexture != null && _webTexture.isDone)
            BlitAndDestroyWebTexture();
    }

    #endregion
}

} // namespace Klak.TestTools
