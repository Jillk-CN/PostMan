using UnityEngine;

namespace PostMan.Trigger
{
    public class SetObjectVisibleTrigger : MonoBehaviour
    {
        [SerializeField]
        private bool visible;
        [SerializeField]
        private bool triggerOnce = true;
        [SerializeField]
        private bool canTrigger = true;
        [SerializeField]
        private GameObject[] objects;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")&&canTrigger)
            {
                foreach (var obj in objects)
                {
                    obj.SetActive(visible);
                }
                if (triggerOnce) 
                {
                    this.canTrigger = false;
                } 
            }
        }
    }
}

