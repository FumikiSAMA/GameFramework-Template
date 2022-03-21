using Sirenix.OdinInspector;

namespace Fumiki
{
    public sealed partial class GameEntry : SerializedMonoBehaviour
    {
        private void Start()
        {
            InitFrameworkComponents();
            InitCustomComponents();
        }
    }
}
