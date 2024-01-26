using UnityEngine;

public class BlurPortionRenderPass : MonoBehaviour
{
    //思路：1. Renderer中在Filtering选项Opaque Layer Mask取消Blurred Background
    //          2. 在Pass抓取当前屏幕
    //          3. 在Pass中多次渲染Blurred Background到相机RT
}