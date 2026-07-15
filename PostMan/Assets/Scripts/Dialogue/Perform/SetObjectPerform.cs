using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Dialogue
{
    public class SetObjectPerform : DialoguePerform
    {
        private SetObjectDataProvider objectData;

        public override void Perform()
        {
            Debug.Log("设置了位置");
            objectData.target.SetActive(objectData.active);
            objectData.target.transform.position = objectData.position;
        }


        public override void ReceiveData(IPerformDataProvider data)
        {
            objectData = data as SetObjectDataProvider;
        }


    }
}