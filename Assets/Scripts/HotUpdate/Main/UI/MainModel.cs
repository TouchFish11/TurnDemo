using System.Collections.Generic;
using Core.Loader.Object;
using Core.Service;
using Core.UI.MVC;
using HotUpdate.Interact.UI;

namespace HotUpdate.Main.UI
{
    /// <summary>
    /// 主界面数据
    /// </summary>
    public class MainModel : UIModel
    {
        // 交互UI列表
        private readonly List<InteractUI> interactUIs = new();

        /// <summary>
        /// 缓存交互UI对象
        /// </summary>
        /// <param name="interactUIs"></param>
        public void CacheInteracts(List<InteractUI> interactUIs)
        {
            foreach (var interactUI in this.interactUIs)
            {
                // 释放资源
                ServiceLocator.Get<IPrefabLoader>().CollectAsset(interactUI.gameObject);
            }
            this.interactUIs.Clear();
            this.interactUIs.AddRange(interactUIs);
        }
    }
}
