using UnityEngine.UI;
namespace UnityEditor.UI
{
    [CustomEditor(typeof(BlurredBackground))]
    [CanEditMultipleObjects]
    public class BlurredBackgroundEditor : GraphicEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            //var image = target as BlurredBackground;
            //if (image != null)
            //{
            //    image.blurSize = EditorGUILayout.IntSlider("Blur Size", image.blurSize, 1, 10);
            //    image.downSample = EditorGUILayout.IntSlider("Down Sample", image.downSample, 0, 9);
            //    image.iterations = EditorGUILayout.IntSlider("Iterations", image.iterations, 1, 10);
            //}
        }
    }
}