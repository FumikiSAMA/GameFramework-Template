using System;
using UnityEngine;

namespace Fumiki
{
    [Serializable]
    public abstract class EntityDataBase
    {
        [SerializeField] private int _id = 0;

        [SerializeField] private int _typeId = 0;

        [SerializeField] private Vector3 _position = Vector3.zero;

        [SerializeField] private Quaternion _rotation = Quaternion.identity;
        [SerializeField] private Vector3 _scale = Vector3.one;

        public EntityDataBase(int entityId, int typeId)
        {
            _id = entityId;
            _typeId = typeId;
        }

        /// <summary>
        /// 实体编号
        /// </summary>
        public int Id => _id;

        /// <summary>
        /// 实体类型编号
        /// </summary>
        public int TypeId => _typeId;

        /// <summary>
        /// 实体位置
        /// </summary>
        public Vector3 Position
        {
            get => _position;
            set => _position = value;
        }

        /// <summary>
        /// 实体朝向
        /// </summary>
        public Quaternion Rotation
        {
            get => _rotation;
            set => _rotation = value;
        }

        /// <summary>
        /// 实体缩放
        /// </summary>
        public Vector3 Scale
        {
            get => _scale;
            set => _scale = value;
        }
    }
}