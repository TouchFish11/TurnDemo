using System;
using System.Collections.Generic;

namespace Framework.InputManager
{
    /// <summary>
    /// 输入数据容器类
    /// </summary>
    [Serializable]
    public sealed class InputDataContainer
    {
        //存储输入数据字典
        private Dictionary<E_EventType, InputData> _inputDataDic = new Dictionary<E_EventType, InputData>();

        /// <summary>
        /// 输入数据字典
        /// </summary>
        public Dictionary<E_EventType, InputData> InputDataDic { get { return _inputDataDic; } set { _inputDataDic = value; } }
    }
}
