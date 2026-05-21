using System;

namespace PostMan.Player
{
    /// <summary>
    /// 可交互物体的接口,所有可交互物体必须实现这个接口
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// 每个可交互物体说明能否交互
        /// </summary>
        public bool CanInteract { get; set; }
        /// <summary>
        /// 交互优先级,如果多个可交互的脚本挂在同一个物体上,
        /// 优先级大的先交互
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// 与玩家交互
        /// </summary>
        /// <param name="player"></param>
        public void InteractWith(PlayerInteractor player);
    }
}
