using System;

namespace Framework
{
    /// <summary>
    /// 输入动作数据
    /// </summary>
    [Serializable]
    public class InputActionData
    {
        public string Up { get; } = "<Keyboard>/w";
        public string Down { get; } = "<Keyboard>/s";
        public string Left { get; } = "<Keyboard>/a";
        public string Right { get; } = "<Keyboard>/d";
        public string Attack { get; } = "<Mouse>/leftButton";
    }
}
