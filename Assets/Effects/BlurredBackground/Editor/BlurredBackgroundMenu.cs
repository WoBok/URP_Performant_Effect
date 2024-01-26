using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BlurredBackgroundMenu : MonoBehaviour
{
    [MenuItem("GameObject/UI/Blurred Background")]
    static void CreateImage()
    {
        var activeTransform = Selection.activeTransform;
        GameObject canvasGo = null;
        bool hasCanvas = false;
        if (activeTransform != null)
        {
            Canvas canvas = activeTransform.GetComponent<Canvas>();
            if (canvas != null)
                hasCanvas = true;
            else
            {
                var parent = activeTransform.parent;
                while (parent != null)
                {
                    var currentCanvas = parent.GetComponent<Canvas>();
                    if (currentCanvas != null)
                    {
                        hasCanvas = true;
                        break;
                    }
                    parent = parent.parent;
                }
            }
        }

        if (hasCanvas)
            canvasGo = activeTransform.gameObject;

        if (!hasCanvas)
        {
            canvasGo = new GameObject("Canvas");
            canvasGo.AddComponent<Canvas>();
            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();
            canvasGo.transform.SetParent(activeTransform);
        }

        var blurredImageGo = new GameObject("Blurred Background");
        blurredImageGo.transform.SetParent(canvasGo.transform, false);
        blurredImageGo.AddComponent<BlurredBackground>();

        Selection.activeTransform = blurredImageGo.transform;

        Undo.RegisterCreatedObjectUndo(canvasGo, "Create " + blurredImageGo.name);
    }
}