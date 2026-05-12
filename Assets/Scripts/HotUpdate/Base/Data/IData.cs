using System;

namespace HotUpdate.Base.Data
{
    public interface IData<out T> where T : class
    {
        event Action<T> OnDataChanged;
    }
}
