using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace Fumiki
{
    /// <summary>
    /// uGUI 界面组辅助器
    /// </summary>
    public class UGuiGroupHelper : UIGroupHelperBase
    {
        public const int DepthFactor = 10000;

        private int _depth = 0;
        private Canvas _cachedCanvas = null;

        /// <summary>
        /// 设置界面组深度
        /// </summary>
        /// <param name="depth">界面组深度</param>
        public override void SetDepth(int depth)
        {
            _depth = depth;
            _cachedCanvas.overrideSorting = true;
            _cachedCanvas.sortingOrder = DepthFactor * depth;
        }

        private void Awake()
        {
            _cachedCanvas = gameObject.GetOrAddComponent<Canvas>();
            gameObject.GetOrAddComponent<GraphicRaycaster>();
        }

        private void Start()
        {
            _cachedCanvas.overrideSorting = true;
            _cachedCanvas.sortingOrder = DepthFactor * _depth;

            RectTransform transform = GetComponent<RectTransform>();
            transform.anchorMin = Vector2.zero;
            transform.anchorMax = Vector2.one;
            transform.anchoredPosition = Vector2.zero;
            transform.sizeDelta = Vector2.zero;
        }
    }
}
