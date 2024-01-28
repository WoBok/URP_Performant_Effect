using UnityEngine;

public class BlurRenderPassParmaSetter : MonoBehaviour
{
    BlurRenderPassSettings m_PassSettings;
    BlurRenderPassSettings PassSettings
    {
        get
        {
            if (m_PassSettings == null)
                m_PassSettings = BlurRenderPassSettings.GetSettings();
            return m_PassSettings;
        }
    }
    public int BlurSize
    {
        get => PassSettings.BlurSize;
        set
        {
            if (value != PassSettings.BlurSize)
            {
                PassSettings.BlurSize = value;
                BlurRenderPassManager.Instance.BlurRenderPass.BlurSize = value;
                BlurRenderPassManager.Instance.SetAllImageVerticesDirty();
            }
        }
    }
    public int DownSample
    {
        get => PassSettings.DownSample;
        set
        {
            if (value != PassSettings.DownSample)
            {
                PassSettings.DownSample = value;
                BlurRenderPassManager.Instance.BlurRenderPass.DownSample = (10 - value) / 10f;
                BlurRenderPassManager.Instance.SetAllImageVerticesDirty();
            }
        }
    }
    public int Iterations
    {
        get => PassSettings.Iterations;
        set
        {
            if (value != PassSettings.Iterations)
            {
                PassSettings.Iterations = value;
                BlurRenderPassManager.Instance.BlurRenderPass.Iterations = value;
                BlurRenderPassManager.Instance.SetAllImageVerticesDirty();
            }
        }
    }
    void Awake()
    {
        SetParma();
    }
#if UNITY_EDITOR
    void Reset()
    {
        SetParma();
        BlurRenderPassManager.Instance.BlurRenderPass.ReleaseRT();
    }
#endif
    void SetParma()
    {
        BlurSize = PassSettings.BlurSize;
        DownSample = PassSettings.DownSample;
        Iterations = PassSettings.Iterations;
    }
}