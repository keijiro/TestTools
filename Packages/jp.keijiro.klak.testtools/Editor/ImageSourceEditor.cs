using UnityEditor;
using UnityEngine;

namespace Klak.TestTools {
    /*

[CustomEditor(typeof(ImageSource))]
sealed class ImageSourceEditor : Editor
{
    static class Labels
    {
        public static Label Asset = "Asset";
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

    AutoProperty SourceType;

    AutoProperty SourceTexture;
    AutoProperty SourceTextureUrl;

    AutoProperty SourceVideo;
    AutoProperty SourceVideoUrl;

    AutoProperty _webcamName;
    AutoProperty _webcamResolution;
    AutoProperty _webcamFrameRate;

    AutoProperty _camera;

#if KLAK_NDI_AVAILABLE
    AutoProperty _ndiReceiver;
#endif

    AutoProperty OutputTexture;
    AutoProperty OutputResolution;

    void OnEnable() => AutoProperty.Scan(this);

    void ChangeWebcam(string name)
    {
        serializedObject.Update();
        _webcamName.Target.stringValue = name;
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

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(SourceType);

        EditorGUI.indentLevel++;

        var type = (ImageSourceType)SourceType.Target.enumValueIndex;

        if (type == ImageSourceType.Texture)
        {
            EditorGUILayout.PropertyField(SourceTexture, Labels.Asset);
            if (SourceTexture.Target.objectReferenceValue == null)
                EditorGUILayout.PropertyField(SourceTextureUrl, Labels.URL);
        }

        if (type == ImageSourceType.Video)
        {
            EditorGUILayout.PropertyField(SourceVideo, Labels.Asset);
            if (SourceVideo.Target.objectReferenceValue == null)
                EditorGUILayout.PropertyField(SourceVideoUrl, Labels.URL);
        }

        if (type == ImageSourceType.Webcam)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(_webcamName, Labels.DeviceName);
            var rect = EditorGUILayout.GetControlRect(false, GUILayout.Width(60));
            if (EditorGUI.DropdownButton(rect, Labels.Select, FocusType.Keyboard))
                ShowDeviceSelector(rect);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(_webcamResolution, Labels.Resolution);
            EditorGUILayout.PropertyField(_webcamFrameRate, Labels.FrameRate);
        }

#if KLAK_NDI_AVAILABLE
        if (type == ImageSourceType.Ndi)
            EditorGUILayout.PropertyField(_ndiReceiver, Labels.NdiReceiver);
#endif

        if (type == ImageSourceType.Camera)
            EditorGUILayout.PropertyField(_camera);

        EditorGUI.indentLevel--;

#if !KLAK_NDI_AVAILABLE
        if (type == ImageSourceType.Ndi)
            EditorGUILayout.HelpBox(Labels.NdiError, MessageType.Error);
#endif

        EditorGUILayout.PropertyField(OutputTexture);
        if (OutputTexture.Target.objectReferenceValue == null)
            EditorGUILayout.PropertyField(OutputResolution);

        serializedObject.ApplyModifiedProperties();
    }
}
*/

} // namespace Klak.TestTools
