//using GameFramework;
//using GameFramework.Event;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityGameFramework.Runtime;
//using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

//namespace Fumiki
//{
//    public class ProcedureUpdateResources : ProcedureBase
//    {
//        private bool _updateResourcesComplete = false;
//        private int _updateCount = 0;
//        private long _updateTotalCompressedLength = 0L;
//        private int _updateSuccessCount = 0;
//        private List<UpdateLengthData> _updateLengthData = new List<UpdateLengthData>();
//        private UIUpdateResourceFormLogic uiUpdateResourceFormLogic = null;

//        public override bool UseNativeDialog => true;

//        protected override void OnEnter(ProcedureOwner procedureOwner)
//        {
//            base.OnEnter(procedureOwner);

//            _updateResourcesComplete = false;
//            _updateCount = procedureOwner.GetData<VarInt32>("UpdateResourceCount");
//            procedureOwner.RemoveData("UpdateResourceCount");
//            _updateTotalCompressedLength = procedureOwner.GetData<VarInt64>("UpdateResourceTotalCompressedLength");
//            procedureOwner.RemoveData("UpdateResourceTotalCompressedLength");
//            _updateSuccessCount = 0;
//            _updateLengthData.Clear();
//            uiUpdateResourceFormLogic = null;

//            GameEntry.Event.Subscribe(ResourceUpdateStartEventArgs.EventId, OnResourceUpdateStart);
//            GameEntry.Event.Subscribe(ResourceUpdateChangedEventArgs.EventId, OnResourceUpdateChanged);
//            GameEntry.Event.Subscribe(ResourceUpdateSuccessEventArgs.EventId, OnResourceUpdateSuccess);
//            GameEntry.Event.Subscribe(ResourceUpdateFailureEventArgs.EventId, OnResourceUpdateFailure);

//            if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
//            {
//                GameEntry.UI.OpenDialog(new DialogParams
//                {
//                    Mode = 2,
//                    Title = GameEntry.Localization.GetString("UpdateResourceViaCarrierDataNetwork.Title"),
//                    Message = GameEntry.Localization.GetString("UpdateResourceViaCarrierDataNetwork.Message"),
//                    ConfirmText = GameEntry.Localization.GetString("UpdateResourceViaCarrierDataNetwork.UpdateButton"),
//                    OnClickConfirm = StartUpdateResources,
//                    CancelText = GameEntry.Localization.GetString("UpdateResourceViaCarrierDataNetwork.QuitButton"),
//                    OnClickCancel = delegate (object userData) { UnityGameFramework.Runtime.GameEntry.Shutdown(ShutdownType.Quit); },
//                });

//                return;
//            }

//            StartUpdateResources(null);
//        }

//        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
//        {
//            if (uiUpdateResourceFormLogic != null)
//            {
//                Object.Destroy(uiUpdateResourceFormLogic.gameObject);
//                uiUpdateResourceFormLogic = null;
//            }

//            GameEntry.Event.Unsubscribe(ResourceUpdateStartEventArgs.EventId, OnResourceUpdateStart);
//            GameEntry.Event.Unsubscribe(ResourceUpdateChangedEventArgs.EventId, OnResourceUpdateChanged);
//            GameEntry.Event.Unsubscribe(ResourceUpdateSuccessEventArgs.EventId, OnResourceUpdateSuccess);
//            GameEntry.Event.Unsubscribe(ResourceUpdateFailureEventArgs.EventId, OnResourceUpdateFailure);

//            base.OnLeave(procedureOwner, isShutdown);
//        }

//        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
//        {
//            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

//            if (!_updateResourcesComplete)
//            {
//                return;
//            }

//            ChangeState<ProcedurePreload>(procedureOwner);
//        }

//        private void StartUpdateResources(object userData)
//        {
//            if (uiUpdateResourceFormLogic == null)
//            {
//                uiUpdateResourceFormLogic = Object.Instantiate(GameEntry.BuiltinData.UiUpdateResourceFormLogicTemplate);
//            }

//            Log.Info("Start update resources...");
//            GameEntry.Resource.UpdateResources(OnUpdateResourcesComplete);
//        }

//        private void RefreshProgress()
//        {
//            long currentTotalUpdateLength = 0L;
//            for (int i = 0; i < _updateLengthData.Count; i++)
//            {
//                currentTotalUpdateLength += _updateLengthData[i].Length;
//            }

//            float progressTotal = (float)currentTotalUpdateLength / _updateTotalCompressedLength;
//            string descriptionText = GameEntry.Localization.GetString("UpdateResource.Tips", _updateSuccessCount.ToString(), _updateCount.ToString(), GetByteLengthString(currentTotalUpdateLength), GetByteLengthString(_updateTotalCompressedLength), progressTotal, GetByteLengthString((int)GameEntry.Download.CurrentSpeed));
//            uiUpdateResourceFormLogic.SetProgress(progressTotal, descriptionText);
//        }

