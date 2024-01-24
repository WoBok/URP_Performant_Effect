public class BlurRenderPassManager//出于性能考虑，所有的图片只能使用相同的Pass处理背景模糊，Pass有默认值，可以添加一个Mono脚本，通过Instance管理所有的参数
{
    static BlurRenderPassManager m_Instance;
    public static BlurRenderPassManager Instance { get { if (m_Instance == null) { m_Instance = new BlurRenderPassManager(); } return m_Instance; } }

    public void Register(BlurRenderPass pass)
    {

    }
}
