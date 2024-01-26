namespace UnityEngine.UI
{
    public partial class BlurredBackground : Image
    {
        Camera cam;
        Bounds bounds;
        Plane[] planes;
        bool m_IsDisplay;
        protected override void Awake()
        {
            base.Awake();
            SetMaterial();
            SetBounds();
            cam = Camera.main;
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
            SetBounds();
        }
#endif
        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            SetBounds();
        }
        /// <summary>
        /// ��Ӳ���
        /// </summary>
        void SetMaterial()
        {
            material = new Material(Shader.Find("UPR Performant Effect/Blurred Background/Blurred Background"));
        }
        /// <summary>
        /// ���AABB��Χ��
        /// </summary>
        void SetBounds()
        {
            bounds = new Bounds();
            var rect = GetComponent<RectTransform>();
            if (rect != null)
            {
                var corners = new Vector3[4];
                rect.GetWorldCorners(corners);
                var boundsSize = corners[2] - corners[0];
                bounds.size = new Vector3(boundsSize.x, boundsSize.y, 0.1f);
            }
        }
        /// <summary>
        /// �жϴ�ģ�������Ƿ��������Ұ��
        /// </summary>
        /// <returns></returns>
        bool IsVisiableInCamera()
        {
            bounds.center = transform.position;
            if (cam != null)
                planes = GeometryUtility.CalculateFrustumPlanes(cam);
            if (GeometryUtility.TestPlanesAABB(planes, bounds))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// �ж��Ƿ�Ҫ��ʾ��ǰģ������
        /// </summary>
        void IsDisplay()
        {
            bool isVisiableInCamera = IsVisiableInCamera();
            if (isVisiableInCamera)
            {
                if (!m_IsDisplay)
                {
                    BlurRenderPassManager.Instance.Register(this);
                    m_IsDisplay = true;
                }
            }
            else
            {
                if (m_IsDisplay)
                {
                    BlurRenderPassManager.Instance.Unregister(this);
                    m_IsDisplay = false;
                }
            }
        }
        void Update()
        {
            IsDisplay();
        }
    }
}