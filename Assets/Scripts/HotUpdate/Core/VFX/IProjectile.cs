namespace HotUpdate.Core.VFX
{
    public interface IProjectile
    {
        /// <summary>
        /// 初始化抛射物核心数据
        /// </summary>
        /// <param name="projectileData">抛射物配置数据</param>
        /// <param name="vFXInfo">特效配置信息</param>
        void Init(ProjectileData projectileData, VFXInfo vFXInfo);
    }
}
