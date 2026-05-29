using PostMan.Common;
using PostMan.InputManagement;
using UnityEngine;

namespace PostMan.UI
{
    /// <summary>
    /// VHS 滤镜调节面板。由 VHSToggleInputSource 驱动切换显隐，
    /// 显示时解锁鼠标并禁用玩家输入。
    /// </summary>
    public class VHSPanel : MonoSingleton<VHSPanel>
    {
        [Tooltip("VHSFilterController 引用，用于在面板激活前同步 Toggle 状态")]
        [SerializeField] private VHSFilterController _filterController;

        private void Awake()
        {
            this.gameObject.SetActive(false);
        }

        /// <summary>显示 VHS 面板，解锁鼠标，禁用玩家输入</summary>
        public void Show()
        {
            // 先同步 Toggle 的 m_IsOn 与 _settings 一致，再激活面板。
            // 避免 Toggle.OnEnable 触发 onValueChanged 时传入与 _settings 不一致的值。
            _filterController?.SyncBeforeShow();
            this.gameObject.SetActive(true);
            GameInputManager.Instance.SetInputSystemSource<PauseInputSource>(false);
            GameInputManager.Instance.DisablePlayerAllInput();
            GameInputManager.Instance.ShowCursor();
        }

        /// <summary>隐藏 VHS 面板，锁定鼠标，恢复玩家输入</summary>
        public void Hide()
        {
            this.gameObject.SetActive(false);
            GameInputManager.Instance.SetInputSystemSource<PauseInputSource>(true);
            GameInputManager.Instance.EnablePlayerAllInput();
            GameInputManager.Instance.HideCursor();
        }

        /// <summary>根据当前状态切换显隐，由 VHSToggleInputSource 调用</summary>
        public void Toggle()
        {
            if (this.gameObject.activeSelf)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }
    }
}
