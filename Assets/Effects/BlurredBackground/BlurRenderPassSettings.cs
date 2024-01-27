using System.IO;
using UnityEditor;
using UnityEngine;

public class BlurRenderPassSettings : ScriptableObject
{
    public int BlurSize;
    public int DownSample;
    public int Iterations;
    public static BlurRenderPassSettings GetSettings()
    {
        var passSettings = Resources.Load<BlurRenderPassSettings>("BlurredBackgroundSettings");
        if (passSettings == null)
        {
            passSettings = CreateInstance<BlurRenderPassSettings>();
            passSettings.BlurSize = 2;
            passSettings.DownSample = 7;
            passSettings.Iterations = 1;

            string path = Application.dataPath + "/Resources";
            if (!Directory.Exists(path))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
                AssetDatabase.CreateAsset(passSettings, "Assets/Resources/BlurredBackgroundSettings.asset");
            }
            else
            {
                AssetDatabase.CreateAsset(passSettings, "Assets/Resources/BlurredBackgroundSettings.asset");
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        return passSettings;
    }
}