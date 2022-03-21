//using UnityGameFramework.Runtime;
//using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

//namespace Fumiki
//{
//    public class ProcedureCheckResources : ProcedureBase
//    {
//        private bool _checkResourcesComplete = false;
//        private bool _needUpdateResources = false;
//        private int _updateResourceCount = 0;
//        private long _updateResourceTotalCompressedLength = 0L;

//        public override bool UseNativeDialog => true;

//        protected override void OnEnter(ProcedureOwner procedureOwner)
//        {
//            base.OnEnter(procedureOwner);

//            _checkResourcesComplete = false;
//            _needUpdateResources = false;
//            _updateResourceCount = 0;
//            _updateResourceTotalCompressedLength = 0L;

//            GameEntry.Resource.CheckResources(OnCheckResourcesComplete);
//        }

//        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
//        {
//            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

//            if (!_checkResourcesComplete)
//            {
//                return;
//            }

//            if (_needUpdateResources)
//            {
//                procedureOwner.SetData<VarInt32>("UpdateResourceCount", _updateResourceCount);
//                procedureOwner.SetData<VarInt64>("UpdateResourceTotalCompressedLength", _updateResourceTotalCompressedLength);
//                ChangeState<ProcedureUpdateResources>(procedureOwner);
//            }
//            else
//            {
//                ChangeState<ProcedurePreload>(procedureOwner);
//            }
//        }

//        private void OnCheckResourcesComplete(int movedCount, int removedCount, int updateCount, long updateTotalLength, long updateTotalCompressedLength)
//        {
//            _checkResourcesComplete = true;
//            _needUpdateResources = updateCount > 0;
//            _updateResourceCount = updateCount;
//            _updateResourceTotalCompressedLength = updateTotalCompressedLength;
//            Log.Info("Check resources complete, '{0}' resources need to update, compressed length is '{1}', uncompressed length is '{2}'.", updateCount.ToString(), updateTotalCompressedLength.ToString(), updateTotalLength.ToString());
//        }
//    }
//}
