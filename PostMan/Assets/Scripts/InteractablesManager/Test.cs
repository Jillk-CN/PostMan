using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 按空格键在两个场景之间切换的测试脚本（物体跨场景保留）
/// </summary>
public class SceneSwitchTest : MonoBehaviour
{
    [Header("场景设置")]
    [Tooltip("场景1的名称")]
    [SerializeField] private string sceneA = "Scene1";
    
    [Tooltip("场景2的名称")]
    [SerializeField] private string sceneB = "Scene2";

    [Header("切换场景2时应用的状态清单")]
    [Tooltip("进入场景2时自动应用的交互物状态清单")]
    [SerializeField] private InteractablesProcessListSO processListForSceneB;

    [Header("调试")]
    [SerializeField] private bool showLog = true;

    private string currentSceneName;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchScene();
        }
    }

    private void SwitchScene()
    {
        string targetScene = currentSceneName == sceneA ? sceneB : sceneA;

        if (!Application.CanStreamedLevelBeLoaded(targetScene))
        {
            Debug.LogError($"[SceneSwitchTest] 场景 '{targetScene}' 不存在！请检查场景名称是否正确，并确保已添加到 Build Settings 中。");
            return;
        }

        if (showLog)
        {
            Debug.Log($"[SceneSwitchTest] {currentSceneName} → {targetScene}");
        }

        SceneManager.LoadScene(targetScene);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentSceneName = scene.name;

        // 切换到场景B时应用状态清单
        // 注意：管理器内部会处理时序 — 如果缓存尚未构建，会自动延迟到缓存构建完成后应用
        if (scene.name == sceneB && processListForSceneB != null)
        {
            if (showLog)
            {
                Debug.Log($"[SceneSwitchTest] 应用状态清单: {processListForSceneB.name}");
            }
            InteractableManager.Instance.ApplyState(processListForSceneB);
        }

        if (showLog)
        {
            Debug.Log($"[SceneSwitchTest] 当前场景: {currentSceneName}");
        }
    }
}
