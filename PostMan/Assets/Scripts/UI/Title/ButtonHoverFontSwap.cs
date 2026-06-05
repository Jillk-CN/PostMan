using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PostMan.UI
{
    /// <summary>
    /// 鼠标悬停时切换按钮 TMP_Text 字体，离开时恢复默认字体。
    /// 不做动画过渡，即时切换。
    /// </summary>
    [RequireComponent(typeof(UGUIEventListener))]
    public class ButtonHoverFontSwap : MonoBehaviour
    {
        [Header("字体配置")]
        [Tooltip("要切换字体的目标 TMP_Text")]
        [SerializeField] private TMP_Text _label;

        [Tooltip("默认（非悬停）状态下使用的字体")]
        [SerializeField] private TMP_FontAsset _defaultFont;

        [Tooltip("鼠标悬停时使用的字体")]
        [SerializeField] private TMP_FontAsset _hoverFont;

        private UGUIEventListener _listener;

        private void Awake()
        {
            _listener = GetComponent<UGUIEventListener>();
        }

        private void OnEnable()
        {
            _listener.PointerEnter += OnEnter;
            _listener.PointerExit  += OnExit;
        }

        private void OnDisable()
        {
            _listener.PointerEnter -= OnEnter;
            _listener.PointerExit  -= OnExit;
        }

        // 悬停进入 → 切换为悬停字体
        private void OnEnter(UGUIEventListener sender, PointerEventData eventData)
        {
            if (_label != null && _hoverFont != null)
                _label.font = _hoverFont;
        }

        // 悬停离开 → 恢复默认字体
        private void OnExit(UGUIEventListener sender, PointerEventData eventData)
        {
            if (_label != null && _defaultFont != null)
                _label.font = _defaultFont;
        }
    }
}
