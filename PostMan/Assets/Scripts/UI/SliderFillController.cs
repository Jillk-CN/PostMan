using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderFillController : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    private void Start()
    {
        var slider = GetComponent<Slider>();
        // 确保 Image 是 Filled 裁剪模式（也可以在 Inspector 里提前设好）
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        fillImage.fillOrigin = 0; // Left
        fillImage.fillAmount = slider.value;

        // 每次 Slider 值变化时同步
        slider.onValueChanged.AddListener(val => fillImage.fillAmount = val);
    }
}