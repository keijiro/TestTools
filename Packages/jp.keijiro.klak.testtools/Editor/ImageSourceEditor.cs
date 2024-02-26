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

    AutoProperty SourceType;
    AutoProperty OutputResolution;
    AutoProperty OutputDestination;

    AutoProperty SourceTexture;
    AutoProperty SourceVideo;
    AutoProperty SourceCamera;
#if KLAK_NDI_AVAILABLE
    AutoProperty NdiReceiver;
#endif

    AutoProperty SourceUrl;
    AutoProperty DeviceName;
    AutoProperty DeviceResolution;
    AutoProperty DeviceFrameRate;

    #endregion

    #region Webcam helpers

    void ChangeWebcam(string name)
    {
        serializedObject.Update();
        DeviceName.Target.stringValue = name;
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

        EditorGUILayout.PropertyField(SourceType);

        EditorGUI.indentLevel++;

        var type = (ImageSourceType)SourceType.Target.enumValueIndex;

        if (type == ImageSourceType.Texture)
            EditorGUILayout.PropertyField(SourceTexture, Labels.Asset);

        if (type == ImageSourceType.Video)
            EditorGUILayout.PropertyField(SourceVideo, Labels.Asset);

        if (type == ImageSourceType.Camera)
            EditorGUILayout.PropertyField(SourceCamera, Labels.Camera);

#if KLAK_NDI_AVAILABLE
        if (type == ImageSourceType.Ndi)
            EditorGUILayout.PropertyField(NdiReceiver, Labels.NdiReceiver);
#endif

        if (type == ImageSourceType.TextureUrl ||
            type == ImageSourceType.VideoUrl)
            EditorGUILayout.PropertyField(SourceUrl, Labels.URL);

        if (type == ImageSourceType.Webcam)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(DeviceName, Labels.DeviceName);
            var rect = EditorGUILayout.GetControlRect(false, GUILayout.Width(60));
            if (EditorGUI.DropdownButton(rect, Labels.Select, FocusType.Keyboard))
                ShowDeviceSelector(rect);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(DeviceResolution, Labels.Resolution);
            EditorGUILayout.PropertyField(DeviceFrameRate, Labels.FrameRate);
        }

        EditorGUI.indentLevel--;

#if !KLAK_NDI_AVAILABLE
        if (type == ImageSourceType.Ndi)
            EditorGUILayout.HelpBox(Labels.NdiError, MessageType.Error);
#endif

        EditorGUILayout.PropertyField(OutputDestination, Labels.Destination);
        if (OutputDestination.Target.objectReferenceValue == null)
            EditorGUILayout.PropertyField(OutputResolution);

        serializedObject.ApplyModifiedProperties();
    }

    #endregion
}

} // namespace Klak.TestTools
