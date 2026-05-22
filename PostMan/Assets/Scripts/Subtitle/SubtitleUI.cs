using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class SubtitleUI : MonoBehaviour
{
    public static SubtitleUI Instance;  //全局单例
    public TextMeshProUGUI ContentBox;  //获取文本框对象
    //public GameObject BackGround;  //获取UI背景
    [SerializeField] private SubtitleData SubtitleData;  //获取字幕表
    private Coroutine currentCoroutine;
    

    [Header("字幕UI设置")]
    [Tooltip("UI延迟退出时间（不包含UI淡出时间）")]
    public float DelayOutTime;  //UI关闭延迟时长

    [Tooltip("淡入时间")]
    public float FadeInTime;    //淡入时间
    [Tooltip("淡出时间")]
    public float FadeOutTime;   //淡出时间
    private float FadeTimer;    //淡入淡出计时器
    [Tooltip("CanvasGroup组件")]
    public CanvasGroup canvasGroup;

    
    [Header("打字机效果设置")]
    [Tooltip("打字间隔时长")]
    public float InterTime = 0.05f;

    [Tooltip("打字音效播放几率")]
    [Range(0 , 1f)]
    public float typeSoundRate;

    [Tooltip("AudioSource组件")]
    public AudioSource audioSource;

    [Tooltip("打字音效列表")]
    public List<AudioClip> audioClips;


    [Header("语言设置")]
    public Language languageSet = Language.CN;

    void Awake()
    {
        //全局单例实现
        if(Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(Instance);
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        //空检测
        if(ContentBox == null)
        {
            Debug.LogError("字幕系统：文本框UI为空！");
            return;
        }
        if(SubtitleData == null)
        {
            Debug.LogError("字幕系统：字幕表为空！");
            return;
        }
        if(audioSource == null)
        {
            Debug.LogError("字幕系统：声音组件为空！");
            return;
        }

        //设置UI不阻挡点击
        canvasGroup.blocksRaycasts = false;

        //隐藏UI
        canvasGroup.alpha = 0;
        ContentBox.gameObject.SetActive(false);
        //BackGround.SetActive(false);
    }

    void Update()
    {
        //测试
        if(Input.GetKeyDown(KeyCode.Space))
        {
            TypeSubtitle(1 , 3);
        }
    }

    //开启UI
    private IEnumerator OpenUI(float FadeInTime)
    {
        //重置淡入计时器
        FadeTimer = 0f;
        ContentBox.gameObject.SetActive(true);
        //BackGround.SetActive(true);

        //逐帧淡入UI
        while(FadeTimer < FadeInTime)
        {
            FadeTimer += Time.deltaTime;

            canvasGroup.alpha = Mathf.Lerp(0f , 1f , FadeTimer/FadeInTime);
            
            yield return null;
        }

        //强制UI完全显示
        canvasGroup.alpha = 1f;
    }

    //关闭UI
    private IEnumerator CloseUI(float FadeOutTime)
    {
        //重置淡出计时器
        FadeTimer = 0f;

        //逐帧淡出UI
        while(FadeTimer < FadeOutTime)
        {
            FadeTimer += Time.deltaTime;

            canvasGroup.alpha = Mathf.Lerp(1f , 0f , FadeTimer/FadeOutTime);
            
            yield return null;
        }

        //强制UI完全隐藏
        canvasGroup.alpha = 0f;
        ContentBox.gameObject.SetActive(false);
        //BackGround.SetActive(false);

        currentCoroutine = null;

        //重置 UI 状态
        canvasGroup.alpha = 0f;
        ContentBox.maxVisibleCharacters = 0;
        ContentBox.text = "";
        currentCoroutine = null;
    }

    /// <summary>
    /// 播放包括StartID和EndID之间所有ID对应的字幕
    /// </summary>
    /// <param name="StartID"></param>
    /// <param name="EndID"></param>
    public void TypeSubtitle(int StartID , int EndID)
    {
        if(currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);

            //重置 UI 状态
            canvasGroup.alpha = 0f;
            ContentBox.maxVisibleCharacters = 0;
            ContentBox.text = "";
            currentCoroutine = null;
        }
        currentCoroutine = StartCoroutine(TypeText(StartID , EndID));
    }

    /// <summary>
    /// 播放对应ID的字幕
    /// </summary>
    /// <param name="ID"></param>
    public void TypeSubtitle(int ID)
    {
        if(currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);

            //重置 UI 状态
            canvasGroup.alpha = 0f;
            ContentBox.maxVisibleCharacters = 0;
            ContentBox.text = "";
            currentCoroutine = null;
        }
        currentCoroutine = StartCoroutine(TypeText(ID , ID));
    }

    //打字
    private IEnumerator TypeText(int StartID , int EndID)
    {
        //开启UI
        yield return StartCoroutine(OpenUI(FadeInTime));

        int ConLength;

        for(int currentIndex = StartID - 1 ; currentIndex <= EndID - 1 ; currentIndex ++)
        {
            //重置字幕框UI显示内容
            ContentBox.text = "";

            //重置当前字幕要打印的内容
            string CurrentSubtitle = "";    

            //按语言设置获取对应字幕内容
            if(languageSet == Language.CN)    //中文
            {
                CurrentSubtitle = SubtitleData.subtitleList[currentIndex].ContentCN;
            }
            else if(languageSet == Language.EN)    //英文
            {
                CurrentSubtitle = SubtitleData.subtitleList[currentIndex].ContentEN;
            }

            //获取当前需要打印的字幕的长度
            ConLength = CurrentSubtitle.Length;
            
            //把当前需要打印的字幕内容赋值给UI
            ContentBox.text = CurrentSubtitle;

            //设置字幕UI显示字数为 0
            ContentBox.maxVisibleCharacters = 0;
            
            //逐个打印文字
            for(int c = 0 ; c <= ConLength ; c++)
            {
                ContentBox.maxVisibleCharacters = c;

                //随机播放打字音效
                if(UnityEngine.Random.Range(0f , 1f) >= 1-typeSoundRate && audioClips != null)
                {
                    //随机在音效列表中选取一个音效播放
                    AudioClip currentClip = audioClips[UnityEngine.Random.Range(0 , audioClips.Count)];
                    if(currentClip != null)
                    {
                        audioSource.PlayOneShot(currentClip);
                    }
                }

                //打字延迟
                yield return new WaitForSeconds(InterTime);
            }

            //延迟后进行下一段字幕打印（延迟间隔由字幕信息决定）
            yield return new WaitForSeconds(SubtitleData.subtitleList[currentIndex].DelayTime);
        }

        //延迟退出
        yield return new WaitForSeconds(DelayOutTime);

        //关闭UI
        yield return StartCoroutine(CloseUI(FadeOutTime));

    }

    public enum Language
    {
        CN,
        EN
    }
}
