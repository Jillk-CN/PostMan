using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PostMan.UI
{
    /// <summary>
    /// 实现查看类似于查看图片的基本功能(暂时不需要做成Image的子类)
    /// 该物体的锚点必须锚定在正中间
    /// </summary>
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(UGUIEventListener))]
    public class ImageViewer : MonoBehaviour
    {
        private Image image;
        private RectTransform rectTransform;
        /// <summary>
        /// 假定父物体要指定边界
        /// </summary>
        private RectTransform boundaryRectTransform;
        private UGUIEventListener listener;
        private Vector2 normalSize;

        [Header("缩放设置")]
        [SerializeField]
        private float zoomSpeed = 0.1f;
        [SerializeField]
        private float scaleMin = 0.5f;
        [SerializeField]
        private float scaleMax = 1.3f;//注意,不要太大,以至于父物体容不下
        private float currentScale;//由于不想修改localScale,需要记录当前的缩放倍数


        [Header("拖拽设置")]
        [SerializeField]
        private float dragSpeed = 1f;

        // 记录鼠标按下的点
        private Vector2 previousMousePosition;

        private void Awake()
        {
            rectTransform = this.transform as RectTransform;
            boundaryRectTransform = this.transform.parent as RectTransform;

            normalSize = rectTransform.rect.size;
            currentScale = 1f;
            
            listener = this.GetComponent<UGUIEventListener>();

            this.image = this.GetComponent<Image>();
        }
        private void OnEnable()
        {
            listener.PointerDown += RecordMousePosition;
            listener.Drag += FollowPointer;
            listener.Scroll += Zoom;
            Restore();
        }
        private void OnDisable()
        {
            listener.PointerDown -= RecordMousePosition;
            listener.Drag -= FollowPointer;
            listener.Scroll -= Zoom;
        }

        public void SetImage(Sprite sprite)
        {
            image.sprite = sprite;
            image.SetNativeSize();
            normalSize = rectTransform.rect.size;
        }

        private void RecordMousePosition(UGUIEventListener sender,PointerEventData eventData)
        {
            // Debug.LogFormat(" PointerDown -pointerPosition {0} cam {1} transformedPos {2} canvasCam {3}",
            //     eventData.position, eventData.pressEventCamera,transPos
            //     , canvas.worldCamera);

            //eventData.position 事件传过来鼠标的屏幕坐标
            previousMousePosition = eventData.position;
        }
        private void FollowPointer(UGUIEventListener sender,PointerEventData eventData)
        {
            //Debug.LogWarningFormat("anchored Pos {0}", thisRectTransform.anchoredPosition);

            Vector2 currentMousePos=eventData.position;
            Vector2 delta = currentMousePos - previousMousePosition;
            //thisRectTransform.anchoredPosition 相对于锚点的位置,
            //如果锚点在一起没有拉伸,就是Editor里的POS X ,POS Y
            rectTransform.anchoredPosition += delta * dragSpeed;

            previousMousePosition = currentMousePos; 

            ClampPosition();
        }
        /// <summary>
        /// 缩放物体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventData"></param>
        private void Zoom(UGUIEventListener sender,PointerEventData eventData)
        {
            // 获取鼠标在图片上的局部坐标,由于是Overlay的Canvas,相机其实可以简单的传null
            Vector2 mouseLocalPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, eventData.position, eventData.enterEventCamera,
                out mouseLocalPos);

            //缩放
            float previousScale = this.currentScale;
            float scrollDelta = eventData.scrollDelta.y * zoomSpeed;
            float newScale = Mathf.Clamp(previousScale + scrollDelta, scaleMin, scaleMax);
            this.currentScale = newScale;

            //只修改宽高,不改scale
            Vector2 newSize = normalSize * newScale;
            SetSize(newSize.y,newSize.x);

            //让图片反向移动(相对)缩放的长度,让鼠标保持在缩放之前的图形里的位置
            rectTransform.anchoredPosition -=
                mouseLocalPos * (newScale / previousScale) - mouseLocalPos;

            ClampPosition();

        }

        /// <summary>
        /// 限制图片位置,要求图片的宽高不能超过父物体Boundary的RectTransform的范围
        /// </summary>
        private void ClampPosition()
        {
            Vector2 clampedAnchoredPosition = rectTransform.anchoredPosition;

            //这段代码要求物体的锚点在正中心,而且scale保持1
            //然后父物体的在锚点的左右两边的宽度,上下两边的高度是等分的
            //注意,要的是宽高,不是长方形的坐标
            float yboundary = boundaryRectTransform.rect.size.y / 2;
            float xboundary = boundaryRectTransform.rect.size.x / 2;
            float halfHeight = rectTransform.rect.size.y / 2;
            float halfWidth = rectTransform.rect.size.x / 2;

           // Debug.LogFormat("{0} : {1} {2} : {3} {4} : {5} {6} : {7}",
           //     nameof(xboundary), xboundary, nameof(yboundary), yboundary,
           //     nameof(halfHeight), halfHeight, nameof(halfWidth), halfWidth);

            // 计算可移动的范围
            Vector2 max = new Vector2(0, yboundary - halfHeight);
            max.x = xboundary - halfWidth;
            Vector2 min = -max;
            
            clampedAnchoredPosition.x = Mathf.Clamp(clampedAnchoredPosition.x, min.x, max.x);
            clampedAnchoredPosition.y = Mathf.Clamp(clampedAnchoredPosition.y, min.y, max.y);
            rectTransform.anchoredPosition = clampedAnchoredPosition;
        }

        /// <summary>
        /// 设置宽高
        /// </summary>
        /// <param name="height"></param>
        /// <param name="width"></param>
        private void SetSize(float height,float width)
        {
            rectTransform.SetSizeWithCurrentAnchors
                (RectTransform.Axis.Horizontal, width);
            rectTransform.SetSizeWithCurrentAnchors
                (RectTransform.Axis.Vertical, height);

        }
        /// <summary>
        /// 复位
        /// </summary>
        private void Restore()
        {
            currentScale = 1;
            SetSize(normalSize.y, normalSize.x);
            rectTransform.anchoredPosition = Vector2.zero;
        }
    }
}
