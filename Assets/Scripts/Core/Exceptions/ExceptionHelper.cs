using System;

namespace Core.Exceptions
{
    /// <summary>
    /// 异常辅助器
    /// </summary>
    public static class ExceptionHelper
    {
        /// <summary>
        /// 前缀
        /// </summary>
        public const string Prefix = "系统错误";

        /// <summary>
        /// 封装未定义异常的通用抛出逻辑
        /// </summary>
        /// <param name="msg">额外的携带消息</param>
        /// <param name="inner">内部异常，若有</param>
        /// <returns></returns>
        public static Exception Throw(string msg, Exception inner = null)
        {
            return new Exception(msg, inner);
        }
        
        public static AssetBundleLoadException ThrowAssetBundleLoadException(string bundleName, Exception innerException)
        {
            var exceptionMsg = $"[{Prefix} {ExceptionCode.AssetBundleLoadErrorCode}]: Load '{bundleName}' assetBundle fail";
            return new AssetBundleLoadException(bundleName, ExceptionCode.AssetBundleLoadErrorCode, exceptionMsg, innerException);
        }

        public static AssetLoadException ThrowAssetLoadException(string assetKey, Exception innerException)
        {
            var exceptionMsg = $"[{Prefix} {ExceptionCode.AssetLoadErrorCode}]: Load '{assetKey}' asset fail";
            return new AssetLoadException(assetKey, ExceptionCode.AssetLoadErrorCode, exceptionMsg, innerException);
        }
        
        public static AssetsLoadException ThrowAssetsLoadException(string bundleName, Type type, Exception innerException)
        {
            var exceptionMsg = $"[{Prefix} {ExceptionCode.AssetsLoadErrorCode}]: Load '{bundleName}' all {type} type asset fail";
            return new AssetsLoadException(bundleName, type, ExceptionCode.AssetsLoadErrorCode, exceptionMsg, innerException);
        }
        
        public static AssetBundleUnloadException ThrowAssetBundleUnloadException(string bundleName, uint refCount, Exception innerException)
        {
            var exceptionMsg = $"[{Prefix} {ExceptionCode.AssetBundleUnloadErrorCode}]: Load '{bundleName}' is unload fail, final refCount is {refCount}";
            return new AssetBundleUnloadException(bundleName, refCount, ExceptionCode.AssetBundleUnloadErrorCode, exceptionMsg, innerException);
        }
        
        public static InvalidHandleAccessException ThrowInvalidHandleAccessException(int handleId, int version, Exception innerException)
        {
            var exceptionMsg = $"[{Prefix} {ExceptionCode.InvalidHandleAccessErrorCode}]: Asset handle(id = {handleId}, version = {version} ) not valid";
            return new InvalidHandleAccessException(handleId, version, ExceptionCode.InvalidHandleAccessErrorCode, exceptionMsg, innerException);
        }

        public static UICreateException ThrowUICreateException(Type uiType, Exception innerException)
        {
            var exceptionMsg = $"[{Prefix} {ExceptionCode.UIInitializeErrorCode}]: {uiType} Create/Init panel error";
            return new UICreateException(uiType, ExceptionCode.UIInitializeErrorCode, exceptionMsg, innerException);
        }

        public static EventTriggerException ThrowEventTriggerException(Type eventType, Exception innerException)
        {
            var exceptionMsg = $"[{Prefix} {ExceptionCode.EventTriggerErrorCode}]: event type: {eventType}";
            return new EventTriggerException(ExceptionCode.EventTriggerErrorCode, exceptionMsg, innerException);
        }
    }
}
