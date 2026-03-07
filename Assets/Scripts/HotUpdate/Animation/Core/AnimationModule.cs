using System.Threading.Tasks;
using Core.Components;
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
        public Task InitModuleAsync()
        {
            
            return Task.CompletedTask;
        }

        public INormalAnimationComponent AddNormalAnimationComponent(IEntityObject entityObject)
        {
            return entityObject.AddComponent<NormalAnimationComponent>();
        }
    }
}
