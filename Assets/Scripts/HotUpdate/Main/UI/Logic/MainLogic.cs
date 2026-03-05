using Core.Pool;

namespace HotUpdate.Main.UI.Logic
{
    /// <summary>
    /// 主界面逻辑
    /// </summary>
    public abstract class MainLogic : IPoolData
    {
        protected MainController mainController;
        protected MainModel mainModel;
        protected MainView mainView;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="mainController"></param>
        /// <param name="mainModel"></param>
        /// <param name="mainView"></param>
        public virtual void Init(MainController mainController, MainModel mainModel, MainView mainView)
        {
            this.mainController = mainController;
            this.mainModel = mainModel;
            this.mainView = mainView;
        }

        public virtual void ResetData()
        {
            mainController = null;
            mainModel = null;
            mainView = null;
        }
    }
}
