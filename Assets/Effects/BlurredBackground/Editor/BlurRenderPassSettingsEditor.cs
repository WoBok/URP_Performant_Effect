using UnityEditor;
[CustomEditor(typeof(BlurRenderPassParmaSetter))]
[CanEditMultipleObjects]
public class BlurRenderPassParmaSetterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var settings = target as BlurRenderPassParmaSetter;
        settings.BlurSize = EditorGUILayout.IntSlider("Blur Size", settings.BlurSize, 1, 10);
        settings.DownSample = EditorGUILayout.IntSlider("Down Sample", settings.DownSample, 0, 9);
        settings.Iterations = EditorGUILayout.IntSlider("Iterations", settings.Iterations, 1, 10);
    }
}