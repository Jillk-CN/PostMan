using System;
using PostMan.Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.InputSystem;
using System.Text.RegularExpressions;

namespace PostMan.Localization
{
    /// <summary>
    /// 本地化管理器(单例),基于LeanLocalization插件来提供更改语言的方法和获取本地化资源的接口
    /// 这个类假定本地化资源在运行时不会增加/减少
    /// </summary>
    public class LocalizationManager : MonoSingleton<LocalizationManager>
    {
        /// <summary>
        /// 确保ProjectSettings里的顺序和这个是一样的
        /// </summary>
        public enum LocaleID
        {
            en=0,//中文
            zh//英文
        }

        #region 资源表管理类
        /// <summary>
        /// 本地化资源的表的名字,需要和资源表的名称一样,所有要加载的表要在这里记录名字
        /// </summary>
        public enum TableName
        {
            DialogueTable,//对话表 
            SubtitleTable,//字幕表
            TaskTable//任务内容表
        }
        /// <summary>
        /// 表格资源管理类,管理本地化资源
        /// </summary>
        /// <typeparam name="TTable">资源表的类型</typeparam>
        /// <typeparam name="TEntry">条目类型,务必保证于表类型对应</typeparam>
        private class LocalizedTableManager<TTable,TEntry> 
            where TTable:DetailedLocalizationTable<TEntry> 
            where TEntry:TableEntry
        {
            /// <summary>
            /// 要管理的资源表
            /// </summary>
            private Dictionary<LocaleID, Dictionary<TableName, TTable>> tables; 
            public LocalizedTableManager()
            {
                tables = new Dictionary<LocaleID, Dictionary<TableName, TTable>>();
            }
            /// <summary>
            /// 添加本地化资源表
            /// </summary>
            /// <param name="locale"></param>
            /// <param name="name"></param>
            /// <param name="table"></param>
            public void AddTable(LocaleID locale,TableName name,TTable table)
            {
                if (table==null)
                {
                    return;
                }
                if (!this.tables.ContainsKey(locale))
                {
                    this.tables.Add(locale, new Dictionary<TableName, TTable>());
                }
                this.tables[locale].Add(name, table);
            }
            /// <summary>
            /// 根据Locale、表名和条目名来获得本地化字符串 
            /// </summary>
            /// <param name="locale"></param>
            /// <param name="table"></param>
            /// <param name="entryKey"></param>
            /// <returns></returns>
            public TEntry GetEntry(LocaleID locale,TableName table,string entryKey)
            {
                if (!this.tables.ContainsKey(locale) || 
                    !this.tables[locale].ContainsKey(table))
                {
                    return null;    
                }
                return this.tables[locale][table].GetEntry(entryKey);
            }
        }
        #endregion
        #region 字符串解析类
        private class StringParser
        {
            private const char SPLIT_CHAR = '#';
            /// <summary>
            /// 解析字幕
            /// </summary>
            /// <returns></returns>
            public SubtitleContent[] GetSubtitle(string text)
            {
                if (string.IsNullOrEmpty(text))
                {
                    return null;
                }
                string[] subtitlesData = text.Split(SPLIT_CHAR);
                SubtitleContent[] result = new SubtitleContent[subtitlesData.Length];
                Match match;
                float delay = 0;
                string displaySubtitle;
                for (int i = 0; i < subtitlesData.Length; i++)
                {
                    delay = 0;
                    displaySubtitle = subtitlesData[i];
                    if ( (match = Regex.Match(subtitlesData[i],@"<(\d+(?:\.\d*)?)>$") ).Success )
                    {
                        delay = float.Parse(match.Groups[1].Value);
                        displaySubtitle = displaySubtitle.
                              Substring(0, match.Index);//丢弃末尾的<数字>
                    }
                    //还要赋值字符串
                    result[i] = new SubtitleContent() 
                    { 
                        DelayTime = delay,
                        ContentCN=displaySubtitle 
                    };
                }
                return result;
            }
            /// <summary>
            /// 获得任务信息,先简单地用out来传递结果,就不整一个类了
            /// </summary>
            /// <param name="text"></param>
            /// <param name="title"></param>
            /// <param name="content"></param>
            public void GetTaskInfo(string text,out string title,out string content )
            {
                if (string.IsNullOrEmpty(text))
                {
                    title = string.Empty;
                    content = string.Empty;
                }
                //不做检查
                string[] result = text.Split(SPLIT_CHAR);
                title = result[0];
                if (result.Length>1)
                {
                    content = result[1];
                }
                else
                {
                    content = string.Empty;
                }
            }
            public string[] GetDialogueInfo(string text)
            {
                if (string.IsNullOrEmpty(text))
                {
                    return null;
                }
                return text.Split(SPLIT_CHAR);
            }
        }
        private StringParser parser;
        #endregion
        private LocaleID currentLocale;
        private LocalizedTableManager<StringTable, StringTableEntry> stringTables;
        public event Action<LocaleID> LocaleChanged;


