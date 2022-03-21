using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameFramework;
using GameFramework.DataTable;
using GameFramework.Event;
using GameFramework.Resource;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Fumiki
{
    public static class AwaitableExtension
    {
        private static readonly Dictionary<int, TaskCompletionSource<UIForm>> _uiFormTcs =
            new Dictionary<int, TaskCompletionSource<UIForm>>();

        private static readonly Dictionary<int, TaskCompletionSource<Entity>> _entityTcs =
            new Dictionary<int, TaskCompletionSource<Entity>>();

        private static readonly Dictionary<string, TaskCompletionSource<bool>> _dataTableTcs =
            new Dictionary<string, TaskCompletionSource<bool>>();

        private static readonly Dictionary<string, TaskCompletionSource<bool>> _sceneTcs =
            new Dictionary<string, TaskCompletionSource<bool>>();

        private static readonly HashSet<int> _webSerialIDs = new HashSet<int>();
        private static readonly List<WebResult> _delayReleaseWebResult = new List<WebResult>();

        private static readonly HashSet<int> _downloadSerialIds = new HashSet<int>();
        private static readonly List<DownloadResult> _delayReleaseDownloadResult = new List<DownloadResult>();

#if UNITY_EDITOR
        private static bool _isSubscribeEvent = false;
#endif

        /// <summary>
        /// 注册需要的事件 (需再流程入口处调用 防止框架重启导致事件被取消问题)
        /// </summary>
        public static void SubscribeEvent()
        {
            EventComponent eventComponent = UnityGameFramework.Runtime.GameEntry.GetComponent<EventComponent>();
            eventComponent.Subscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);
            eventComponent.Subscribe(OpenUIFormFailureEventArgs.EventId, OnOpenUIFormFailure);

            eventComponent.Subscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            eventComponent.Subscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFailure);

            eventComponent.Subscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
            eventComponent.Subscribe(LoadSceneFailureEventArgs.EventId, OnLoadSceneFailure);

            eventComponent.Subscribe(LoadDataTableSuccessEventArgs.EventId, OnLoadDataTableSuccess);
            eventComponent.Subscribe(LoadDataTableFailureEventArgs.EventId, OnLoadDataTableFailure);

            eventComponent.Subscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);
            eventComponent.Subscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailure);

            eventComponent.Subscribe(DownloadSuccessEventArgs.EventId, OnDownloadSuccess);
            eventComponent.Subscribe(DownloadFailureEventArgs.EventId, OnDownloadFailure);
#if UNITY_EDITOR
            _isSubscribeEvent = true;
#endif
        }

#if UNITY_EDITOR
        private static void TipsSubscribeEvent()
        {
            if (!_isSubscribeEvent)
            {
                throw new Exception("Use await/async extensions must to subscribe event!");
            }
        }
#endif

        /// <summary>
        /// 打开界面（可等待）
        /// </summary>
        public static Task<UIForm> OpenUIFormAsync(this UIComponent uiComponent, int uiFormId, object userData = null)
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            int? serialId = uiComponent.OpenUIForm(uiFormId, userData);
            if (serialId == null)
            {
                return Task.FromResult((UIForm) null);
            }

            var tcs = new TaskCompletionSource<UIForm>();
            _uiFormTcs.Add(serialId.Value, tcs);
            return tcs.Task;
        }

        /// <summary>
        /// 打开界面（可等待）
        /// </summary>
        public static Task<UIForm> OpenUIFormAsync(this UIComponent uiComponent, UIFormId uiFormId,
            object userData = null)
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            int? serialId = uiComponent.OpenUIForm(uiFormId, userData);
#elif UNITY_ANDROID || UNITY_IPHONE
            int? serialId = uiComponent.OpenUIForm((int) uiFormId + 1000, userData);
