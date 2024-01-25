using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.UI
{
    public partial class BlurredBackground : Image
    {
        protected override void Awake()
        {
            base.Awake();
            SetMaterial();
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            BlurRenderPassManager.Instance.Register(this);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            BlurRenderPassManager.Instance.Unregister(this);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            BlurRenderPassManager.Instance.Unregister(this);
        }
#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            SetMaterial();
        }
#endif
        void SetMaterial()
        {
            material = new Material(Shader.Find("UPR Performant Effect/Blurred Background/Blurred Background"));
        }
    }
}