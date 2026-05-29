using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// VHS 滤镜渲染 Pass。
/// 负责分配临时 RTHandle、执行 Blit，不持有任何业务参数。
/// 参数已由 VHSFilterEffect 提前写入 Material。
/// </summary>
internal sealed class VHSFilterPass : ScriptableRenderPass
{
    // ─────────────────────────────────────────────
    // 私有状态
    // ─────────────────────────────────────────────

    private readonly ProfilingSampler _sampler = new ProfilingSampler("VHSFilter");
    private readonly Material _material;
    private RTHandle _sourceHandle;
    private RTHandle _tempHandle;

    // ─────────────────────────────────────────────
    // 构造
    // ─────────────────────────────────────────────

    /// <summary>
    /// 创建 VHS 滤镜渲染 Pass。
    /// </summary>
    /// <param name="material">VHS Blit 材质（由 VHSFilterEffect 创建）。</param>
    /// <param name="evt">渲染注入时机。</param>
    internal VHSFilterPass(Material material, RenderPassEvent evt)
    {
        _material = material;
        renderPassEvent = evt;
    }

    // ─────────────────────────────────────────────
    // 公开接口
    // ─────────────────────────────────────────────

    /// <summary>设置源 RT（相机颜色目标）。</summary>
    internal void SetSource(RTHandle source) => _sourceHandle = source;

    // ─────────────────────────────────────────────
    // ScriptableRenderPass 生命周期
    // ─────────────────────────────────────────────

    /// <summary>
    /// 分配与相机同尺寸的临时 RT（无深度、无 MSAA）。
    /// </summary>
    public override void Configure(CommandBuffer cmd,
                                   RenderTextureDescriptor cameraTextureDescriptor)
    {
        var desc = cameraTextureDescriptor;
        desc.depthBufferBits = 0;   // 不需要深度
        desc.msaaSamples = 1;       // 不需要 MSAA

        RenderingUtils.ReAllocateIfNeeded(
            ref _tempHandle, desc,
            FilterMode.Bilinear, TextureWrapMode.Clamp,
            name: "_VHSFilterTemp");
    }

    /// <summary>
    /// 执行两步 Blit：源→临时（应用 VHS Shader）→源（回写）。
    /// 两步 Blit 是必要的，URP 14 Forward 下 cameraColorTargetHandle 不能同时读写。
    /// </summary>
    public override void Execute(ScriptableRenderContext context,
                                 ref RenderingData renderingData)
    {
        if (_material == null || _sourceHandle == null) return;

        CommandBuffer cmd = CommandBufferPool.Get();
        using (new ProfilingScope(cmd, _sampler))
        {
            // 1. 将相机颜色 Blit 到临时 RT（应用 VHS Shader Pass 0）
            Blitter.BlitCameraTexture(cmd, _sourceHandle, _tempHandle, _material, 0);
            // 2. 将处理结果 Blit 回相机颜色 RT（直通拷贝）
            Blitter.BlitCameraTexture(cmd, _tempHandle, _sourceHandle);
        }

        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }

    // ─────────────────────────────────────────────
    // 资源释放
    // ─────────────────────────────────────────────

    /// <summary>释放临时 RTHandle。</summary>
    internal void Dispose()
    {
        _tempHandle?.Release();
    }
}