        //需要说明,如果有良好的设计的话,不要让这个管理器负责加载,需要迁移到资源管理的地方
        #region 初始化
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            parser = new StringParser();
            //等待本地化插件初始化完成(插件是异步初始化的)
            if (LocalizationSettings.InitializationOperation.IsDone)
            {
                LoadLocale();
                LoadAllStringTables(LocalizationSettings.InitializationOperation);
            }
            else
            {
                LocalizationSettings.InitializationOperation.Completed 
                    += LoadLocale;
                LocalizationSettings.InitializationOperation.Completed 
                    += LoadAllStringTables;
            }
        }
        //小项目可以开始的时候加载所有的本地化资源,大项目不行,这里就选择
        //开始时加载所有的资源
        private void LoadAllStringTables(AsyncOperationHandle<LocalizationSettings> handle)
        {
            this.stringTables = new LocalizedTableManager<StringTable, StringTableEntry>();
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                StartCoroutine(LoadStringTable
                       (LocaleID.zh));
                StartCoroutine(LoadStringTable
                       (LocaleID.en));
            }
            else
            {
                Debug.LogError("本地化插件初始化错误!");
            }
        }

        //加载一种语言的本地化资源
        private IEnumerator LoadStringTable(LocaleID locale)
        {
            AsyncOperationHandle loadOp = LocalizationSettings.StringDatabase.
                 GetAllTables
                 (LocalizationSettings.AvailableLocales.Locales[(int)locale]);
            yield return loadOp;
            InitStringTableManager(locale, loadOp.Result as IList<StringTable>);
        }
        private void InitStringTableManager(LocaleID locale,IList<StringTable> loadedTables)
        {
            if (loadedTables==null)
            {
                return;
            }
            StringTable result = null;
            foreach (TableName name in Enum.GetValues(typeof(TableName)))
            {
                result = loadedTables.FirstOrDefault
                      ((table) => table.name == string.Format("{0}_{1}",name,locale));
                if (result==null)
                {
                    continue;
                }
                this.stringTables.AddTable(locale, name, result);        
            }

        }
        #endregion

        //用来快速测试
#if DEBUG
        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard.hKey.wasPressedThisFrame)
            {
                if (currentLocale == LocaleID.zh)
                {
                    SetLocale(LocaleID.en);
                }
                else
                {
                    SetLocale(LocaleID.zh);
                }
            }
        }
