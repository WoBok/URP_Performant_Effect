using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlurPortionRenderPass : ScriptableRenderPass
{
    //思路：1. Renderer中在Filtering选项Opaque Layer Mask取消Blurred Background
    //          2. 在Pass抓取当前屏幕
    //          3. 在Pass中多次渲染Blurred Background到相机RT

    float downSample = 0.3f;

    Material m_BlurMaterial;
    Material BlurMaterial
    {
        get
        {
            if (m_BlurMaterial == null)
            {
                m_BlurMaterial = new Material(Shader.Find("URP Shader/ABc"));
            }
            return m_BlurMaterial;
        }
    }

    RenderTexture m_CameraColorRT;
    RenderTexture CameraColorRT
    {
        get
        {
            if (m_CameraColorRT == null)
            {
                m_CameraColorRT = new RenderTexture((int)(Screen.width * downSample), (int)(Screen.height * downSample), 0);
            }
            return m_CameraColorRT;
        }
    }
    RTHandle m_CameraColorRTH;
    RTHandle CameraColorRTH
    {
        get
        {
            if (m_CameraColorRTH == null)
            {
                m_CameraColorRT = RTHandles.Alloc(CameraColorRT);
            }
            return m_CameraColorRTH;
        }
    }
    DrawingSettings m_DrawingSettings;
    FilteringSettings m_FilteringSettings;
    public BlurPortionRenderPass(LayerMask layerMask)
    {
        m_FilteringSettings = new FilteringSettings(RenderQueueRange.opaque, layerMask);
        renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        profilingSampler = new ProfilingSampler("BlurPortionRenderPass");
    }
    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        base.OnCameraSetup(cmd, ref renderingData);
        ConfigureInput(ScriptableRenderPassInput.Color);
    }
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
#if UNITY_EDITOR
        if (renderingData.cameraData.cameraType == CameraType.Preview
            || renderingData.cameraData.cameraType == CameraType.SceneView)
            return;
#endif
        //var cameraCTH = renderingData.cameraData.renderer.cameraColorTargetHandle;
        //if (cameraCTH != null)
        //{
        //    if (cameraCTH.rt != null)
        //    {
        //        var cmd = CommandBufferPool.Get("BlurPortionRenderPass");
        //        cmd.Clear();
        //        Blit(cmd, cameraCTH, CameraColorRTH);
        //        cmd.SetGlobalTexture("_CameraColorRT", CameraColorRTH);
        //        context.ExecuteCommandBuffer(cmd);
        //        cmd.Clear();
        //        CommandBufferPool.Release(cmd);
        //    }
        //}

        m_DrawingSettings = CreateDrawingSettings(new ShaderTagId("UniversalForward"), ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
        m_DrawingSettings.overrideMaterial = BlurMaterial;

        m_DrawingSettings.overrideMaterialPassIndex = 0;
        context.DrawRenderers(renderingData.cullResults, ref m_DrawingSettings, ref m_FilteringSettings);
        //m_DrawingSettings.overrideMaterialPassIndex = 1;
        //context.DrawRenderers(renderingData.cullResults, ref m_DrawingSettings, ref m_FilteringSettings);
        Debug.Log("Excute...");
    }
}