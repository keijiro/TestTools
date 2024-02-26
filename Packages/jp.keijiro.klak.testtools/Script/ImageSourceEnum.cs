using UnityEngine;

namespace Klak.TestTools {

public enum ImageSourceType
{
    Texture,
    Video,
    Webcam,
    Card,
    Gradient,
    Camera,
    [InspectorName("NDI")] Ndi,
    [InspectorName("Image URL")] TextureUrl,
    [InspectorName("Video URL")] VideoUrl
}

} // namespace Klak.TestTools
