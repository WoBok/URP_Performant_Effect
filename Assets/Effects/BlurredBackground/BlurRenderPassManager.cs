using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class BlurRenderPassManager
{
    static BlurRenderPassManager m_Instance;
    public static BlurRenderPassManager Instance { get { if (m_Instance == null) { m_Instance = new BlurRenderPassManager(); } return m_Instance; } }

    BlurRenderPass m_BlurRenderPass;
    public BlurRenderPass BlurRenderPass { get { if (m_BlurRenderPass == null) { m_BlurRenderPass = new BlurRenderPass(); } return m_BlurRenderPass; } }

    List<BlurredBackground> m_BlurredBackgrounds = new List<BlurredBackground>();
    bool m_IsPassRunning;

    public void Register(BlurredBackground background)
    {
        if (background == null) return;
        m_BlurredBackgrounds.Add(background);
        ProcessPass();
    }
    public void Unregister(BlurredBackground background)
    {
        if (background == null) return;
        if (!m_BlurredBackgrounds.Contains(background)) return;
        m_BlurredBackgrounds.Remove(background);
        ProcessPass();
    }
    void ProcessPass()
    {
        if (m_BlurredBackgrounds.Count > 0)
        {
            if (m_IsPassRunning) return;
            m_IsPassRunning = true;
            RenderPipelineManager.beginCameraRendering += beginCameraRendering;
        }
        else
        {
            m_IsPassRunning = false;
            RenderPipelineManager.beginCameraRendering -= beginCameraRendering;
        }
    }
    void beginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (camera == null || !camera.isActiveAndEnabled || !camera.CompareTag("MainCamera")) return;
        var data = camera.GetUniversalAdditionalCameraData();
        if (data == null) return;
        if (BlurRenderPass != null)
            data.scriptableRenderer.EnqueuePass(BlurRenderPass);
        Debug.Log("beginCameraRendering");
    }
    public void SetAllImageVerticesDirty()
    {
        foreach (var background in m_BlurredBackgrounds)
        {
            background.SetVerticesDirty();
        }
    }
}
