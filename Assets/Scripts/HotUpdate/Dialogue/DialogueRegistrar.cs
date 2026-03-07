using Core.Service;
using HotUpdate.Core.Dialogue;
using HotUpdate.Core.Manager;

namespace HotUpdate.Dialogue
{
    /// <summary>
    /// 对话模块注册器
    /// </summary>
    public class DialogueRegistrar : IGameServiceRegistrar
    {
        public void RegisterServices()
        {
            ServiceLocator.Register<IDialogueManager>(DialogueManager.Instance);
        }
    }
}
