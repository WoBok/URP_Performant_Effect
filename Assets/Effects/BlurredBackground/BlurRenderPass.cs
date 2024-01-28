using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class BlurRenderPass : ScriptableRenderPass
{
    float m_DownSample = 0.3f;
    public float DownSample
    {
        get => m_DownSample;
        set
        {
            m_DownSample = value;
            ReleaseRT();
        }
    }
    int m_Iterations = 1;
    public int Iterations
    {
        get => m_Iterations;
        set => m_Iterations = value;
    }
    Material m_BlurredBackgroundMaterial;
    Material BlurredBackgroundMaterial
    {
        get
        {
            if (m_BlurredBackgroundMaterial == null)
                m_BlurredBackgroundMaterial = new Material(Shader.Find("UPR Performant Effect/Blurred Background/Blur Texture"));
            return m_BlurredBackgroundMaterial;
        }
    }
    public int BlurSize
    {
        set => BlurredBackgroundMaterial.SetInt("_BlurSize", value);
    }
    RenderTexture m_BlurredBackgroundRenderTexture;
    public RenderTexture BlurredBackgroundRenderTexture
    {
        get
        {
            if (m_BlurredBackgroundRenderTexture == null)
            {
                CreateRenderTexture(ref m_BlurredBackgroundRenderTexture);
            }
            return m_BlurredBackgroundRenderTexture;
        }
    }
    RenderTexture m_BlurredBackgroundRenderTextureTemp;
    RenderTexture BlurredBackgroundRenderTextureTemp
    {
        get
        {
            if (m_BlurredBackgroundRenderTextureTemp == null)
            {
                CreateRenderTexture(ref m_BlurredBackgroundRenderTextureTemp);
            }
            return m_BlurredBackgroundRenderTextureTemp;
        }
    }
    RTHandle m_BlurredBackgroundRTHandle;
    RTHandle BlurredBackgroundRTHandle
    {
        get
        {
            if (m_BlurredBackgroundRTHandle == null)
            {
                CreateRTHandle(ref m_BlurredBackgroundRTHandle, BlurredBackgroundRenderTexture);
            }
            return m_BlurredBackgroundRTHandle;
        }
    }
    RTHandle m_BlurredBackgroundRTHandleTemp;
    RTHandle BlurredBackgroundRTHandleTemp
    {
        get
        {
            if (m_BlurredBackgroundRTHandleTemp == null)
            {
                CreateRTHandle(ref m_BlurredBackgroundRTHandleTemp, BlurredBackgroundRenderTextureTemp);
            }
            return m_BlurredBackgroundRTHandleTemp;
        }
    }
    BlurRenderPassSettings passSettings;
    public BlurRenderPass()
    {
        renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;
        SetParmas();
    }
    void SetParmas()
    {
        passSettings = BlurRenderPassSettings.GetSettings();
        BlurSize = passSettings.BlurSize;
        DownSample = (10 - passSettings.DownSample) / 10f;
        Iterations = passSettings.Iterations;
    }
    void CreateRenderTexture(ref RenderTexture renderTexture)
    {
        renderTexture = new RenderTexture((int)(Screen.width * m_DownSample), (int)(Screen.height * m_DownSample), 0);
        renderTexture.filterMode = FilterMode.Bilinear;
    }
    void CreateRTHandle(ref RTHandle rtHandle, RenderTexture renderTexture)
    {
        rtHandle = RTHandles.Alloc(renderTexture);
    }
    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        base.OnCameraSetup(cmd, ref renderingData);
        ConfigureInput(ScriptableRenderPassInput.Color);
    }
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
#if UNITY_EDITOR
        if (renderingData.cameraData.cameraType == CameraType.Preview)
            return;
#endif

        var source = renderingData.cameraData.renderer.cameraColorTargetHandle;

        if (source != null)
        {
            CommandBuffer cmd = CommandBufferPool.Get("Blurred Background Pass");
            cmd.Clear();

            Blit(cmd, source, BlurredBackgroundRTHandle);

            for (int i = 0; i < Iterations; i++)
            {
                Blit(cmd, BlurredBackgroundRTHandle, BlurredBackgroundRTHandleTemp, BlurredBackgroundMaterial, 0);
                Blit(cmd, BlurredBackgroundRTHandleTemp, BlurredBackgroundRTHandle, BlurredBackgroundMaterial, 1);
            }
            cmd.SetGlobalTexture("_BlurredBackgroundRT", BlurredBackgroundRTHandle);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            CommandBufferPool.Release(cmd);
        }
    }
    public void ReleaseRT()
    {
        if (m_BlurredBackgroundRTHandle != null)
        {
            m_BlurredBackgroundRTHandle.Release();
            m_BlurredBackgroundRTHandle = null;
        }
        if (m_BlurredBackgroundRTHandleTemp != null)
        {
            m_BlurredBackgroundRTHandleTemp.Release();
            m_BlurredBackgroundRTHandleTemp = null;
        }
        if (m_BlurredBackgroundRenderTexture != null)
        {
            m_BlurredBackgroundRenderTexture.Release();
            m_BlurredBackgroundRenderTexture = null;
        }
        if (m_BlurredBackgroundRenderTextureTemp != null)
        {
            m_BlurredBackgroundRenderTextureTemp.Release();
            m_BlurredBackgroundRenderTextureTemp = null;
        }
    }
}