using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.UI
{
    //-----------------------------------------------------------------Image-----------------------------------------------------------------//
    public partial class BlurredBackground : Image
    {
        BlurredBackgroundRenderPass m_RenderPass;
        protected override void Awake()
        {
            base.Awake();
            CreatePass();
            material = new Material(Shader.Find("UPR Performant Effect/Blurred Background/Blurred Background"));
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            CreatePass();
            RenderPipelineManager.beginCameraRendering += beginCameraRendering;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            RenderPipelineManager.beginCameraRendering -= beginCameraRendering;
            if (m_RenderPass != null)
                m_RenderPass.ReleaseRT();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            RenderPipelineManager.beginCameraRendering -= beginCameraRendering;
            if (m_RenderPass != null)
                m_RenderPass.ReleaseRT();
        }
        void beginCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if (camera == null || !camera.isActiveAndEnabled || !camera.CompareTag("MainCamera")) return;
            var data = camera.GetUniversalAdditionalCameraData();
            if (data == null) return;
            if (m_RenderPass != null)
                data.scriptableRenderer.EnqueuePass(m_RenderPass);
        }
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            base.OnPopulateMesh(toFill);
        }
#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            CreatePass();
            material = new Material(Shader.Find("UPR Performant Effect/Blurred Background/Blurred Background"));
        }
#endif
        void CreatePass()
        {
            if (m_RenderPass == null)
            {
                m_RenderPass = new BlurredBackgroundRenderPass();
                m_RenderPass.renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;
            }
        }
    }
    //-----------------------------------------------------------------Image-----------------------------------------------------------------//

    //------------------------------------------------------------------Pass------------------------------------------------------------------//
    public class BlurredBackgroundRenderPass : ScriptableRenderPass
    {
        const float renderScale = 0.3f;
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
        RenderTexture m_BlurredBackgroundRenderTexture;
        public RenderTexture BlurredBackgroundRenderTexture
        {
            get
            {
                if (m_BlurredBackgroundRenderTexture == null)
                {
                    m_BlurredBackgroundRenderTexture = new RenderTexture((int)(Screen.width * renderScale), (int)(Screen.height * renderScale), 0);
                    m_BlurredBackgroundRenderTexture.filterMode = FilterMode.Bilinear;
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
                    m_BlurredBackgroundRenderTextureTemp = new RenderTexture((int)(Screen.width * renderScale), (int)(Screen.height * renderScale), 0);
                    m_BlurredBackgroundRenderTextureTemp.filterMode = FilterMode.Bilinear;
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
                    m_BlurredBackgroundRTHandle = RTHandles.Alloc(BlurredBackgroundRenderTexture);
                return m_BlurredBackgroundRTHandle;
            }
        }
        RTHandle m_BlurredBackgroundRTHandleTemp;
        RTHandle BlurredBackgroundRTHandleTemp
        {
            get
            {
                if (m_BlurredBackgroundRTHandleTemp == null)
                    m_BlurredBackgroundRTHandleTemp = RTHandles.Alloc(BlurredBackgroundRenderTextureTemp);
                return m_BlurredBackgroundRTHandleTemp;
            }
        }
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            base.OnCameraSetup(cmd, ref renderingData);
            ConfigureInput(ScriptableRenderPassInput.Color);
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
#if UNITY_EDITOR
            if (renderingData.cameraData.cameraType == CameraType.SceneView
                || renderingData.cameraData.cameraType == CameraType.Preview)
                return;
#endif

            var source = renderingData.cameraData.renderer.cameraColorTargetHandle;

            CommandBuffer cmd = CommandBufferPool.Get("Blurred Background Pass");
            cmd.Clear();

            Blit(cmd, source, BlurredBackgroundRTHandle);
            Blit(cmd, BlurredBackgroundRTHandle, BlurredBackgroundRTHandleTemp, BlurredBackgroundMaterial, 0);
            Blit(cmd, BlurredBackgroundRTHandleTemp, BlurredBackgroundRTHandle, BlurredBackgroundMaterial, 1);

            cmd.SetGlobalTexture("_BlurredBackgroundRT", BlurredBackgroundRTHandle);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            CommandBufferPool.Release(cmd);
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
    //------------------------------------------------------------------Pass------------------------------------------------------------------//
}