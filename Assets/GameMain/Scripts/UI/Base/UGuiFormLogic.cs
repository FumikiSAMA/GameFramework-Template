using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace Fumiki
{
    public abstract class UGuiFormLogic : UIFormLogic
    {
        public const int DepthFactor = 100;
        private const float FadeTime = 0.3f;

        private Canvas _cachedCanvas = null;
        private CanvasGroup _canvasGroup = null;
        private List<Canvas> _cachedCanvasContainer = new List<Canvas>();

        public int OriginalDepth { get; private set; }

        public int Depth => _cachedCanvas.sortingOrder;

        public void Close()
        {
            GameEntry.UI.CloseUIForm(this);
        }

        public void PlayUISound(int uiSoundId)
        {
            GameEntry.Sound.PlayUISound(uiSoundId);
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnInit(object userData)
#else
        protected internal override void OnInit(object userData)
#endif
        {
            base.OnInit(userData);

            _cachedCanvas = gameObject.GetOrAddComponent<Canvas>();
            _cachedCanvas.overrideSorting = true;
            OriginalDepth = _cachedCanvas.sortingOrder;

            _canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();

            RectTransform transform = GetComponent<RectTransform>();
            transform.anchorMin = Vector2.zero;
            transform.anchorMax = Vector2.one;
            transform.anchoredPosition = Vector2.zero;
            transform.sizeDelta = Vector2.zero;

            gameObject.GetOrAddComponent<GraphicRaycaster>();

            TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
            foreach (TMP_Text tmpText in texts)
            {
                tmpText.font = GameEntry.StaticResource.FontAssets["MainFont"];
                if (!string.IsNullOrEmpty(tmpText.text))
                {
                    tmpText.text = GameEntry.Localization.GetString(tmpText.text);
                }
            }
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
        protected override void OnOpen(object userData)
#else
        protected internal override void OnOpen(object userData)
#endif
        {
            base.OnOpen(userData);
#if UNITY_ANDROID || UNITY_IOS
            gameObject.GetComponent<LiteFeel.UIAdaptation.UIPanel>().OnEnable();
#endif
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnClose(bool isShutdown, object userData)
#else
        protected internal override void OnClose(bool isShutdown, object userData)
#endif
        {
            base.OnClose(isShutdown, userData);
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnPause()
#else
        protected internal override void OnPause()
#endif
        {
            base.OnPause();
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnResume()
#else
        protected internal override void OnResume()
#endif
        {
            base.OnResume();
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnCover()
#else
        protected internal override void OnCover()
#endif
        {
            base.OnCover();
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnReveal()
#else
        protected internal override void OnReveal()
#endif
        {
            base.OnReveal();
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnRefocus(object userData)
#else
        protected internal override void OnRefocus(object userData)
#endif
        {
            base.OnRefocus(userData);
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
#else
        protected internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
#endif
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
#else
        protected internal override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
#endif
        {
            int oldDepth = Depth;
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);
            int deltaDepth = UGuiGroupHelper.DepthFactor * uiGroupDepth + DepthFactor * depthInUIGroup - oldDepth +
                             OriginalDepth;
            GetComponentsInChildren(true, _cachedCanvasContainer);
            for (int i = 0; i < _cachedCanvasContainer.Count; i++)
            {
                _cachedCanvasContainer[i].sortingOrder += deltaDepth;
            }

            _cachedCanvasContainer.Clear();
        }
    }
}