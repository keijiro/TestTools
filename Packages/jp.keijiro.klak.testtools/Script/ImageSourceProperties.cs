using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Video;

namespace Klak.TestTools {

partial class ImageSource
{
    #region General properties

    [field:SerializeField, FormerlySerializedAs("_sourceType")]
    public ImageSourceType SourceType { get; set; } = ImageSourceType.Card;

    [field:SerializeField, FormerlySerializedAs("_outputResolution")]
    public Vector2Int OutputResolution { get; set; } = new Vector2Int(1920, 1080);

    [field:SerializeField, FormerlySerializedAs("_outputTexture")]
    public RenderTexture OutputDestination { get; set; } = null;

    #endregion

    #region Source object references

    [field:SerializeField, FormerlySerializedAs("_texture")]
    public Texture2D SourceTexture { get; set; } = null;

    [field:SerializeField, FormerlySerializedAs("_video")]
    public VideoClip SourceVideo { get; set; } = null;

    [field:SerializeField, FormerlySerializedAs("_camera")]
    public Camera SourceCamera { get; set; } = null;

#if KLAK_NDI_AVAILABLE
    [SerializeField, FormerlySerializedAs("_ndiReceiver")]
    public Klak.Ndi.NdiReceiver NdiReceiver { get; set; } = null;
#endif

    #endregion

    #region Additional source information

    [field:SerializeField, FormerlySerializedAs("_textureUrl")]
    public string SourceUrl { get; set; } = null;

    [field:SerializeField, FormerlySerializedAs("_webcamName")]
    public string DeviceName { get; set; } = "";

    [field:SerializeField, FormerlySerializedAs("_webcamResolution")]
    public Vector2Int DeviceResolution { get; set; } = Vector2Int.zero;

    [field:SerializeField, FormerlySerializedAs("_webcamFrameRate")]
    public int DeviceFrameRate { get; set; } = 0;

    #endregion

    #region Runtime property

    public Texture AsTexture => OutputBuffer;

    #endregion
}

} // namespace Klak.TestTools
