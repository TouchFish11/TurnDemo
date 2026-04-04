using System;

namespace HotUpdate.Core.Data
{
    public interface IData<out T> where T : class
    {
        event Action<T> OnDataChanged;
    }
}
