using UnityEditor;
using UnityEngine;
#if KLAK_NDI_AVAILABLE
using Klak.Ndi;
#endif

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
        public static Label NdiName = "NDI Name";
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

    AutoProperty _sourceUrl;
    AutoProperty _sourceName;
    AutoProperty _deviceResolution;
    AutoProperty _deviceFrameRate;

    #endregion

    #region Webcam helpers

    void ChangeWebcam(string name)
    {
        serializedObject.Update();
        _sourceName.Target.stringValue = name;
        serializedObject.ApplyModifiedProperties();
    }

    void ShowWebcamSelector(Rect rect)
    {
        var menu = new GenericMenu();
        foreach (var dev in WebCamTexture.devices)
            menu.AddItem(new GUIContent(dev.name), false, () => ChangeWebcam(dev.name));
        menu.DropDown(rect);
    }

    #endregion

    #region NDI helpers

    #if KLAK_NDI_AVAILABLE
    void ChangeNdi(string name)
    {
        serializedObject.Update();
        _sourceName.Target.stringValue = name;
        serializedObject.ApplyModifiedProperties();
    }

    void ShowNdiSelector(Rect rect)
    {
        var menu = new GenericMenu();
        foreach (var name in NdiFinder.sourceNames)
            menu.AddItem(new GUIContent(name), false, () => ChangeNdi(name));
        menu.DropDown(rect);
    }
    #endif

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
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.DelayedTextField(_sourceName, Labels.NdiName);
            var rect = EditorGUILayout.GetControlRect(false, GUILayout.Width(60));
            if (EditorGUI.DropdownButton(rect, Labels.Select, FocusType.Keyboard))
                ShowNdiSelector(rect);
            EditorGUILayout.EndHorizontal();
        }
        #endif

        if (type == ImageSourceType.TextureUrl ||
            type == ImageSourceType.VideoUrl)
            EditorGUILayout.DelayedTextField(_sourceUrl, Labels.URL);

        if (type == ImageSourceType.Webcam)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.DelayedTextField(_sourceName, Labels.DeviceName);
            var rect = EditorGUILayout.GetControlRect(false, GUILayout.Width(60));
            if (EditorGUI.DropdownButton(rect, Labels.Select, FocusType.Keyboard))
                ShowWebcamSelector(rect);
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
