using UnityEngine;

/// <summary>
/// VHS 滤镜预设 ScriptableObject。
/// 在 Inspector 中配置所有效果参数，运行时只读，不写入。
/// 右键 Assets > Create > VHSFilter > VHS Preset 创建资产。
/// </summary>
[CreateAssetMenu(menuName = "VHSFilter/VHS Preset", fileName = "VHSPreset")]
public class VHSPresetSO : ScriptableObject
{
    [Header("默认启用状态")]
    [Tooltip("游戏启动时是否默认开启 VHS 滤镜")]
    public bool enabledByDefault = true;

    [Header("效果参数")]
    [Tooltip("在 Inspector 中直接调节所有 VHS 效果参数")]
    public VHSSettings settings = VHSSettings.Default;
}