using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PostMan.UI
{
    //当初写的时候自定义了委托类型,就这样先
    public delegate void PointerEventHandler(UGUIEventListener sender, PointerEventData eventData);
    public delegate void BaseEventHandler(UGUIEventListener sender, BaseEventData eventData);
    public delegate void AxisEventHandler(UGUIEventListener sender, AxisEventData eventData);
    /// <summary>
    /// UI事件监听器,提供访问所有UGUI的EventSystem的交互事件的接口,
    /// 如果需要特殊的交互,比如双击,长按等等需要自行实现,
    /// </summary>
    public class UGUIEventListener : MonoBehaviour, 
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerClickHandler,
        IInitializePotentialDragHandler,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IDropHandler,
        IScrollHandler,
        IUpdateSelectedHandler,
        ISelectHandler,
        IDeselectHandler,
        IMoveHandler,
        ISubmitHandler,
        ICancelHandler
    {
        #region 交互事件
        public event PointerEventHandler PointerEnter;
        public event PointerEventHandler PointerExit;

        public event PointerEventHandler PointerClick;
        public event PointerEventHandler PointerUp;
        public event PointerEventHandler PointerDown;

        public event PointerEventHandler BeginDrag;
        public event PointerEventHandler Drag;
        public event PointerEventHandler EndDrag;

        public event PointerEventHandler Drop;
        public event PointerEventHandler Scroll;
        public event PointerEventHandler InitializePotentialDrag;

        public event BaseEventHandler Select;
        public event BaseEventHandler Deselect;
        public event BaseEventHandler UpdateSelected;

        public event BaseEventHandler Cancel;
        public event BaseEventHandler Submit;

        public event AxisEventHandler Move;
        #endregion

        #region 交互接口的实现
        //用显式实现防止外部直接调用

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            PointerClick?.Invoke(this, eventData);
        }
        void IPointerDownHandler. OnPointerDown(PointerEventData eventData)
        {
            if (PointerDown != null) PointerDown(this, eventData);
        }
        void IPointerUpHandler. OnPointerUp(PointerEventData eventData)
        {
            if (PointerUp != null)
            {
                PointerUp(this, eventData);
            }
        }

        void ISelectHandler.OnSelect(BaseEventData eventData)
        {
            if (Select==null )
            {
                return;
            }
            Select(this, eventData);
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (PointerEnter==null)
            {
                return;
            }
            PointerEnter(this,eventData);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            if (PointerExit==null)
            {
                return;
            }
            PointerExit(this,eventData);
        }

        void IInitializePotentialDragHandler.OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (InitializePotentialDrag==null)
            {
                return;
            }
            InitializePotentialDrag(this, eventData);
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            if (BeginDrag==null)
            {
                return;
            }
            BeginDrag(this, eventData);
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (Drag==null)
            {
                return;
            }
            Drag(this, eventData);
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            if (EndDrag==null)
            {
                return;
            }
            EndDrag(this, eventData);
        }

        void IDropHandler.OnDrop(PointerEventData eventData)
        {
            if (Drop==null)
            {
                return;
            }
            Drop(this, eventData);
        }

        void IScrollHandler.OnScroll(PointerEventData eventData)
        {
            if (Scroll==null)
            {
                return;
            }
            Scroll(this, eventData);
        }

        void IUpdateSelectedHandler.OnUpdateSelected(BaseEventData eventData)
        {
            if (UpdateSelected==null)
            {
                return;
            }
            UpdateSelected(this, eventData);
        }

        void ICancelHandler.OnCancel(BaseEventData eventData)
        {
            if (Cancel==null)
            {
                return;
            }
            Cancel(this, eventData);
        }

        void ISubmitHandler.OnSubmit(BaseEventData eventData)
        {
            if (Submit==null)
            {
                return;
            }
            Submit(this,eventData);
        }

        void IDeselectHandler.OnDeselect(BaseEventData eventData)
        {
            if (Deselect==null)
            {
                return;
            }
            Deselect(this,eventData);
        }

        void IMoveHandler.OnMove(AxisEventData eventData)
        {
            if (Move==null)
            {
                return;
            }
            Move(this,eventData);
        }

        #endregion

        /*
        public static UIEventListener GetListener(Transform objectTF)
        {
            UIEventListener listener = objectTF.GetComponent<UIEventListener>();
            if (listener == null)
            {
                listener = objectTF.gameObject.AddComponent<UIEventListener>();
            }
            return listener;
        }
         */
    }
}