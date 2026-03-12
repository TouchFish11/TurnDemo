using System.Threading.Tasks;
using Core.Components;
using Core.Log;
using HotUpdate.Animation.Component;
using HotUpdate.Core.Animation;
using HotUpdate.Core.Module;

namespace HotUpdate.Animation.Core
{
    /// <summary>
    /// 动画模块
    /// </summary>
    public class AnimationModule : IAnimationModule
    {
        public int Priority => 0;
        
        public Task InitModuleAsync()
        {
            LogManager.Log($"{nameof(AnimationModule)}.{nameof(InitModuleAsync)}：初始化完成");
            return Task.CompletedTask;
        }

        public INormalAnimationComponent AddNormalAnimationComponent(IEntityObject entityObject)
        {
            return entityObject.AddComponent<NormalAnimationComponent>();
        }
    }
}
