namespace Core.Mono.MonoFunction
{
    public interface IDestroyable
    {
        /// <summary>
        /// 在销毁时
        /// </summary>
        /// <returns></returns>
        void OnDestroy();
    }
}
