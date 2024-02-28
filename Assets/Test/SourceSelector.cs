using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;
using System.Linq;
using System.Collections.Generic;

public sealed class SourceSelector : MonoBehaviour
{
    [CreateProperty]
    public List<string> SourceList { set; get; }

    async Awaitable Start()
    {
        await Application.RequestUserAuthorization(UserAuthorization.WebCam);
        SourceList = WebCamTexture.devices.Select(dev => dev.name).ToList();
        GetComponent<UIDocument>().rootVisualElement.dataSource = this;
        //selector.RegisterValueChangedCallback(e => SelectDevice(e.newValue));
    }
}
