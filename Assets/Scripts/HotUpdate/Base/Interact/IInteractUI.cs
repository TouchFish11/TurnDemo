using UnityEngine;

namespace HotUpdate.Base.Interact
{
    public interface IInteractUI
    {
        GameObject GameObject { get; }
        void Init(string text);
    }
}
