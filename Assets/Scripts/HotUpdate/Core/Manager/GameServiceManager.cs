using System.Collections.Generic;
using Core.GlobalEvent;
using Core.Service;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Dialogue;
using HotUpdate.Core.Input;
using HotUpdate.Core.Main;
using HotUpdate.Core.Task;
using HotUpdate.Core.VFX;
using UnityEngine.VFX;

namespace HotUpdate.Core.Manager
{
    /// <summary>
    /// 游戏服务管理器
    /// </summary>
    public class GameServiceManager
    {
        private readonly List<IGameServiceRegistrar> _registrars = new();
    
        public void AddRegistrar(IGameServiceRegistrar registrar)
        {
            _registrars.Add(registrar);
        }
    
        public void InitService()
        {
            foreach (var registrar in _registrars)
            {
                registrar.RegisterServices();
            }
        }
    }
}
