public class BlurRenderPassManager//�������ܿ��ǣ����е�ͼƬֻ��ʹ����ͬ��Pass������ģ����Pass��Ĭ��ֵ���������һ��Mono�ű���ͨ��Instance�������еĲ���
{
    static BlurRenderPassManager m_Instance;
    public static BlurRenderPassManager Instance { get { if (m_Instance == null) { m_Instance = new BlurRenderPassManager(); } return m_Instance; } }

    public void Register(BlurRenderPass pass)
    {

    }
}
