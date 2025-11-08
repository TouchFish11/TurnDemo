using Framework.InputManager;
using System.Collections;
using UnityEngine.Events;

namespace Framework
{
    /// <summary>
    /// 游戏数据管理器
    /// </summary>
    public class GameDataMgr : SingletonBase<GameDataMgr>
    {
        //音乐数据
        private MusicData _musicData;
        //输入系统动作数据容器对象
        private InputSystem.InputActionContainer _inputActionContainer;
        //输入系统输入数据容器对象
        private InputDataContainer _inputDataContainer;

        private GameDataMgr() { }

        /// <summary>
        /// 初始化数据(Main主函数最先调用)
        /// </summary>
        /// <param name="overCallBack"></param>
        /// <returns></returns>
        public IEnumerator InitData(UnityAction overCallBack)
        {
            //读取表数据
            yield return BinaryDataMgr.Instance.InitTableInfo();
            //读取Json数据
             yield return JsonManager.Instance.LoadJsonData();
            //读取音乐数据
            _musicData = BinaryDataMgr.Instance.Load<MusicData>(FileUtility.LocalMusicDataFileName);
            //读取输入动作数据
            _inputActionContainer = BinaryDataMgr.Instance.Load<InputSystem.InputActionContainer>(FileUtility.LocalInputDataFileName);
            //数据加载结束回调
            overCallBack?.Invoke();
        }

        /// <summary>
        /// 音乐数据
        /// </summary>
        public MusicData MusicData { get => _musicData; }

        /// <summary>
        /// 输入系统动作数据容器
        /// </summary>
        public InputSystem.InputActionContainer InputActionContainer { get => _inputActionContainer; }

        /// <summary>
        /// 输入系统输入数据容器
        /// </summary>
        public InputDataContainer InputDataContainer { get => _inputDataContainer; }

    }
}
