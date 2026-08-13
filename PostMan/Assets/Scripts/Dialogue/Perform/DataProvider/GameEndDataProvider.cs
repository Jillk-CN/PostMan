using PostMan.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Dialogue
{
    public class GameEndDataProvider : MonoBehaviour,IPerformDataProvider
    {
        [SerializeField]
        private int priority;
        [SerializeField]
        private int dialogueIndex;
        public int TargetDialogueIndex => dialogueIndex;
        public int Priority => priority;
        public string PerformName => nameof(GameEndPerform);

        public List<string> scenesToLoad;
        public List<string> scenesToUnload;
        [HideInInspector]
        public string gameEndText;
        [SerializeField]
        private string localizationKey;
        public float delaySwitchTime = 1f;
        public float blackDuration = 1f;
        private void Start()
        {
            this.gameEndText = LocalizationManager.Instance.GetLocalizedString
                (LocalizationManager.TableName.ReadingMaterials, localizationKey);
        }
    }
}
