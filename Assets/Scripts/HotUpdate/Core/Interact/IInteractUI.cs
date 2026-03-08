using UnityEngine;

namespace HotUpdate.Core.Interact
{
    public interface IInteractUI
    {
        GameObject GameObject { get; }
        void Init(string text);
    }
}
