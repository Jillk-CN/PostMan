using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Dialogue
{
    public class SwitchPhonePerform : DialoguePerform
    {
        private I_Telephone phone;
        public override void Perform()
        {
            if (phone.Holding())
            {
                phone.PutDown();
            }
            else
            {
                phone.PickUp();
            }
        }

        public override void ReceiveData(IPerformDataProvider data)
        {
            phone = (data as SwitchPhoneDataProvider).phone;
        }
    }
}
