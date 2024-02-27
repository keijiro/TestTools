using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.Video;

namespace Klak.TestTools {

partial class ImageSource
{
    #region Project asset reference

    [SerializeField, HideInInspector, FormerlySerializedAs("_shader")]
    Shader _generatorShader = null;

    #endregion

    #region Private objects with lazy initialization

    Material _generatorMaterial;
    RenderTexture _internalOutputBuffer;
    WebCamTexture _webcam;
    VideoPlayer _videoPlayer;
    UnityWebRequest _webTexture;

    Material GeneratorMaterial =>
      _generatorMaterial != null ? _generatorMaterial :
        _generatorMaterial = new Material(_generatorShader);

    RenderTexture InternalOutputBuffer =>
      _internalOutputBuffer != null ? _internalOutputBuffer :
        _internalOutputBuffer = new RenderTexture(OutputResolution.x,
                                                  OutputResolution.y, 0);
 
    WebCamTexture Webcam =>
      _webcam != null ? _webcam :
        _webcam = new WebCamTexture(DeviceName, DeviceResolution.x,
                                    DeviceResolution.y, DeviceFrameRate);

    UnityWebRequest WebTexture =>
      _webTexture != null ? _webTexture :
        _webTexture = (IsSourceUrlGiven ? UnityWebRequestTexture.GetTexture(SourceUrl) : null);

    VideoPlayer AttachedVideoPlayer =>
      _videoPlayer != null ? _videoPlayer :
        _videoPlayer = AttachVideoPlayer();

    VideoPlayer AttachVideoPlayer()
    {
        if (SourceType == ImageSourceType.Video && SourceVideo != null)
            return AttachVideoPlayer(SourceVideo, null);
        if (SourceType == ImageSourceType.VideoUrl && IsSourceUrlGiven)
            return AttachVideoPlayer(null, SourceUrl);
        return null;
    }

    VideoPlayer AttachVideoPlayer(VideoClip clip, string url)
    {
        var player = gameObject.AddComponent<VideoPlayer>();
        player.source = clip != null ? VideoSource.VideoClip : VideoSource.Url;
        player.clip = clip;
        player.url = url;
        player.isLooping = true;
        player.renderMode = VideoRenderMode.APIOnly;
        return player;
    }

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

        if (_webTexture != null)
        {
            _webTexture.Dispose();
            _webTexture = null;
        }
    }

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

    #region Source accessors

    void InitializeSource()
    {
        // Video source initialization
        if (SourceType == ImageSourceType.Video)
            AttachedVideoPlayer?.Play();

        // Webcam source initialization
        if (SourceType == ImageSourceType.Webcam)
            Webcam.Play();

        // Card source initialization
        if (SourceType == ImageSourceType.Card)
            BlitToOutputWithCardGenerator(new Vector2(OutputBuffer.width,
                                                      OutputBuffer.height));

        // Texture URL source type
        if (SourceType == ImageSourceType.TextureUrl && IsSourceUrlGiven)
            WebTexture.SendWebRequest();

        // Video (URL) source initialization
        if (SourceType == ImageSourceType.VideoUrl)
            AttachedVideoPlayer?.Play();
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
        if (SourceType == ImageSourceType.Webcam && _webcam.didUpdateThisFrame)
            BlitToOutput(_webcam, _webcam.videoVerticallyMirrored);

        // Gradient source update
        if (SourceType == ImageSourceType.Gradient)
            BlitToOutputWithGradientGenerator();

        // Camera source update
        if (SourceType == ImageSourceType.Camera)
        {
            SourceCamera.targetTexture = OutputBuffer;
            if (!SourceCamera.enabled) SourceCamera.Render();
        }

#if KLAK_NDI_AVAILABLE
        // NDI source update
        if (SourceType == ImageSourceType.Ndi)
            BlitToOutput(_ndiReceiver?.texture);
#endif

        // Texture (URL) update
        if (_webTexture != null && _webTexture.isDone)
        {
            var texture = DownloadHandlerTexture.GetContent(_webTexture);
            _webTexture.Dispose();
            _webTexture = null;
            BlitToOutput(texture);
            Destroy(texture);
        }
    }

    #endregion
}

} // namespace Klak.TestTools
