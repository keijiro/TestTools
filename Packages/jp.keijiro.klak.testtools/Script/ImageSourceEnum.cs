using UnityEngine;

namespace Klak.TestTools {

public enum ImageSourceType
{
    Texture,
    [InspectorName("Image URL")] TextureUrl,
    Video,
    [InspectorName("Video URL")] VideoUrl,
    Webcam,
    Card,
    Gradient,
    Camera,
    [InspectorName("NDI")] Ndi
}

} // namespace Klak.TestTools
