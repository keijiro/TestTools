using UnityEngine;
using UnityEngine.Video;

namespace Klak.TestTools {

public sealed class ImageSource : MonoBehaviour
{
    #region Source type options

    public enum SourceType { Texture, Video, Webcam, Card }

    [SerializeField] SourceType _sourceType = SourceType.Card;

    #endregion

    #region Texture mode options

    [SerializeField] Texture2D _texture = null;
    [SerializeField] string _textureUrl = null;

    #endregion

    #region Video mode options

    [SerializeField] VideoClip _sourceVideo = null;
    [SerializeField] string _videoUrl = null;

    #endregion

    #region Webcam options

    [SerializeField] string _webcamName = "";
    [SerializeField] Vector2Int _webcamResolution = new Vector2Int(1920, 1080);
    [SerializeField] int _webcamFrameRate = 30;

    #endregion

    #region Output options

    [SerializeField] Vector2Int _outputResolution = new Vector2Int(1920, 1080);

    #endregion

    #region Internal objects

    WebCamTexture _webcam;
    RenderTexture _buffer;

    #endregion

    #region Public properties

    public Texture Texture => _buffer;

    #endregion

    /*
    #region MonoBehaviour implementation

    void Start()
    {
        if (_dummyImage != null) return;
        _webcam = new WebCamTexture
          (_deviceName, _captureSize.x, _captureSize.y, _frameRate);
        _buffer = new RenderTexture(_cropSize.x, _cropSize.y, 0);
        _webcam.Play();
    }

    void OnDestroy()
    {
        if (_webcam != null) Destroy(_webcam);
        if (_buffer != null) Destroy(_buffer);
    }

    void Update()
    {
        if (_dummyImage != null) return;
        if (!_webcam.didUpdateThisFrame) return;

        var aspect1 = (float)_webcam.width / _webcam.height;
        var aspect2 = (float)_cropSize.x / _cropSize.y;
        var gap = aspect2 / aspect1;

        var vflip = _webcam.videoVerticallyMirrored;
        var scale = new Vector2(gap, vflip ? -1 : 1);
        var offset = new Vector2((1 - gap) / 2, vflip ? 1 : 0);

        Graphics.Blit(_webcam, _buffer, scale, offset);
    }

    #endregion
    */
}

} // namespace Klak.TestTools
