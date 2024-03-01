using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;
using System.Linq;
using System.Collections.Generic;
using Klak.TestTools;
using Klak.Ndi;

public sealed class SourceSelector : MonoBehaviour
{
    #region Editable property

    [field:SerializeField]
    public UIDocument TargetUI { get; set; }

    #endregion

    #region Data source accessor for UI Toolkit

    [CreateProperty]
    public List<string> SourceList { set; get; } = new List<string>();

    #endregion

    #region Project asset reference

    [SerializeField, HideInInspector]
    NdiResources _ndiResources;

    #endregion

    #region UI methods

    void ToggleUI()
      => TargetUI.rootVisualElement.Q("SourceSelector").visible ^= true;

    void UpdateOptions()
      => SourceList =
           WebCamTexture.devices.Select(dev => "UVC - " + dev.name).
           Concat(NdiFinder.sourceNames.Select(name => "NDI - " + name)).
           ToList();

    void SelectSource(string name)
    {
        var source = GetComponent<ImageSource>();

        // NDI Receiver finalization
        if (GetComponent<NdiReceiver>() != null)
            Destroy(GetComponent<NdiReceiver>());

        if (name.StartsWith("UVC"))
        {
            // UVC (Webcam) source
            source.SourceType = ImageSourceType.Webcam;
            source.DeviceName = name.Substring(6);
        }
        else
        {
            // NDI source (with addition of the NDI Receiver component)
            var recv = gameObject.AddComponent<NdiReceiver>();
            recv.SetResources(_ndiResources);
            recv.ndiName = name.Substring(6);
            source.SourceType = ImageSourceType.Ndi;
            source.NdiReceiver = recv;
        }
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        // UI root: Make it clickable and set this as a data source.
        var root = TargetUI.rootVisualElement;
        root.AddManipulator(new Clickable(ToggleUI));
        root.dataSource = this;

        // Dropdown selector: Hook the callbacks up and intially hide the UI.
        var list = root.Q<DropdownField>("SourceDropdown");
        list.RegisterValueChangedCallback(evt => SelectSource(evt.newValue));
        list.RegisterCallback<FocusEvent>(_ => UpdateOptions());
    }

    #endregion
}