//        private string GetByteLengthString(long byteLength)
//        {
//            if (byteLength < 1024L) // 2 ^ 10
//            {
//                return Utility.Text.Format("{0} Bytes", byteLength);
//            }

//            if (byteLength < 1048576L) // 2 ^ 20
//            {
//                return Utility.Text.Format("{0:F2} KB", byteLength / 1024f);
//            }

//            if (byteLength < 1073741824L) // 2 ^ 30
//            {
//                return Utility.Text.Format("{0:F2} MB", byteLength / 1048576f);
//            }

//            if (byteLength < 1099511627776L) // 2 ^ 40
//            {
//                return Utility.Text.Format("{0:F2} GB", byteLength / 1073741824f);
//            }

//            if (byteLength < 1125899906842624L) // 2 ^ 50
//            {
//                return Utility.Text.Format("{0:F2} TB", byteLength / 1099511627776f);
//            }

//            if (byteLength < 1152921504606846976L) // 2 ^ 60
//            {
//                return Utility.Text.Format("{0:F2} PB", byteLength / 1125899906842624f);
//            }

//            return Utility.Text.Format("{0:F2} EB", byteLength / 1152921504606846976f);
//        }

//        private void OnUpdateResourcesComplete(GameFramework.Resource.IResourceGroup resourceGroup, bool result)
//        {
//            if (result)
//            {
//                _updateResourcesComplete = true;
//                Log.Info("Update resources complete with no errors.");
//            }
//            else
//            {
//                Log.Error("Update resources complete with errors.");
//            }
//        }

//        private void OnResourceUpdateStart(object sender, GameEventArgs e)
//        {
//            ResourceUpdateStartEventArgs ne = (ResourceUpdateStartEventArgs)e;

//            for (int i = 0; i < _updateLengthData.Count; i++)
//            {
//                if (_updateLengthData[i].Name == ne.Name)
//                {
//                    Log.Warning("Update resource '{0}' is invalid.", ne.Name);
//                    _updateLengthData[i].Length = 0;
//                    RefreshProgress();
//                    return;
//                }
//            }

//            _updateLengthData.Add(new UpdateLengthData(ne.Name));
//        }

//        private void OnResourceUpdateChanged(object sender, GameEventArgs e)
//        {
//            ResourceUpdateChangedEventArgs ne = (ResourceUpdateChangedEventArgs)e;

//            for (int i = 0; i < _updateLengthData.Count; i++)
//            {
//                if (_updateLengthData[i].Name == ne.Name)
//                {
//                    _updateLengthData[i].Length = ne.CurrentLength;
//                    RefreshProgress();
//                    return;
//                }
//            }

//            Log.Warning("Update resource '{0}' is invalid.", ne.Name);
//        }

//        private void OnResourceUpdateSuccess(object sender, GameEventArgs e)
//        {
//            ResourceUpdateSuccessEventArgs ne = (ResourceUpdateSuccessEventArgs)e;
//            Log.Info("Update resource '{0}' success.", ne.Name);

//            for (int i = 0; i < _updateLengthData.Count; i++)
//            {
//                if (_updateLengthData[i].Name == ne.Name)
//                {
//                    _updateLengthData[i].Length = ne.CompressedLength;
//                    _updateSuccessCount++;
//                    RefreshProgress();
//                    return;
//                }
//            }

//            Log.Warning("Update resource '{0}' is invalid.", ne.Name);
//        }

//        private void OnResourceUpdateFailure(object sender, GameEventArgs e)
//        {
//            ResourceUpdateFailureEventArgs ne = (ResourceUpdateFailureEventArgs)e;
//            if (ne.RetryCount >= ne.TotalRetryCount)
//            {
//                Log.Error("Update resource '{0}' failure from '{1}' with error message '{2}', retry count '{3}'.", ne.Name, ne.DownloadUri, ne.ErrorMessage, ne.RetryCount.ToString());
//                return;
//            }
//            else
//            {
//                Log.Info("Update resource '{0}' failure from '{1}' with error message '{2}', retry count '{3}'.", ne.Name, ne.DownloadUri, ne.ErrorMessage, ne.RetryCount.ToString());
//            }

//            for (int i = 0; i < _updateLengthData.Count; i++)
//            {
//                if (_updateLengthData[i].Name == ne.Name)
//                {
//                    _updateLengthData.Remove(_updateLengthData[i]);
//                    RefreshProgress();
//                    return;
//                }
//            }

//            Log.Warning("Update resource '{0}' is invalid.", ne.Name);
//        }

//        private class UpdateLengthData
//        {
//            private readonly string m_Name;

//            public UpdateLengthData(string name)
//            {
//                m_Name = name;
//            }

//            public string Name => m_Name;

//            public int Length
//            {
//                get;
//                set;
//            }
//        }
//    }
//}
