using UnityEngine;

namespace Zenject
{
    public class GuiRenderer : MonoBehaviour
    {
        private GuiRenderableManager _renderableManager;

        public void OnGUI()
        {
            _renderableManager.OnGui();
        }

        [Inject]
        private void Construct(GuiRenderableManager renderableManager)
        {
            _renderableManager = renderableManager;
        }
    }
}