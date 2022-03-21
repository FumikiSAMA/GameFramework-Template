using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace Fumiki.Editor
{

    /// <summary>
    /// 事件参数类代码生成器
    /// </summary>
    public class EventGenerator : EditorWindow
    {
        /// <summary>
        /// 事件参数数据
        /// </summary>
        [Serializable]
        private class EventArgsData
        {
            public string Type;
            public string Name;
            public EventArgType TypeEnum;
            public EventArgsData()
            {

            }

            public EventArgsData(string type, string name)
            {
                Type = type;
                Name = name;
            }
        }

        private enum EventArgType
        {
            Object,
            Int,
            Float,
            Bool,
            Char,
            String,

            UnityObject,
            GameObject,
            Transform,
            Vector2,
            Vector3,
            Quaternion,

            Other,
        }

        [MenuItem("Game Framework/Generate Events")]
        public static void OpenAutoGenWindow()
        {
            EventGenerator window = GetWindow<EventGenerator>(true, "事件参数类代码生成器");
            window.minSize = new Vector2(600f, 600f);
        }

        /// <summary>
        /// 事件参数数据列表
        /// </summary>
        [SerializeField] private List<EventArgsData> _eventDatas = new List<EventArgsData>();

        /// <summary>
        /// 事件参数类名
        /// </summary>
        private string _className;

        //事件代码生成后的路径
        private const string EventCodePath = "Assets/GameMain/Scripts/Event";

        private void OnEnable()
        {
            _eventDatas.Clear();
            _className = "EventArgs";
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("事件参数类名：", GUILayout.Width(140f));
            _className = EditorGUILayout.TextField(_className);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("热更新层事件：", GUILayout.Width(140f));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("自动生成的代码路径：", GUILayout.Width(140f));
            EditorGUILayout.LabelField(EventCodePath);
            EditorGUILayout.EndHorizontal();

            //绘制事件参数相关按钮
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("添加事件参数", GUILayout.Width(140f)))
            {
                _eventDatas.Add(new EventArgsData(null, null));
            }
            if (GUILayout.Button("删除所有事件参数", GUILayout.Width(140f)))
            {
                _eventDatas.Clear();
            }
            if (GUILayout.Button("删除空事件参数", GUILayout.Width(140f)))
            {
                for (int i = _eventDatas.Count - 1; i >= 0; i--)
                {
                    EventArgsData data = _eventDatas[i];
                    if (string.IsNullOrWhiteSpace(data.Name))
                    {
                        _eventDatas.RemoveAt(i);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            //绘制事件参数数据
            for (int i = 0; i < _eventDatas.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EventArgsData data = _eventDatas[i];
                EditorGUILayout.LabelField("参数类型：", GUILayout.Width(70f));
                data.TypeEnum = (EventArgType)EditorGUILayout.EnumPopup(data.TypeEnum, GUILayout.Width(100f));
                switch (data.TypeEnum)
                {
                    case EventArgType.Object:
                    case EventArgType.Int:
                    case EventArgType.Float:
                    case EventArgType.Bool:
                    case EventArgType.Char:
                    case EventArgType.String:
                        data.Type = data.TypeEnum.ToString().ToLower();
                        break;

                    case EventArgType.UnityObject:
                        data.Type = "UnityEngine.Object";
                        break;

                    case EventArgType.Other:
                        data.Type = EditorGUILayout.TextField(data.Type, GUILayout.Width(140f));
                        break;

                    default:
                        data.Type = data.TypeEnum.ToString();
                        break;
                }
                EditorGUILayout.LabelField("参数字段名：", GUILayout.Width(70f));
                data.Name = EditorGUILayout.TextField(data.Name, GUILayout.Width(140f));
                EditorGUILayout.EndHorizontal();
            }

            //生成事件参数类代码
            if (GUILayout.Button("生成事件参数类代码", GUILayout.Width(210f)))
            {
                GenEventCode();
                AssetDatabase.Refresh();
            }
        }

        private void GenEventCode()
        {
            //根据是否为热更新层事件来决定一些参数
            string codePath = EventCodePath;
            string nameSpace = "Fumiki";
            string baseClass = "GameEventArgs";

            if (!Directory.Exists($"{codePath}/"))
            {
                Directory.CreateDirectory($"{codePath}/");
            }

            using (StreamWriter sw = new StreamWriter($"{codePath}/{_className}.cs"))
            {
                sw.WriteLine("using UnityEngine;");
                sw.WriteLine("using GameFramework.Event;");
                sw.WriteLine("");

                sw.WriteLine("//自动生成于：" + DateTime.Now);

                //命名空间
                sw.WriteLine("namespace " + nameSpace);
                sw.WriteLine("{");

                //类名
                sw.WriteLine($"\tpublic class {_className} : {baseClass}");
                sw.WriteLine("\t{");

                //事件编号
                sw.WriteLine($"\t\tpublic static readonly int EventId = typeof({_className}).GetHashCode();");
                sw.WriteLine("");
                sw.WriteLine("\t\tpublic override int Id => EventId;");
                sw.WriteLine("");

                //事件参数
                for (int i = 0; i < _eventDatas.Count; i++)
                {
                    EventArgsData data = _eventDatas[i];
                    data.Name = data.Name[0].ToString().ToUpper() + data.Name.Substring(1);
                    sw.WriteLine($"\t\tpublic {data.Type} {data.Name} {{ get; private set; }}");
                    sw.WriteLine("");
                }

                //清空参数数据方法
                sw.WriteLine($"\t\tpublic override void Clear()");
                sw.WriteLine("\t\t{");
                if (_eventDatas.Count <= 0) sw.WriteLine();
                foreach (EventArgsData data in _eventDatas)
                {
                    sw.WriteLine($"\t\t\t{data.Name} = default({data.Type});");
                }
                sw.WriteLine("\t\t}");
                sw.WriteLine("");

                //填充参数数据方法
                sw.Write($"\t\tpublic {_className} Fill(");
                for (int i = 0; i < _eventDatas.Count; i++)
                {
                    EventArgsData data = _eventDatas[i];
                    sw.Write($"{data.Type} {data.Name.ToLower()}");
                    if (i != _eventDatas.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.WriteLine(")");
                sw.WriteLine("\t\t{");
                foreach (EventArgsData data in _eventDatas)
                {
                    sw.WriteLine($"\t\t\t{data.Name} = {data.Name.ToLower()};");
                }
                sw.WriteLine("\t\t\treturn this;");
                sw.WriteLine("\t\t}");
                sw.WriteLine("\t}");
                sw.WriteLine("}");
            }
        }
    }
}
