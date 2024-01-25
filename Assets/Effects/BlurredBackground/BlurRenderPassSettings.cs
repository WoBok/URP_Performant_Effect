using UnityEngine;
using static UnityEngine.XR.XRDisplaySubsystem;

public class BlurRenderPassSettings : MonoBehaviour
{
    int m_BlurSize = 2;
    public int BlurSize
    {
        get => m_BlurSize;
        set
        {
            m_BlurSize = value;
            Debug.Log("m_BlurSize" + value);
        }
    }
    //int m_BlurSize = 1;
    //public int blurSize
    //{
    //    get => m_BlurSize;
    //    set
    //    {
    //        if (value != m_BlurSize)
    //        {
    //            m_BlurSize = value;
    //            m_RenderPass.BlurSize = value;
    //            SetVerticesDirty();
    //        }
    //    }
    //}

    //int m_DownSample = 7;
    //public int downSample
    //{
    //    get => m_DownSample;
    //    set
    //    {
    //        if (value != m_DownSample)
    //        {
    //            m_DownSample = value;
    //            m_RenderPass.DownSample = (10 - value) / 10f;
    //            SetVerticesDirty();
    //        }
    //    }
    //}

    //int m_Iterations = 1;
    //public int iterations
    //{
    //    get => m_Iterations;
    //    set
    //    {
    //        if (value != m_Iterations)
    //        {
    //            m_Iterations = value;
    //            m_RenderPass.Iterations = value;
    //            SetVerticesDirty();
    //        }
    //    }
    //}
}