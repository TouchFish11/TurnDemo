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
        public int Priority => 1;
        
        public Task InitModuleAsync()
        {
            LogManager.Log($"{nameof(AnimationModule)}.{nameof(InitModuleAsync)}:Animation module initialization completed");
            return Task.CompletedTask;
        }

        public INormalAnimationComponent AddNormalAnimationComponent(IEntityObject entityObject)
        {
            if (entityObject != null) return entityObject.AddComponent<NormalAnimationComponent>();
            LogManager.LogError($"{nameof(AnimationModule)}.{nameof(AddNormalAnimationComponent)}: entityObject is null");
            return null;
        }
    }
}
