﻿//------------------------------------------------------------
// Fumiki Game Studio
// Author: Fumiki
//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2023-04-29 14:27:54.305
//------------------------------------------------------------

using GameFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Fumiki
{
    /// <summary>
    /// 声音配置表。
    /// </summary>
    public class DRUISound : DataRowBase
    {
        private int _id = 0;

        /// <summary>
        /// 获取声音编号。
        /// </summary>
        public override int Id => _id;

        /// <summary>
        /// 获取资源名称。
        /// </summary>
        public string AssetName
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取优先级（默认0，128最高，-128最低）。
        /// </summary>
        public int Priority
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取音量（0~1）。
        /// </summary>
        public float Volume
        {
            get;
            private set;
        }

        public override bool ParseDataRow(string dataRowString, object userData)
        {
            string[] columnStrings = dataRowString.Split(DataTableExtension.DataSplitSeparators);
            for (int i = 0; i < columnStrings.Length; i++)
            {
                columnStrings[i] = columnStrings[i].Trim(DataTableExtension.DataTrimSeparators);
            }

            int index = 0;
            index++;
            _id = int.Parse(columnStrings[index++]);
            index++;
			AssetName = columnStrings[index++];
			Priority = int.Parse(columnStrings[index++]);
			Volume = float.Parse(columnStrings[index++]);
            GeneratePropertyArray();
            return true;
        }

        public override bool ParseDataRow(byte[] dataRowBytes, int startIndex, int length, object userData)
        {
            using (MemoryStream memoryStream = new MemoryStream(dataRowBytes, startIndex, length, false))
            {
                using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8))
                {
                    _id = binaryReader.Read7BitEncodedInt32();
                    AssetName = binaryReader.ReadString();
                    Priority = binaryReader.Read7BitEncodedInt32();
                    Volume = binaryReader.ReadSingle();
                }
            }

            GeneratePropertyArray();
            return true;
        }

        private void GeneratePropertyArray()
        {

        }
    }
}
