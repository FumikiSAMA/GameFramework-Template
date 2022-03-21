using GameFramework.Debugger;
using GameFramework.Localization;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Fumiki
{
    public class ChangeLanguageDebuggerWindow : IDebuggerWindow
    {
        private Vector2 _scrollPosition = Vector2.zero;
        private bool _needRestart = false;

        public void Initialize(params object[] args)
        {
        }

        public void Shutdown()
        {
        }

        public void OnEnter()
        {
        }

        public void OnLeave()
        {
        }

        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (_needRestart)
            {
                _needRestart = false;
                UnityGameFramework.Runtime.GameEntry.Shutdown(ShutdownType.Restart);
            }
        }

        public void OnDraw()
        {
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            {
                DrawSectionChangeLanguage();
            }
            GUILayout.EndScrollView();
        }

        private void DrawSectionChangeLanguage()
        {
            GUILayout.Label("<b>Change Language</b>");
            GUILayout.BeginHorizontal("box");
            {
                if (GUILayout.Button("Chinese Simplified", GUILayout.Height(30)))
                {
                    GameEntry.Localization.Language = Language.ChineseSimplified;
                    SaveLanguage();
                }
                if (GUILayout.Button("Chinese Traditional", GUILayout.Height(30)))
                {
                    GameEntry.Localization.Language = Language.ChineseTraditional;
                    SaveLanguage();
                }
                if (GUILayout.Button("English", GUILayout.Height(30)))
                {
                    GameEntry.Localization.Language = Language.English;
                    SaveLanguage();
                }
            }
            GUILayout.EndHorizontal();
        }

        private void SaveLanguage()
        {
            GameEntry.Setting.SetString(Constant.Setting.Language, GameEntry.Localization.Language.ToString());
            GameEntry.Setting.Save();
            _needRestart = true;
        }
    }
}
