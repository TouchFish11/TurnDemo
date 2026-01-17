using System.Threading.Tasks;

/// <summary>
/// 玩家管理器接口
/// </summary>
public interface IPlayerManager
{
    IEntityObject MainPlayer { get; }

    /// <summary>
    /// 创建玩家用户
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    Task CreatePlayer(uint uid);

    /// <summary>
    /// 清理玩家
    /// </summary>
    void Clear();
}
