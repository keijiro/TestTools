using UnityEditor;
using UnityEngine;

namespace Klak.TestTools {

[CustomEditor(typeof(ImageSource))]
sealed class ImageSourceEditor : Editor
{
    AutoProperty _webcamName;
    AutoProperty _webcamResolution;
    AutoProperty _webcamFrameRate;
    AutoProperty _outputResolution;

    void OnEnable() => AutoProperty.Scan(this);

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_webcamName);
        EditorGUILayout.PropertyField(_webcamResolution);
        EditorGUILayout.PropertyField(_webcamFrameRate);
        EditorGUILayout.PropertyField(_outputResolution);

        serializedObject.ApplyModifiedProperties();
    }
}

} // namespace Klak.TestTools
