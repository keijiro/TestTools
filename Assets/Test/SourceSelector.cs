using Klak.Ndi;
using Klak.TestTools;
using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using UnityEngine.UIElements;
using UnityEngine;

public sealed class SourceSelector : MonoBehaviour
{
    #region Data source accessor for UI Toolkit

    [CreateProperty]
    public List<string> SourceList { set; get; } = new List<string>();

    #endregion

    #region UI methods

    VisualElement UIRoot => GetComponent<UIDocument>().rootVisualElement;

    void ToggleUI()
      => UIRoot.Q("Selector").visible ^= true;

    void UpdateOptions()
      => SourceList =
           WebCamTexture.devices.Select(dev => "UVC - " + dev.name).
           Concat(NdiFinder.sourceNames.Select(name => "NDI - " + name)).
           ToList();

    void SelectSource(string name)
    {
        var source = GetComponent<ImageSource>();
        source.SourceName = name.Substring(6);
        source.SourceType = name.StartsWith("UVC") ?
          ImageSourceType.Webcam : ImageSourceType.Ndi;
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        // UI root: Make it clickable and set this as a data source.
        UIRoot.AddManipulator(new Clickable(ToggleUI));
        UIRoot.dataSource = this;

        // Dropdown selector: Hook the callbacks up and intially hide the UI.
        var list = UIRoot.Q<DropdownField>("Dropdown");
        list.RegisterValueChangedCallback(evt => SelectSource(evt.newValue));
        list.RegisterCallback<FocusEvent>(_ => UpdateOptions());
    }

    #endregion
}
