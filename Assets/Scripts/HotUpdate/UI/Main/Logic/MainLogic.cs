using System;
using System.Threading.Tasks;
using Core.Pool;

namespace HotUpdate.UI.Main.Logic
{
    /// <summary>
    /// 主界面逻辑
    /// </summary>
    public abstract class MainLogic : IPoolData
    {
        protected MainController mainController;
        protected MainView mainView;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="mainController"></param>
        /// <param name="mainView"></param>
        public Task Init(MainController mainController, MainView mainView)
        {
            if (mainController == null || !mainView)
                throw new ArgumentNullException($"[{nameof(MainLogic)}]: {nameof(mainController)}={mainController == null},{nameof(mainView)}={!mainView}");
            
            this.mainController = mainController;
            this.mainView = mainView;
            return OnInit();
        }

        /// <summary>
        /// 初始化后执行
        /// </summary>
        protected abstract Task OnInit();

        void IPoolData.ResetData()
        {
            OnResetData();
            mainController = null;
            mainView = null;
        }

        protected abstract void OnResetData();
    }
}
