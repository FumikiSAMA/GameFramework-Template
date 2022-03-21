//using GameFramework.Resource;
//using UnityGameFramework.Runtime;
//using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

//namespace Fumiki
//{
//    public class ProcedureUpdateVersion : ProcedureBase
//    {
//        private bool _updateVersionComplete = false;
//        private UpdateVersionListCallbacks _updateVersionListCallbacks = null;

//        public override bool UseNativeDialog => true;

//        protected override void OnInit(ProcedureOwner procedureOwner)
//        {
//            base.OnInit(procedureOwner);

//            _updateVersionListCallbacks = new UpdateVersionListCallbacks(OnUpdateVersionListSuccess, OnUpdateVersionListFailure);
//        }

//        protected override void OnEnter(ProcedureOwner procedureOwner)
//        {
//            base.OnEnter(procedureOwner);

//            _updateVersionComplete = false;

//            GameEntry.Resource.UpdateVersionList(procedureOwner.GetData<VarInt32>("VersionListLength"), procedureOwner.GetData<VarInt32>("VersionListHashCode"), procedureOwner.GetData<VarInt32>("VersionListCompressedLength"), procedureOwner.GetData<VarInt32>("VersionListCompressedHashCode"), _updateVersionListCallbacks);
//            procedureOwner.RemoveData("VersionListLength");
//            procedureOwner.RemoveData("VersionListHashCode");
//            procedureOwner.RemoveData("VersionListCompressedLength");
//            procedureOwner.RemoveData("VersionListCompressedHashCode");
//        }

//        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
//        {
//            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

//            if (!_updateVersionComplete)
//            {
//                return;
//            }

//            ChangeState<ProcedureVerifyResources>(procedureOwner);
//        }

//        private void OnUpdateVersionListSuccess(string downloadPath, string downloadUri)
//        {
//            _updateVersionComplete = true;
//            Log.Info("Update version list from '{0}' success.", downloadUri);
//        }

//        private void OnUpdateVersionListFailure(string downloadUri, string errorMessage)
//        {
//            Log.Warning("Update version list from '{0}' failure, error message is '{1}'.", downloadUri, errorMessage);
//        }
//    }
//}
