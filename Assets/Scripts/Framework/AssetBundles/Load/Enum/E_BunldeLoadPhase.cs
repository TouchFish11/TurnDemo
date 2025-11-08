
namespace Framework
{
    /// <summary>
    /// AB包加载阶段
    /// </summary>
    public enum E_BunldeLoadPhase
    {
        None,
        /// <summary>
        /// 即将加载
        /// </summary>
        Start,
        /// <summary>
        /// 加载中
        /// </summary>
        Loading,
        /// <summary>
        /// 加载成功（加载失败为Start）
        /// </summary>
        Finish,
    }
}