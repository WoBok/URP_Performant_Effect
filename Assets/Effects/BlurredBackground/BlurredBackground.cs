using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.UI
{
    public partial class BlurredBackground : Image
    {
        BlurRenderPass m_RenderPass;
        int m_BlurSize = 1;
        public int blurSize
        {
            get => m_BlurSize;
            set
            {
                if (value != m_BlurSize)
                {
                    m_BlurSize = value;
                    m_RenderPass.BlurSize = value;
                    SetVerticesDirty();
                }
            }
        }

        int m_DownSample = 7;
        public int downSample
        {
            get => m_DownSample;
            set
            {
                if (value != m_DownSample)
                {
                    m_DownSample = value;
                    m_RenderPass.DownSample = (10 - value) / 10f;
                    SetVerticesDirty();
                }
            }
        }

        int m_Iterations = 1;
        public int iterations
        {
            get => m_Iterations;
            set
            {
                if (value != m_Iterations)
                {
                    m_Iterations = value;
                    m_RenderPass.Iterations = value;
                    SetVerticesDirty();
                }
            }
        }

        //Rect¸Ä±äÊ±

        protected override void Awake()
        {
            base.Awake();
            CreatePass();
            SetMaterial();
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
            Debug.Log("beginCameraRendering");
        }
#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            CreatePass();
            SetMaterial();
        }
#endif
        void CreatePass()
        {
            if (m_RenderPass == null)
            {
                m_RenderPass = new BlurRenderPass();
                m_RenderPass.renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;
            }
        }
        void SetMaterial()
        {
            material = new Material(Shader.Find("UPR Performant Effect/Blurred Background/Blurred Background"));
        }
    }
}