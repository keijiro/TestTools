using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Video;

namespace Klak.TestTools {

partial class ImageSource
{
    #region General properties

    [SerializeField]
    ImageSourceType _sourceType = ImageSourceType.Card;

    public ImageSourceType SourceType
      { get => _sourceType;
        set { _sourceType = value; OnValidate(); } } 

    [SerializeField]
    Vector2Int _outputResolution = new Vector2Int(1920, 1080);

    public Vector2Int OutputResolution
      { get => _outputResolution;
        set { _outputResolution = value; OnValidate(); } }

    [SerializeField, FormerlySerializedAs("_outputTexture")]
    RenderTexture _outputDestination = null;

    public RenderTexture OutputDestination
      { get => _outputDestination;
        set { _outputDestination = value; OnValidate(); } }

    #endregion

    #region Source object references

    [field:SerializeField, FormerlySerializedAs("_texture")]
    public Texture2D SourceTexture { get; set; } = null;

    [SerializeField, FormerlySerializedAs("_video")]
    VideoClip _sourceVideo = null;

    public VideoClip SourceVideo
      { get => _sourceVideo;
        set { _sourceVideo = value; OnValidate(); } }

    [field:SerializeField, FormerlySerializedAs("_camera")]
    public Camera SourceCamera { get; set; } = null;

    #endregion

    #region Additional source information

    [SerializeField, FormerlySerializedAs("_textureUrl")]
    string _sourceUrl = null;

    public string SourceUrl
      { get => _sourceUrl;
        set { _sourceUrl = value; OnValidate(); } }

    [SerializeField, FormerlySerializedAs("_webcamName")]
    string _sourceName = "";

    public string SourceName
      { get => _sourceName;
        set { _sourceName = value; OnValidate(); } }

    [SerializeField, FormerlySerializedAs("_webcamResolution")]
    Vector2Int _deviceResolution = Vector2Int.zero;

    public Vector2Int DeviceResolution
      { get => _deviceResolution;
        set { _deviceResolution = value; OnValidate(); } }

    [SerializeField, FormerlySerializedAs("_webcamFrameRate")]
    int _deviceFrameRate = 0;

    public int DeviceFrameRate
      { get => _deviceFrameRate;
        set { _deviceFrameRate = value; OnValidate(); } }

    #endregion

    #region Runtime property

    public Texture AsTexture => OutputBuffer;
    public RenderTexture AsRenderTexture => OutputBuffer;

    #endregion
}

} // namespace Klak.TestTools
