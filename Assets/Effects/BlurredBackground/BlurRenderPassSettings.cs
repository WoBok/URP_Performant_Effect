using UnityEngine;

public class BlurRenderPassSettings : MonoBehaviour
{
    int m_BlurSize = 2;
    public int BlurSize
    {
        get => m_BlurSize;
        set
        {
            if (value != m_BlurSize)
            {
                m_BlurSize = value;
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
                BlurRenderPassManager.Instance.BlurRenderPass.DownSample = (10 - value) / 10f;
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
                BlurRenderPassManager.Instance.BlurRenderPass.Iterations = value;
                BlurRenderPassManager.Instance.SetAllImageVerticesDirty();
            }
        }
    }
    void Awake()
    {
        BlurSize = m_BlurSize;
        DownSample = m_DownSample;
        Iterations = m_Iterations;
    }
}