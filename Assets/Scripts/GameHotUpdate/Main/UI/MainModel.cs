using System.Collections.Generic;
using Core.Pool;
using Core.Service;
using Core.UI.MVC;
using GameHotUpdate.Interact.UI;

namespace GameHotUpdate.Main.UI
{
    /// <summary>
    /// ����������
    /// </summary>
    public class MainModel : UIModel
    {
        // ����UI����
        private readonly List<InteractUI> interactUIs = new();

        /// <summary>
        /// ���ý���
        /// </summary>
        /// <param name="interactUIs"></param>
        public void CacheInteracts(List<InteractUI> interactUIs)
        {
            foreach (var interactUI in this.interactUIs)
            {
                ServiceLocator.Get<IPoolManager>().PushObj(interactUI.gameObject);
            }
            
            this.interactUIs.Clear();
            this.interactUIs.AddRange(interactUIs);
        }
    }
}
