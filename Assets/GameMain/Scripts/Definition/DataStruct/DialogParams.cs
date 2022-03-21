using GameFramework;

namespace Fumiki
{
    /// <summary>
    /// 对话框显示数据
    /// </summary>
    public class DialogParams
    {
        /// <summary>
        /// 模式，即按钮数量取值 1、2、3
        /// </summary>
        public int Mode { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 按钮1的文本
        /// </summary>
        public string Button1Text { get; set; }

        /// <summary>
        /// 按钮1回调
        /// </summary>
        public GameFrameworkAction<object> OnButton1Down { get; set; }

        /// <summary>
        /// 按钮2文本
        /// </summary>
        public string Button2Text { get; set; }

        /// <summary>
        /// 按钮2回调
        /// </summary>
        public GameFrameworkAction<object> OnButton2Down { get; set; }

        /// <summary>
        /// 按钮3文本
        /// </summary>
        public string Button3Text { get; set; }

        /// <summary>
        /// 按钮3回调
        /// </summary>
        public GameFrameworkAction<object> OnButton3Down { get; set; }

        /// <summary>
        /// 用户自定义数据
        /// </summary>
        public string UserData { get; set; }

        /// <summary>
        /// 是否隐藏关闭按钮
        /// </summary>
        public bool HideClose { get; set; }
    }
}