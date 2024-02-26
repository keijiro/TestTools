using UnityEditor;
using UnityEngine;

namespace Klak.TestTools {

[CustomEditor(typeof(ImageSource))]
sealed class ImageSourceEditor : Editor
{
    #region Label objects

    static class Labels
    {
        public static Label Asset = "Asset";
        public static Label Camera = "Camera";
        public static Label Destination = "Destination";
        public static Label DeviceName = "Device Name";
        public static Label FrameRate = "Frame Rate";
        public static Label NdiReceiver = "NDI Receiver";
        public static Label Resolution = "Resolution";
        public static Label Select = "Select";
        public static Label URL = "URL";
        public const string NdiError =
          "The NDI receiver component is not available. " +
          "Import the KlakNDI package to use this feature.";
    }

    #endregion

    #region AutoProperty set

    AutoProperty _sourceType;
    AutoProperty _outputResolution;
    AutoProperty _outputDestination;

    AutoProperty SourceTexture;
    AutoProperty _sourceVideo;
    AutoProperty SourceCamera;
#if KLAK_NDI_AVAILABLE
    AutoProperty NdiReceiver;
#endif

    AutoProperty _sourceUrl;
    AutoProperty _deviceName;
    AutoProperty _deviceResolution;
    AutoProperty _deviceFrameRate;

    #endregion

    #region Webcam helpers

    void ChangeWebcam(string name)
    {
        serializedObject.Update();
        _deviceName.Target.stringValue = name;
        serializedObject.ApplyModifiedProperties();
    }

    void ShowDeviceSelector(Rect rect)
    {
        var menu = new GenericMenu();
        foreach (var device in WebCamTexture.devices)
            menu.AddItem(new GUIContent(device.name), false,
                         () => ChangeWebcam(device.name));
        menu.DropDown(rect);
    }

    #endregion

    #region Editor implementation

    void OnEnable() => AutoProperty.Scan(this);

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_sourceType);

        EditorGUI.indentLevel++;

        var type = (ImageSourceType)_sourceType.Target.enumValueIndex;

        if (type == ImageSourceType.Texture)
            EditorGUILayout.PropertyField(SourceTexture, Labels.Asset);

        if (type == ImageSourceType.Video)
            EditorGUILayout.PropertyField(_sourceVideo, Labels.Asset);

        if (type == ImageSourceType.Camera)
            EditorGUILayout.PropertyField(SourceCamera, Labels.Camera);

#if KLAK_NDI_AVAILABLE
        if (type == ImageSourceType.Ndi)
            EditorGUILayout.PropertyField(NdiReceiver, Labels.NdiReceiver);
#endif

        if (type == ImageSourceType.TextureUrl ||
            type == ImageSourceType.VideoUrl)
            EditorGUILayout.PropertyField(_sourceUrl, Labels.URL);

        if (type == ImageSourceType.Webcam)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(_deviceName, Labels.DeviceName);
            var rect = EditorGUILayout.GetControlRect(false, GUILayout.Width(60));
            if (EditorGUI.DropdownButton(rect, Labels.Select, FocusType.Keyboard))
                ShowDeviceSelector(rect);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(_deviceResolution, Labels.Resolution);
            EditorGUILayout.PropertyField(_deviceFrameRate, Labels.FrameRate);
        }

        EditorGUI.indentLevel--;

#if !KLAK_NDI_AVAILABLE
        if (type == ImageSourceType.Ndi)
            EditorGUILayout.HelpBox(Labels.NdiError, MessageType.Error);
#endif

        EditorGUILayout.PropertyField(_outputDestination, Labels.Destination);

        if (_outputDestination.Target.objectReferenceValue == null)
            EditorGUILayout.PropertyField(_outputResolution);

        serializedObject.ApplyModifiedProperties();
    }

    #endregion
}

} // namespace Klak.TestTools
