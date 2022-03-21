using System;
using System.Collections.Generic;

namespace Fumiki.Editor.DataTableEditor
{
    [Serializable]
    public class DataTableRowData
    {
        public List<string> Data { get; set; }

        public DataTableRowData()
        {
            Data = new List<string>();
        }
    }
}