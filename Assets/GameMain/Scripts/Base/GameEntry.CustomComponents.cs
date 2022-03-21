namespace Fumiki
{
    public sealed partial class GameEntry
    {
        public static SpriteCollectionComponent SpriteCollection { get; private set; }
        public static StaticResourceComponent StaticResource { get; private set; }
        public static TextureSetComponent TextureSet { get; private set; }
        public static TimerComponent Timer { get; private set; }
        
        private void InitCustomComponents()
        {
            SpriteCollection = UnityGameFramework.Runtime.GameEntry.GetComponent<SpriteCollectionComponent>();
            StaticResource = UnityGameFramework.Runtime.GameEntry.GetComponent<StaticResourceComponent>();
            TextureSet = UnityGameFramework.Runtime.GameEntry.GetComponent<TextureSetComponent>();
            Timer = UnityGameFramework.Runtime.GameEntry.GetComponent<TimerComponent>();
        }
    }
}