using System;

namespace Fumiki.Editor.DataTableGenerator
{
    public sealed partial class DataTableProcessor
    {
        public interface IDictionaryProcessor
        {
            Type KeyType { get; }
            Type ValueType { get; }
            string KeyLanguageKeyword { get; }
            string ValueLanguageKeyword { get; }
        }
    }
}