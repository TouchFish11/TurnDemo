using System.Collections.Generic;
using Core.Loader.UI;
using Core.Pool;
using Core.Service;
using Core.UI.MVC;
using GameHotUpdate.Config;
using GameHotUpdate.Interact.UI;
using UnityEngine;

namespace GameHotUpdate.Main.UI
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
                ServiceLocator.Get<IUiLoader>().RealseAsset(AbKeyCollection.Ui, interactUI.gameObject);
            }
            
            this.interactUIs.Clear();
            this.interactUIs.AddRange(interactUIs);
        }
    }
}
