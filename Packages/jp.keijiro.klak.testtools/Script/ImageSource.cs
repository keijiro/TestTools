using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

namespace Klak.TestTools {

public sealed class ImageSource : MonoBehaviour
{
    #region Source type options

    public enum SourceType { Texture, Video, Webcam, Card, Gradient }

    [SerializeField] SourceType _sourceType = SourceType.Card;

    #endregion

    #region Texture mode options

    [SerializeField] Texture2D _texture = null;
    [SerializeField] string _textureUrl = null;

    #endregion

    #region Video mode options

    [SerializeField] VideoClip _video = null;
    [SerializeField] string _videoUrl = null;

    #endregion

    #region Webcam options

    [SerializeField] string _webcamName = "";
    [SerializeField] Vector2Int _webcamResolution = new Vector2Int(1920, 1080);
    [SerializeField] int _webcamFrameRate = 30;

    #endregion

    #region Output options

    [SerializeField] RenderTexture _outputTexture = null;
    [SerializeField] Vector2Int _outputResolution = new Vector2Int(1920, 1080);

    #endregion

    #region Public properties

    public RenderTexture Texture
      => _outputTexture != null ? _outputTexture : _buffer;

    #endregion

    #region Package asset reference

    [SerializeField, HideInInspector] Shader _shader = null;

    #endregion

    #region Private members

    UnityWebRequest _webTexture;
    WebCamTexture _webcam;
    RenderTexture _buffer;

    void Blit(Texture source, bool vflip = false)
    {
        if (source == null) return;

        var aspect1 = (float)source.width / source.height;
        var aspect2 = (float)Texture.width / Texture.height;
        var gap = aspect2 / aspect1;

        var scale = new Vector2(gap, vflip ? -1 : 1);
        var offset = new Vector2((1 - gap) / 2, vflip ? 1 : 0);

        Graphics.Blit(source, Texture, scale, offset);
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        if (_outputTexture == null)
            _buffer = new RenderTexture
              (_outputResolution.x, _outputResolution.y, 0);

        if (_sourceType == SourceType.Texture)
        {
            if (_texture != null)
            {
                Blit(_texture);
            }
            else
            {
                _webTexture = UnityWebRequestTexture.GetTexture(_textureUrl);
                _webTexture.SendWebRequest();
            }
        }

        if (_sourceType == SourceType.Video)
        {
            var player = gameObject.AddComponent<VideoPlayer>();
            player.source =
              _video != null ? VideoSource.VideoClip : VideoSource.Url;
            player.clip = _video;
            player.url = _videoUrl;
            player.isLooping = true;
            player.renderMode = VideoRenderMode.APIOnly;
            player.Play();
        }

        if (_sourceType == SourceType.Webcam)
        {
            _webcam = new WebCamTexture
              (_webcamName,
               _webcamResolution.x, _webcamResolution.y, _webcamFrameRate);
            _webcam.Play();
        }

        if (_sourceType == SourceType.Card)
        {
            ShaderMaterial.SetVector("_Resolution", new Vector2(Texture.width, Texture.height));
            Graphics.Blit(null, Texture, ShaderMaterial, 0);
        }
    }

    Material _material;

    Material ShaderMaterial => GetShaderMaterial();

    Material GetShaderMaterial()
    {
        if (_material == null)
            _material = new Material(_shader);
        return _material;
    }

    void OnDestroy()
    {
        if (_webcam != null) Destroy(_webcam);
        if (_buffer != null) Destroy(_buffer);
        if (_material != null) Destroy(_material);
    }

    void Update()
    {
        if (_sourceType == SourceType.Video)
            Blit(GetComponent<VideoPlayer>().texture);

        if (_sourceType == SourceType.Webcam && _webcam.didUpdateThisFrame)
            Blit(_webcam, _webcam.videoVerticallyMirrored);

        if (_webTexture != null && _webTexture.isDone)
        {
            var texture = DownloadHandlerTexture.GetContent(_webTexture);
            _webTexture.Dispose();
            _webTexture = null;
            Blit(texture);
            Destroy(texture);
        }

        if (_sourceType == SourceType.Gradient)
            Graphics.Blit(null, Texture, ShaderMaterial, 1);
    }

    #endregion
}

} // namespace Klak.TestTools
