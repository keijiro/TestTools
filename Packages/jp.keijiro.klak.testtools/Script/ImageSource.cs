using UnityEngine;

namespace Klak.TestTools {

public sealed partial class ImageSource : MonoBehaviour
{
    void Start()
      => InitializeSource();

    void OnDestroy()
    {
        if (_webcam != null)
        {
            Destroy(_webcam);
            _webcam = null;
        }

        DestroyLazyObjects();
    }

    void Update()
        => UpdateSource();
}

} // namespace Klak.TestTools
