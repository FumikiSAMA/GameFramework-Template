//using GameFramework;
//using GameFramework.Event;
//using GameFramework.Resource;
//using UnityEngine;
//using UnityGameFramework.Runtime;
//using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

//namespace Fumiki
//{
//    public class ProcedureCheckVersion : ProcedureBase
//    {
//        private bool _checkVersionComplete = false;
//        private bool _needUpdateVersion = false;
//        private VersionInfo _versionInfo = null;

//        public override bool UseNativeDialog => true;

//        protected override void OnEnter(ProcedureOwner procedureOwner)
//        {
//            base.OnEnter(procedureOwner);

//            _checkVersionComplete = false;
//            _needUpdateVersion = false;
//            _versionInfo = null;

//            GameEntry.Event.Subscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);
//            GameEntry.Event.Subscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailure);

//            // 向服务器请求版本信息
//            GameEntry.WebRequest.AddWebRequest(Utility.Text.Format(GameEntry.BuiltinData.BuildInfo.CheckVersionUrl, GetPlatformPath()), this);
//        }

//        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
//        {
//            GameEntry.Event.Unsubscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);
//            GameEntry.Event.Unsubscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailure);

//            base.OnLeave(procedureOwner, isShutdown);
//        }

//        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
//        {
//            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

//            if (!_checkVersionComplete)
//            {
//                return;
//            }

//            if (_needUpdateVersion)
//            {
//                procedureOwner.SetData<VarInt32>("VersionListLength", _versionInfo.VersionListLength);
//                procedureOwner.SetData<VarInt32>("VersionListHashCode", _versionInfo.VersionListHashCode);
//                procedureOwner.SetData<VarInt32>("VersionListCompressedLength", _versionInfo.VersionListCompressedLength);
//                procedureOwner.SetData<VarInt32>("VersionListCompressedHashCode", _versionInfo.VersionListCompressedHashCode);
//                ChangeState<ProcedureUpdateVersion>(procedureOwner);
//            }
//            else
//            {
//                ChangeState<ProcedureVerifyResources>(procedureOwner);
//            }
//        }

//        private void GotoUpdateApp(object userData)
//        {
//            string url = null;
//#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
//            url = GameEntry.BuiltinData.BuildInfo.WindowsAppUrl;
//#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
//            url = GameEntry.BuiltinData.BuildInfo.MacOSAppUrl;
//#elif UNITY_IOS
//            url = GameEntry.BuiltinData.BuildInfo.IOSAppUrl;
//#elif UNITY_ANDROID
//            url = GameEntry.BuiltinData.BuildInfo.AndroidAppUrl;
//#endif
//            if (!string.IsNullOrEmpty(url))
//            {
//                Application.OpenURL(url);
//            }
//        }

//        private void OnWebRequestSuccess(object sender, GameEventArgs e)
//        {
//            WebRequestSuccessEventArgs ne = (WebRequestSuccessEventArgs)e;
//            if (ne.UserData != this)
//            {
//                return;
//            }

//            // 解析版本信息
//            byte[] versionInfoBytes = ne.GetWebResponseBytes();
//            string versionInfoString = Utility.Converter.GetString(versionInfoBytes);
//            _versionInfo = Utility.Json.ToObject<VersionInfo>(versionInfoString);
//            if (_versionInfo == null)
//            {
//                Log.Error("Parse VersionInfo failure.");
//                return;
//            }

//            Log.Info("Latest game version is '{0} ({1})', local game version is '{2} ({3})'.", _versionInfo.LatestGameVersion, _versionInfo.InternalGameVersion.ToString(), Version.GameVersion, Version.InternalGameVersion.ToString());

//            if (_versionInfo.ForceUpdateGame)
//            {
//                // 需要强制更新游戏应用
//                GameEntry.UI.OpenDialog(new DialogParams
//                {
//                    Mode = 2,
//                    Title = GameEntry.Localization.GetString("ForceUpdate.Title"),
//                    Message = GameEntry.Localization.GetString("ForceUpdate.Message"),
//                    ConfirmText = GameEntry.Localization.GetString("ForceUpdate.UpdateButton"),
//                    OnClickConfirm = GotoUpdateApp,
//                    CancelText = GameEntry.Localization.GetString("ForceUpdate.QuitButton"),
//                    OnClickCancel = delegate (object userData) { UnityGameFramework.Runtime.GameEntry.Shutdown(ShutdownType.Quit); },
//                });

//                return;
//            }

//            // 设置资源更新下载地址
//            GameEntry.Resource.UpdatePrefixUri = Utility.Path.GetRegularPath(_versionInfo.UpdatePrefixUri);

//            _checkVersionComplete = true;
//            _needUpdateVersion = GameEntry.Resource.CheckVersionList(_versionInfo.InternalResourceVersion) == CheckVersionListResult.NeedUpdate;
//        }

//        private void OnWebRequestFailure(object sender, GameEventArgs e)
//        {
//            WebRequestFailureEventArgs ne = (WebRequestFailureEventArgs)e;
//            if (ne.UserData != this)
//            {
//                return;
//            }

//            Log.Warning("Check version failure, error message is '{0}'.", ne.ErrorMessage);
//        }

//        private string GetPlatformPath()
//        {
//            switch (Application.platform)
//            {
//                case RuntimePlatform.WindowsEditor:
//                case RuntimePlatform.WindowsPlayer:
//                    return "Windows";

//                case RuntimePlatform.OSXEditor:
//                case RuntimePlatform.OSXPlayer:
//                    return "MacOS";

//                case RuntimePlatform.IPhonePlayer:
//                    return "IOS";

//                case RuntimePlatform.Android:
//                    return "Android";

//                default:
//                    throw new System.NotSupportedException(Utility.Text.Format("Platform '{0}' is not supported.", Application.platform));
//            }
//        }
//    }
//}
