using UnityEngine;

namespace Klak.TestTools {

public sealed partial class ImageSource : MonoBehaviour
{
    bool _initialized;

    void OnValidate() => OnDestroy();

    void OnDestroy()
    {
        if (_initialized)
        {
            DestroyIntermediateObjects();
            DestroyLazyObjects();
            _initialized = false;
        }
    }

    void Update()
    {
        if (!_initialized)
        {
            InitializeSource();
            _initialized = true;
        }
        UpdateSource();
    }
}

} // namespace Klak.TestTools
