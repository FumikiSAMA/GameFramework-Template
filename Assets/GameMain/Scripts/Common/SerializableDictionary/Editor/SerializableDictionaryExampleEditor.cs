using UnityEditor;

namespace Fumiki.Editor.SerializableDictionary
{
    [CustomEditor(typeof(SerializableDictionaryExample))]
    public class SerializableDictionaryExampleEditor : UnityEditor.Editor
    {
        private SerializableDictionaryExample Target=> target as SerializableDictionaryExample;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}