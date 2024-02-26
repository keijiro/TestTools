using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.Video;

namespace Klak.TestTools {

public sealed partial class ImageSource : MonoBehaviour
{
    #region Project asset reference

    [SerializeField, HideInInspector, FormerlySerializedAs("_shader")]
    Shader _generatorShader = null;

    #endregion

    #region Private objects with lazy initialization

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

    #region Private members

    UnityWebRequest _webTexture;
    WebCamTexture _webcam;

    void InitializeSource()
    {
        // Texture source type
        if (SourceType == ImageSourceType.Texture && SourceTexture != null)
            BlitToOutput(SourceTexture);

        // Texture URL source type
        if (SourceType == ImageSourceType.TextureUrl && IsSourceUrlGiven)
        {
            _webTexture = UnityWebRequestTexture.GetTexture(SourceUrl);
            _webTexture.SendWebRequest();
        }

        // Video source type
        if ((SourceType == ImageSourceType.Video && SourceVideo != null) ||
            (SourceType == ImageSourceType.VideoUrl && IsSourceUrlGiven))
        {
            var player = gameObject.AddComponent<VideoPlayer>();
            player.source = SourceType == ImageSourceType.Video ?
                            VideoSource.VideoClip : VideoSource.Url;
            player.clip = SourceVideo;
            player.url = SourceUrl;
            player.isLooping = true;
            player.renderMode = VideoRenderMode.APIOnly;
            player.Play();
        }

        // Webcam source type
        if (SourceType == ImageSourceType.Webcam)
        {
            _webcam = new WebCamTexture(DeviceName, DeviceResolution.x,
                                        DeviceResolution.y, DeviceFrameRate);
            _webcam.Play();
        }

        // Card source type
        if (SourceType == ImageSourceType.Card)
            BlitToOutputWithCardGenerator(new Vector2(OutputBuffer.width,
                                                      OutputBuffer.height));
    }

    void UpdateSource()
    {
        if (SourceType == ImageSourceType.Video)
            BlitToOutput(GetComponent<VideoPlayer>().texture);

        if (SourceType == ImageSourceType.Webcam && _webcam.didUpdateThisFrame)
            BlitToOutput(_webcam, _webcam.videoVerticallyMirrored);

        // Asynchronous image downloading
        if (_webTexture != null && _webTexture.isDone)
        {
            var texture = DownloadHandlerTexture.GetContent(_webTexture);
            _webTexture.Dispose();
            _webTexture = null;
            BlitToOutput(texture);
            Destroy(texture);
        }

        if (SourceType == ImageSourceType.Gradient)
            BlitToOutputWithGradientGenerator();

        if (SourceType == ImageSourceType.Camera)
        {
            SourceCamera.targetTexture = OutputBuffer;
            if (!SourceCamera.enabled) SourceCamera.Render();
        }

#if KLAK_NDI_AVAILABLE
        if (SourceType == ImageSourceType.Ndi)
            BlitToOutput(_ndiReceiver?.texture);
#endif
    }

    #endregion
}

} // namespace Klak.TestTools
