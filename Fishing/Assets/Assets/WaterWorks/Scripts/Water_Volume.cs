using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Water_Volume : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        public RTHandle source; // RTHandle 사용

        private Material _material;

        private RTHandle tempRenderTarget;
        private RTHandle tempRenderTarget2;

        public CustomRenderPass(Material mat)
        {
            _material = mat;

            // RTHandle 초기화
            tempRenderTarget = RTHandles.Alloc("_TemporaryColourTexture", name: "_TemporaryColourTexture");
            tempRenderTarget2 = RTHandles.Alloc("_TemporaryDepthTexture", name: "_TemporaryDepthTexture");
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            // 임시 렌더 타겟 설정
            ConfigureTarget(tempRenderTarget, tempRenderTarget2);
            ConfigureClear(ClearFlag.All, Color.black);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType != CameraType.Reflection)
            {
                CommandBuffer commandBuffer = CommandBufferPool.Get("Water_Volume_RenderPass");

                // Blit으로 렌더 타겟 처리
                Blit(commandBuffer, source, tempRenderTarget, _material);
                Blit(commandBuffer, tempRenderTarget, source);

                context.ExecuteCommandBuffer(commandBuffer);
                CommandBufferPool.Release(commandBuffer);
            }
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            // 메모리 관리는 자동화되지만 필요시 여기서 해제 가능
        }

        public void Dispose()
        {
            tempRenderTarget?.Release();
            tempRenderTarget2?.Release();
        }
    }

    [System.Serializable]
    public class _Settings
    {
        public Material material = null;
        public RenderPassEvent renderPass = RenderPassEvent.AfterRenderingSkybox;
    }

    public _Settings settings = new _Settings();

    CustomRenderPass m_ScriptablePass;

    public override void Create()
    {
        if (settings.material == null)
        {
            settings.material = (Material)Resources.Load("Water_Volume");
        }

        m_ScriptablePass = new CustomRenderPass(settings.material);
        m_ScriptablePass.renderPassEvent = settings.renderPass;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (m_ScriptablePass != null)
        {
            m_ScriptablePass.source = renderer.cameraColorTargetHandle; // RTHandle 사용
            renderer.EnqueuePass(m_ScriptablePass);
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        m_ScriptablePass?.Dispose();
    }
}
