using UnityEngine;
using static UnityEngine.Experimental.Rendering.RayTracingAccelerationStructure;

public class BlurRenderPassParmaSetter : MonoBehaviour
{
    BlurRenderPassSettings passSettings;
    int m_BlurSize = 2;
    public int BlurSize
    {
        get => m_BlurSize;
        set
        {
            if (value != m_BlurSize)
            {
                m_BlurSize = value;
                passSettings.BlurSize = value;
                BlurRenderPassManager.Instance.BlurRenderPass.BlurSize = value;
                BlurRenderPassManager.Instance.SetAllImageVerticesDirty();
            }
        }
    }
    int m_DownSample = 7;
    public int DownSample
    {
        get => m_DownSample;
        set
        {
            if (value != m_DownSample)
            {
                m_DownSample = value;
                passSettings.DownSample = value;
                BlurRenderPassManager.Instance.BlurRenderPass.DownSample = value;
                BlurRenderPassManager.Instance.SetAllImageVerticesDirty();
            }
        }
    }
    int m_Iterations = 1;
    public int Iterations
    {
        get => m_Iterations;
        set
        {
            if (value != m_Iterations)
            {
                m_Iterations = value;
                passSettings.Iterations = value;
                BlurRenderPassManager.Instance.BlurRenderPass.Iterations = value;
                BlurRenderPassManager.Instance.SetAllImageVerticesDirty();
            }
        }
    }
    void Awake()
    {
        passSettings = BlurRenderPassSettings.GetSettings();
        BlurSize = passSettings.BlurSize;
        DownSample = passSettings.DownSample;
        Iterations = passSettings.Iterations;
    }
}