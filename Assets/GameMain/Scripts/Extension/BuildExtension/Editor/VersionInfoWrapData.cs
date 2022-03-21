using System;
using UnityEngine;

namespace Fumiki.Editor.BuildExtension
{
    [Serializable]
    public class VersionInfoWrapData
    {
        [SerializeField] private string m_Key;
        [SerializeField] private VersionInfoData m_Value;

        public string Key
        {
            get => m_Key;
            set => m_Key = value;
        }

        public VersionInfoData Value
        {
            get => m_Value;
            set => m_Value = value;
        }
    }
}