#endif
            if (serialId == null)
            {
                return Task.FromResult((UIForm) null);
            }

            var tcs = new TaskCompletionSource<UIForm>();
            _uiFormTcs.Add(serialId.Value, tcs);
            return tcs.Task;
        }

        /// <summary>
        /// 显示实体（可等待）
        /// </summary>
        public static Task<Entity> ShowEntityAsync(this EntityComponent entityComponent, Type logicType, int priority,
            EntityDataBase data)
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            TaskCompletionSource<Entity> tcs = new TaskCompletionSource<Entity>();
            _entityTcs.Add(data.Id, tcs);
            entityComponent.ShowEntity(logicType, priority, data);
            return tcs.Task;
        }

        /// <summary>
        /// 加载数据表（可等待）
        /// </summary>
        public static async Task<IDataTable<T>> LoadDataTableAsync<T>(this DataTableComponent dataTableComponent,
            string dataTableName, bool formBytes, object userData = null) where T : IDataRow
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            IDataTable<T> dataTable = dataTableComponent.GetDataTable<T>();
            if (dataTable != null)
            {
                return await Task.FromResult(dataTable);
            }

            var loadTcs = new TaskCompletionSource<bool>();
            var dataTableAssetName = AssetUtility.GetDataTableAsset(dataTableName, formBytes);
            _dataTableTcs.Add(dataTableAssetName, loadTcs);
            dataTableComponent.LoadDataTable(dataTableName, dataTableAssetName, userData);
            bool isLoaded = await loadTcs.Task;
            dataTable = isLoaded ? dataTableComponent.GetDataTable<T>() : null;
            return await Task.FromResult(dataTable);
        }


        private static void OnLoadDataTableSuccess(object sender, GameEventArgs e)
        {
            var ne = (LoadDataTableSuccessEventArgs) e;
            _dataTableTcs.TryGetValue(ne.DataTableAssetName, out TaskCompletionSource<bool> tcs);
            if (tcs != null)
            {
                Log.Info("Load data table '{0}' OK.", ne.DataTableAssetName);
                tcs.SetResult(true);
                _dataTableTcs.Remove(ne.DataTableAssetName);
            }
        }

        private static void OnLoadDataTableFailure(object sender, GameEventArgs e)
        {
            var ne = (LoadDataTableFailureEventArgs) e;
            _dataTableTcs.TryGetValue(ne.DataTableAssetName, out TaskCompletionSource<bool> tcs);
            if (tcs != null)
            {
                Log.Error("Can not load data table '{0}' from '{1}' with error message '{2}'.", ne.DataTableAssetName,
                    ne.DataTableAssetName, ne.ErrorMessage);
                tcs.SetResult(false);
                _dataTableTcs.Remove(ne.DataTableAssetName);
            }
        }


        /// <summary>
        /// 打开界面（可等待）
        /// </summary>
        public static Task<UIForm> OpenUIFormAsync(this UIComponent uiComponent, string uiFormAssetName,
            string uiGroupName, int priority, bool pauseCoveredUIForm, object userData)
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            int serialId = uiComponent.OpenUIForm(uiFormAssetName, uiGroupName, priority, pauseCoveredUIForm, userData);
            var tcs = new TaskCompletionSource<UIForm>();
            _uiFormTcs.Add(serialId, tcs);
            return tcs.Task;
        }

        private static void OnOpenUIFormSuccess(object sender, GameEventArgs e)
        {
            OpenUIFormSuccessEventArgs ne = (OpenUIFormSuccessEventArgs) e;
            _uiFormTcs.TryGetValue(ne.UIForm.SerialId, out TaskCompletionSource<UIForm> tcs);
            if (tcs != null)
            {
                tcs.SetResult(ne.UIForm);
                _uiFormTcs.Remove(ne.UIForm.SerialId);
            }
        }

        private static void OnOpenUIFormFailure(object sender, GameEventArgs e)
        {
            OpenUIFormFailureEventArgs ne = (OpenUIFormFailureEventArgs) e;
            _uiFormTcs.TryGetValue(ne.SerialId, out TaskCompletionSource<UIForm> tcs);
            if (tcs != null)
            {
                tcs.SetException(new GameFrameworkException(ne.ErrorMessage));
                _uiFormTcs.Remove(ne.SerialId);
            }
        }

        /// <summary>
        /// 显示实体（可等待）
        /// </summary>
        public static Task<Entity> ShowEntityAsync(this EntityComponent entityComponent, int entityId,
            Type entityLogicType, string entityAssetName, string entityGroupName, int priority, object userData)
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            var tcs = new TaskCompletionSource<Entity>();
            _entityTcs.Add(entityId, tcs);
            entityComponent.ShowEntity(entityId, entityLogicType, entityAssetName, entityGroupName, priority, userData);
            return tcs.Task;
        }


        private static void OnShowEntitySuccess(object sender, GameEventArgs e)
        {
            ShowEntitySuccessEventArgs ne = (ShowEntitySuccessEventArgs) e;
            EntityDataBase data = (EntityDataBase) ne.UserData;
            _entityTcs.TryGetValue(data.Id, out var tcs);
            if (tcs != null)
            {
                tcs.SetResult(ne.Entity);
                _entityTcs.Remove(data.Id);
            }
        }

        private static void OnShowEntityFailure(object sender, GameEventArgs e)
        {
            ShowEntityFailureEventArgs ne = (ShowEntityFailureEventArgs) e;
            _entityTcs.TryGetValue(ne.EntityId, out var tcs);
            if (tcs != null)
            {
                tcs.SetException(new GameFrameworkException(ne.ErrorMessage));
                _entityTcs.Remove(ne.EntityId);
            }
        }


        /// <summary>
        /// 加载场景（可等待）
        /// </summary>
        public static Task<bool> LoadSceneAsync(this SceneComponent sceneComponent, string sceneAssetName)
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            var tcs = new TaskCompletionSource<bool>();
            _sceneTcs.Add(sceneAssetName, tcs);
            sceneComponent.LoadScene(sceneAssetName);
            return tcs.Task;
        }

        private static void OnLoadSceneSuccess(object sender, GameEventArgs e)
        {
            LoadSceneSuccessEventArgs ne = (LoadSceneSuccessEventArgs) e;
            _sceneTcs.TryGetValue(ne.SceneAssetName, out var tcs);
            if (tcs != null)
            {
                tcs.SetResult(true);
                _sceneTcs.Remove(ne.SceneAssetName);
            }
        }

        private static void OnLoadSceneFailure(object sender, GameEventArgs e)
        {
            LoadSceneFailureEventArgs ne = (LoadSceneFailureEventArgs) e;
            _sceneTcs.TryGetValue(ne.SceneAssetName, out var tcs);
            if (tcs != null)
            {
                tcs.SetException(new GameFrameworkException(ne.ErrorMessage));
                _sceneTcs.Remove(ne.SceneAssetName);
            }
        }

        /// <summary>
        /// 加载资源（可等待）
        /// </summary>
        public static Task<T> LoadAssetAsync<T>(this ResourceComponent resourceComponent, string assetName)
            where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            TaskCompletionSource<T> loadAssetTcs = new TaskCompletionSource<T>();
            resourceComponent.LoadAsset(assetName, typeof(T), new LoadAssetCallbacks(
                (tempAssetName, asset, duration, userdata) =>
                {
                    var source = loadAssetTcs;
                    loadAssetTcs = null;
                    T tAsset = asset as T;
                    if (tAsset != null)
                    {
                        source.SetResult(tAsset);
                    }
                    else
                    {
                        source.SetException(new GameFrameworkException(
                            $"Load asset failure load type is {asset.GetType()} but asset type is {typeof(T)}."));
                    }
                },
                (tempAssetName, status, errorMessage, userdata) =>
                {
                    loadAssetTcs.SetException(new GameFrameworkException(errorMessage));
                }
            ));

            return loadAssetTcs.Task;
        }

        /// <summary>
        /// 加载多个资源（可等待）
        /// </summary>
        public static async Task<T[]> LoadAssetsAsync<T>(this ResourceComponent resourceComponent, string[] assetName)
            where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            if (assetName == null)
            {
                return null;
            }

            T[] assets = new T[assetName.Length];
            Task<T>[] tasks = new Task<T>[assets.Length];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = resourceComponent.LoadAssetAsync<T>(assetName[i]);
            }

            await Task.WhenAll(tasks);
            for (int i = 0; i < assets.Length; i++)
            {
                assets[i] = tasks[i].Result;
            }

            return assets;
        }


        /// <summary>
        /// 增加Web请求任务（可等待）
        /// </summary>
        public static Task<WebResult> AddWebRequestAsync(this WebRequestComponent webRequestComponent,
            string webRequestUri, WWWForm wwwForm = null, object userdata = null)
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            var tsc = new TaskCompletionSource<WebResult>();
            int serialId = webRequestComponent.AddWebRequest(webRequestUri, wwwForm,
                AwaitDataWrap<WebResult>.Create(userdata, tsc));
            _webSerialIDs.Add(serialId);
            return tsc.Task;
        }

        /// <summary>
        /// 增加Web请求任务（可等待）
        /// </summary>
        public static Task<WebResult> AddWebRequestAsync(this WebRequestComponent webRequestComponent,
            string webRequestUri, byte[] postData, object userdata = null)
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            var tsc = new TaskCompletionSource<WebResult>();
            int serialId = webRequestComponent.AddWebRequest(webRequestUri, postData,
                AwaitDataWrap<WebResult>.Create(userdata, tsc));
            _webSerialIDs.Add(serialId);
            return tsc.Task;
        }

        private static void OnWebRequestSuccess(object sender, GameEventArgs e)
        {
            WebRequestSuccessEventArgs ne = (WebRequestSuccessEventArgs) e;
            if (_webSerialIDs.Contains(ne.SerialId))
            {
                if (ne.UserData is AwaitDataWrap<WebResult> webRequestUserdata)
                {
                    WebResult result = WebResult.Create(ne.GetWebResponseBytes(), false, string.Empty,
                        webRequestUserdata.UserData);
                    _delayReleaseWebResult.Add(result);
                    webRequestUserdata.Source.TrySetResult(result);
                    ReferencePool.Release(webRequestUserdata);
                }

                _webSerialIDs.Remove(ne.SerialId);
                if (_webSerialIDs.Count == 0)
                {
                    for (int i = 0; i < _delayReleaseWebResult.Count; i++)
                    {
                        ReferencePool.Release(_delayReleaseWebResult[i]);
                    }

                    _delayReleaseWebResult.Clear();
                }
            }
        }

        private static void OnWebRequestFailure(object sender, GameEventArgs e)
        {
            WebRequestFailureEventArgs ne = (WebRequestFailureEventArgs) e;
            if (_webSerialIDs.Contains(ne.SerialId))
            {
                if (ne.UserData is AwaitDataWrap<WebResult> webRequestUserdata)
                {
                    WebResult result = WebResult.Create(null, true, ne.ErrorMessage, webRequestUserdata.UserData);
                    webRequestUserdata.Source.TrySetResult(result);
                    _delayReleaseWebResult.Add(result);
                    ReferencePool.Release(webRequestUserdata);
                }

                _webSerialIDs.Remove(ne.SerialId);
                if (_webSerialIDs.Count == 0)
                {
                    for (int i = 0; i < _delayReleaseWebResult.Count; i++)
                    {
                        ReferencePool.Release(_delayReleaseWebResult[i]);
                    }

                    _delayReleaseWebResult.Clear();
                }
            }
        }

        /// <summary>
        /// 增加下载任务（可等待)
        /// </summary>
        public static Task<DownloadResult> AddDownloadAsync(this DownloadComponent downloadComponent,
            string downloadPath, string downloadUri, object userdata = null)
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            var tcs = new TaskCompletionSource<DownloadResult>();
            int serialId = downloadComponent.AddDownload(downloadPath, downloadUri,
                AwaitDataWrap<DownloadResult>.Create(userdata, tcs));
            _downloadSerialIds.Add(serialId);
            return tcs.Task;
        }

        private static void OnDownloadSuccess(object sender, GameEventArgs e)
        {
            DownloadSuccessEventArgs ne = (DownloadSuccessEventArgs) e;
            if (_downloadSerialIds.Contains(ne.SerialId))
            {
                if (ne.UserData is AwaitDataWrap<DownloadResult> awaitDataWrap)
                {
                    DownloadResult result = DownloadResult.Create(false, string.Empty, awaitDataWrap.UserData);
                    _delayReleaseDownloadResult.Add(result);
                    awaitDataWrap.Source.TrySetResult(result);
                    ReferencePool.Release(awaitDataWrap);
                }

                _downloadSerialIds.Remove(ne.SerialId);
                if (_downloadSerialIds.Count == 0)
                {
                    for (int i = 0; i < _delayReleaseDownloadResult.Count; i++)
                    {
                        ReferencePool.Release(_delayReleaseDownloadResult[i]);
                    }

                    _delayReleaseDownloadResult.Clear();
                }
            }
        }

        private static void OnDownloadFailure(object sender, GameEventArgs e)
        {
            DownloadFailureEventArgs ne = (DownloadFailureEventArgs) e;
            if (_downloadSerialIds.Contains(ne.SerialId))
            {
                if (ne.UserData is AwaitDataWrap<DownloadResult> awaitDataWrap)
                {
                    DownloadResult result = DownloadResult.Create(true, ne.ErrorMessage, awaitDataWrap.UserData);
                    _delayReleaseDownloadResult.Add(result);
                    awaitDataWrap.Source.TrySetResult(result);
                    ReferencePool.Release(awaitDataWrap);
                }

                _downloadSerialIds.Remove(ne.SerialId);
                if (_downloadSerialIds.Count == 0)
                {
                    for (int i = 0; i < _delayReleaseDownloadResult.Count; i++)
                    {
                        ReferencePool.Release(_delayReleaseDownloadResult[i]);
                    }

                    _delayReleaseDownloadResult.Clear();
                }
            }
        }
    }
}