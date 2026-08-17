using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PostMan.Localization
{
    public class LocalizationTest : MonoBehaviour
    {
        [SerializeField]
        private string key;
        [SerializeField]
        private LocalizationManager.TableName table;
        private TextMeshProUGUI text;

        private void Start()
        {
            this.text = this.GetComponent<TextMeshProUGUI>(); 
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard.spaceKey.wasPressedThisFrame)
            {
                Show();
            }

        }
        private void Show()
        {
            this.text.text = LocalizationManager.Instance.GetLocalizedString(table, key);
            SubtitleContent[] result1 = LocalizationManager.Instance.GetLocalizedSubtitles(text.text);
            string title;
            string content;
            LocalizationManager.Instance.GetLocalizedTaskInfo
                (text.text, out title, out content);
        }
    }
}
