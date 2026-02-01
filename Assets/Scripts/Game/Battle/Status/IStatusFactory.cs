using Core.Reflection;

namespace Game.Battle.Status
{
    public interface IStatusFactory : IFactory
    {
        /// <summary>
        /// 通过ID获取状态
        /// 内部会通过反射创建状态对象，而不是复用之前对象
        /// </summary>
        /// <returns></returns>
        IStatus GetStatus(int statusId);
    }
}
