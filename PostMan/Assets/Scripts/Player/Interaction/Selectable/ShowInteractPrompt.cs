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
