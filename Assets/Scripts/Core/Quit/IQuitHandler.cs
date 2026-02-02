using System;
using System.Threading.Tasks;

namespace Core.Quit
{
    /// <summary>
    /// 退出处理器接口
    /// </summary>
    public interface IQuitHandler
    {
        event Func<Task> OnAppQuit;

        void ActiveHandler();
    }
}
