using UnityEditor;
using UnityEngine;

namespace Klak.TestTools {

[CustomEditor(typeof(ImageSource))]
sealed class ImageSourceEditor : Editor
{
    static class Labels
    {
        public static Label Asset = "Asset";
        public static Label DeviceName = "Device Name";
        public static Label FrameRate = "Frame Rate";
        public static Label Resolution = "Resolution";
        public static Label Select = "Select";
        public static Label URL = "URL";
    }

    AutoProperty _sourceType;

    AutoProperty _texture;
    AutoProperty _textureUrl;

    AutoProperty _video;
    AutoProperty _videoUrl;

    AutoProperty _webcamName;
    AutoProperty _webcamResolution;
    AutoProperty _webcamFrameRate;

    AutoProperty _outputTexture;
    AutoProperty _outputResolution;

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

        EditorGUI.BeginDisabledGroup(Application.isPlaying);

        EditorGUILayout.PropertyField(_sourceType);

        EditorGUI.indentLevel++;

        var type = (ImageSource.SourceType)_sourceType.Target.enumValueIndex;

        if (type == ImageSource.SourceType.Texture)
        {
            EditorGUILayout.PropertyField(_texture, Labels.Asset);
            if (_texture.Target.objectReferenceValue == null)
                EditorGUILayout.PropertyField(_textureUrl, Labels.URL);
        }

        if (type == ImageSource.SourceType.Video)
        {
            EditorGUILayout.PropertyField(_video, Labels.Asset);
            if (_video.Target.objectReferenceValue == null)
                EditorGUILayout.PropertyField(_videoUrl, Labels.URL);
        }

        if (type == ImageSource.SourceType.Webcam)
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

        EditorGUI.indentLevel--;

        EditorGUILayout.PropertyField(_outputTexture);
        if (_outputTexture.Target.objectReferenceValue == null)
            EditorGUILayout.PropertyField(_outputResolution);

        EditorGUI.EndDisabledGroup();

        serializedObject.ApplyModifiedProperties();
    }
}

} // namespace Klak.TestTools
