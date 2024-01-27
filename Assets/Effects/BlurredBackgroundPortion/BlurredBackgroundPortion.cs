using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class BlurredBackgroundPortion : Image
{
    public LayerMask layerMask;
    BlurPortionRenderPass renderPass;
    protected override void Awake()
    {
        base.Awake();
        layerMask =LayerMask.GetMask("Water");
        renderPass = new BlurPortionRenderPass(layerMask);
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        RenderPipelineManager.beginCameraRendering += BeginCameraRendering;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        RenderPipelineManager.beginCameraRendering -= BeginCameraRendering;
    }
    private void BeginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (camera == null || !camera.isActiveAndEnabled || !camera.CompareTag("MainCamera")) return;
        var data = camera.GetUniversalAdditionalCameraData();
        if (data == null) return;
        data.scriptableRenderer.EnqueuePass(renderPass);
    }
}