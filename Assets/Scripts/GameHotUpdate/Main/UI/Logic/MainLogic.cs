namespace GameHotUpdate.Main.UI.Logic
{
    /// <summary>
    /// �������߼���
    /// </summary>
    public abstract class MainLogic
    {
        protected MainController mainController;
        protected MainModel mainModel;
        protected MainView mainView;

        protected MainLogic(MainController mainController, MainModel mainModel, MainView mainView)
        {
            this.mainController = mainController;
            this.mainModel = mainModel;
            this.mainView = mainView;
        }

        /// <summary>
        /// ��ʼ��
        /// </summary>
        public abstract void Init();

        /// <summary>
        /// ת��
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T As<T>() where T : MainLogic => this as T;
        
        public virtual void Dispose()
        {
            mainController = null;
            mainModel = null;
            mainView = null;
        }
    }
}