#endif
        /* 
         
         */

        #region 更改语言的接口 
        public void SetLocale(LocaleID id)
        {
            //修改本地化设置并发布事件
            LocalizationSettings.SelectedLocale =
                LocalizationSettings.AvailableLocales.Locales[(int)id];
            this.currentLocale = id;
            //由于没有事件中心,先放这里先
            LocaleChanged?.Invoke(this.currentLocale);
            
            //EventCenter.Instance.Publish

            //GameEventBus.Publish<LocaleChangeEvent>
             //   (new LocaleChangeEvent { locale = this.currentLocale });
        }

        /// <summary>
        /// 获取当前的语言
        /// </summary>
        /// <returns></returns>
        public LocaleID GetLocale()
        {
            return currentLocale;
        }

        /// <summary>
        /// 获取语言名称
        /// </summary>
        /// <param name="locale"></param>
        /// <returns></returns>
        public string GetLocaleName(LocaleID locale)
        {
            //出于简单考虑,就不从stringTable里读取了,直接硬编码
            switch (locale)
            {
                case LocaleID.zh:
                    return "简体中文";
                    //break;
                case LocaleID.en:
                    return "English";
                    //break;
                default:
                    return string.Empty;
                    //break;
            }

        }
        /// <summary>
        /// 获取当前的语言名称
        /// </summary>
        /// <returns></returns>
        public string GetLocaleName()
        {
            return GetLocaleName(currentLocale); 
        }

        #endregion

        #region 获取本地化资源接口
        /// <summary>
        /// 获取本地化字符串,在调用这个方法前,确保加载完成
        /// </summary>
        /// <param name="table"></param>
        /// <param name="entryKey"></param>
        /// <returns>若失败,返回string.Empty</returns>
        public string GetLocalizedString(TableName table,string entryKey)
        {
            if (entryKey==null)
            {
                return string.Empty;
            }
            if (this.stringTables==null)
            {
                Debug.LogWarning("本地化资源未加载");
                return string.Empty;
            }
            StringTableEntry entry = this.stringTables.
                GetEntry(GetLocale(), table, entryKey);
            if (entry==null)
            {
                return string.Empty;
            }
            return entry.GetLocalizedString();
        }
        /// <summary>
        /// 获取字幕
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public SubtitleContent[] GetLocalizedSubtitles(string text)
        {
            return parser.GetSubtitle(text);
        }
        public void GetLocalizedTaskInfo(string text,out string title,out string content)
        {
            parser.GetTaskInfo(text, out title, out content); 
        }
        /*调用示例
        string str=
        LocalizationManager.Instance.GetLocalizedString(TableName.SubtitleTable, "");
        GetLocalizedSubtitles(str);
         */
        //缺少dialogue的
        #endregion

        //-------------下面这些做数据持久化的代码最好不写到这个类里--------------------------
        private const string LanguagePrefKey = "SelectedLanguage";
        /// <summary>
        /// 初始化语言。如果是首次进入游戏，根据系统地区选择默认语言；否则读取本地保存的设定。
        /// </summary>
        private void LoadLocale()
        {
            int savedLocale = 0;
            // 检查是否已经保存过语言设置（判断是否首次进入游戏）
            if (PlayerPrefs.HasKey(LanguagePrefKey))
            {
                savedLocale = PlayerPrefs.GetInt(LanguagePrefKey);
                SetLocale((LocaleID)savedLocale);
                return;
            }
            // 第一次进入游戏,根据系统语言选择
            if (Application.systemLanguage == SystemLanguage.Chinese ||
                Application.systemLanguage == SystemLanguage.ChineseSimplified ||
                Application.systemLanguage == SystemLanguage.ChineseTraditional)
            {
                SetLocale(LocaleID.zh);
            }
            else
            {
                // 非中文地区默认英文
                SetLocale(LocaleID.en);
            }
            //Debug.LogWarning("没有存储的语言设置,只用默认语言");
        }
        private void LoadLocale(AsyncOperationHandle<LocalizationSettings> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                LoadLocale();
            }
            else
            {
                Debug.LogError("本地化插件初始化错误!");
            }
        }
        private void SaveLocale()
        {
            PlayerPrefs.SetInt(LanguagePrefKey,(int)currentLocale);
            PlayerPrefs.Save();
        }
        private void OnApplicationQuit()
        {
            SaveLocale(); 
        }

    }
}
