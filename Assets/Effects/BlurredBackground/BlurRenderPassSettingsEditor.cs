using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(BlurRenderPassSettings))]
[CanEditMultipleObjects]
public class BlurRenderPassSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var settings = target as BlurRenderPassSettings;
        settings.BlurSize = EditorGUILayout.IntSlider("Blur Size", settings.BlurSize, 0, 2);
    }
}