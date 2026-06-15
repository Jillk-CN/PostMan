namespace PostMan.AudioSystem
{
    /// <summary>
    /// 音轨标识符枚举。
    /// 每个值对应 AudioManager.tracks 列表中的一条 AudioTrackConfig。
    /// 新增子音轨时，只需在此处添加枚举值，并在 Inspector 的 Tracks 列表中追加一条配置。
    /// </summary>
    public enum AudioTrackId
    {
        /// <summary>背景音乐主轨</summary>
        BGM = 0,

        /// <summary>玩家音效轨</summary>
        Player = 1,

        /// <summary>通用音效轨</summary>
        FX = 2,

        /// <summary>UI音效轨</summary>
        UI = 3,
    }
}
