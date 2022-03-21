using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Fumiki
{
    public abstract class EntityLogicBase : EntityLogic
    {
        [SerializeField]
        private EntityDataBase _entityData = null;

        public int Id => Entity.Id;

        public Animator CachedAnimator
        {
            get;
            set;
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnInit(object userData)
#else
        protected internal override void OnInit(object userData)
#endif
        {
            base.OnInit(userData);
            if (transform.GetChild(0).childCount > 0)
                CachedAnimator = transform.GetChild(0).GetChild(0).GetComponent<Animator>();
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnRecycle()
#else
        protected internal override void OnRecycle()
#endif
        {
            base.OnRecycle();
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnShow(object userData)
#else
        protected internal override void OnShow(object userData)
#endif
        {
            base.OnShow(userData);

            _entityData = userData as EntityDataBase;
            if (_entityData == null)
            {
                Log.Error("Entity data is invalid.");
                return;
            }

            //Name = Utility.Text.Format("[Entity {0}]", Id);
            CachedTransform.localPosition = _entityData.Position;
            CachedTransform.localRotation = _entityData.Rotation;
            CachedTransform.localScale = _entityData.Scale;
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnHide(bool isShutdown, object userData)
#else
        protected internal override void OnHide(bool isShutdown, object userData)
#endif
        {
            base.OnHide(isShutdown, userData);
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnAttached(EntityLogic childEntity, Transform parentTransform, object userData)
#else
        protected internal override void OnAttached(EntityLogic childEntity, Transform parentTransform, object userData)
#endif
        {
            base.OnAttached(childEntity, parentTransform, userData);
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnDetached(EntityLogic childEntity, object userData)
#else
        protected internal override void OnDetached(EntityLogic childEntity, object userData)
#endif
        {
            base.OnDetached(childEntity, userData);
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnAttachTo(EntityLogic parentEntity, Transform parentTransform, object userData)
#else
        protected internal override void OnAttachTo(EntityLogic parentEntity, Transform parentTransform, object userData)
#endif
        {
            base.OnAttachTo(parentEntity, parentTransform, userData);
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnDetachFrom(EntityLogic parentEntity, object userData)
#else
        protected internal override void OnDetachFrom(EntityLogic parentEntity, object userData)
#endif
        {
            base.OnDetachFrom(parentEntity, userData);
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
#else
        protected internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
#endif
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
        }

        public void SetPosition(Vector3 position)
        {
            CachedTransform.localPosition = position;
        }

        public void SetRotation(Quaternion rotation)
        {
            CachedTransform.localRotation = rotation;
        }

        public void SetScale(Vector3 scale)
        {
            CachedTransform.localScale = scale;
        }
    }
}
