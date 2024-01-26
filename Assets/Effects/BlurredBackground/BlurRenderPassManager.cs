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
    /// <summary>
    /// 缓存当前活跃模糊背景，并判断是否需要开启Pass
    /// </summary>
    /// <param name="background">模糊背景</param>
    public void Register(BlurredBackground background)
    {
        if (background == null) return;
        if (!m_BlurredBackgrounds.Contains(background))
            m_BlurredBackgrounds.Add(background);
        ProcessPass();
    }
    /// <summary>
    /// 移除非活跃模糊背景，并判断是否需要开启Pass
    /// </summary>
    /// <param name="background">模糊背景</param>
    public void Unregister(BlurredBackground background)
    {
        if (background == null) return;
        if (!m_BlurredBackgrounds.Contains(background)) return;
        m_BlurredBackgrounds.Remove(background);
        ProcessPass();
    }
    /// <summary>
    /// 判断是否有活跃的模糊背景，如果有则开启Pass，如果无则关闭Pass
    /// </summary>
    void ProcessPass()
    {
        if (m_BlurredBackgrounds.Count > 0)
        {
            if (m_IsPassRunning) return;
            m_IsPassRunning = true;
            RenderPipelineManager.beginCameraRendering += BeginCameraRendering;
        }
        else
        {
            m_IsPassRunning = false;
            RenderPipelineManager.beginCameraRendering -= BeginCameraRendering;
            if (m_BlurRenderPass != null)
            {
                m_BlurRenderPass.ReleaseRT();
                m_BlurRenderPass = null;
            }
        }
    }
    void BeginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (camera == null || !camera.isActiveAndEnabled || !camera.CompareTag("MainCamera")) return;
        var data = camera.GetUniversalAdditionalCameraData();
        if (data == null) return;
        data.scriptableRenderer.EnqueuePass(BlurRenderPass);
    }
    /// <summary>
    /// 标记所有活跃的模糊背景顶点为脏
    /// </summary>
    public void SetAllImageVerticesDirty()
    {
        foreach (var background in m_BlurredBackgrounds)
        {
            background.SetVerticesDirty();
        }
    }
}