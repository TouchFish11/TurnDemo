using System.Threading.Tasks;
using HotUpdate.Core.Module;

namespace HotUpdate.Dialogue
{
    public class DialogueModule : IModule
    {
        public Task InitModuleAsync()
        {
            return Task.CompletedTask;
        }
    }
}
