using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubtitleTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        //测试
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SubtitleUI.Instance.TypeSubtitle("stl_d2_3");
        }
    }
}
