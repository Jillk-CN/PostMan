using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Player
{
    /// <summary>
    /// 描边效果,在需要实现描边的物体上挂上这个脚本
    /// </summary>
    public class OutlineVisual : MonoBehaviour, ISelectable
    {
        [Tooltip("实现描边的插件,手动拖上去,一个模型所有的outline都放这里,外部就调用这个脚本就行了")]
        [SerializeField]
        private Outline[] outlineVisuals;
        private bool selected;
        public bool Selected
        {
            get => selected;
            set => selected = value;
        }

        private void Start()
        {
            this.selected = false;
            SetOutline(false);
        }
        public void Deselect()
        {
            this.selected = false;
            SetOutline(false);
        }

        public void Select()
        {
            this.selected = true;
            SetOutline(true);
        }

        /// <summary>
        /// 设置描边的显隐
        /// </summary>
        /// <param name="visible">显示或隐藏</param>
        public void SetOutline(bool visible)
        {
            foreach (var outline in outlineVisuals)
            {
                outline.enabled = visible;
            }
        }

    }
}
