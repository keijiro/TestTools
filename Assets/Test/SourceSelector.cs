using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;
using System.Linq;
using System.Collections.Generic;
using Klak.TestTools;

public sealed class SourceSelector : MonoBehaviour
{
    [field:SerializeField]
    public UIDocument TargetUI { get; set; }

    [CreateProperty]
    public List<string> SourceList { set; get; } = new List<string>();

    void UpdateOptions()
    {
        SourceList =
          WebCamTexture.devices.Select(dev => "UVC: " + dev.name).ToList();
    }

    void ToggleUI()
    {
        var list = TargetUI.rootVisualElement.Q("Selector");
        list.visible = !list.visible;
        if (list.visible) UpdateOptions();
    }

    void SelectSource(string name)
    {
        var source = GetComponent<ImageSource>();
        source.SourceType = ImageSourceType.Webcam;
        source.DeviceName = name.Substring(5); // "UVC: "
    }

    async Awaitable Start()
    {
        var root = TargetUI.rootVisualElement;
        root.AddManipulator(new Clickable(ToggleUI));
        root.dataSource = this;

        var list = root.Q<DropdownField>("Selector");
        list.RegisterValueChangedCallback(e => SelectSource(e.newValue));
        list.visible = false;

        await Application.RequestUserAuthorization(UserAuthorization.WebCam);
    }
}
