using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadTitle : MonoBehaviour
{
    // Start is called before the first frame update
    //emm,临时脚本
    public bool load;
    private void Awake()
    {
        if (load)
        {
            SceneManager.LoadScene(1, LoadSceneMode.Additive); 
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
