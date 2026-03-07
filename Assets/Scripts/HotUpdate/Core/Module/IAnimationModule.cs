using Core.Components;
using HotUpdate.Core.Animation;

namespace HotUpdate.Core.Module
{
    public interface IAnimationModule : IModule
    {
        INormalAnimationComponent AddNormalAnimationComponent(IEntityObject entityObject);
    }
}
