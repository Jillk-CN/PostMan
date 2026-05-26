using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Player
{
    public class ShowInteractPrompt : MonoBehaviour, ISelectable
    {
        private bool selected;
        public bool Selected
        {
            get => selected;
            set => selected = value;
        }
        [SerializeField]
        private bool canSelect=true;
        public bool CanSelect 
        { 
            get => canSelect;
            set
            {
                canSelect = value;
                if (!canSelect)
                {
                    Deselect();
                }
            }
        }

        public void Deselect()
        {
            selected = false;
            InteractPrompt.Instance.Hide();
        }

        public void Select()
        {
            selected = true;
            InteractPrompt.Instance.Show();
        }
    }
}
