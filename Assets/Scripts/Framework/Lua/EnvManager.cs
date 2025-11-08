using UnityEngine;
using XLua;

namespace Framework
{
    /// <summary>
    /// lua管理器
    /// </summary>
    public class EnvManager : SingletonBase<EnvManager>
    {
        //lua解析器
        private readonly LuaEnv _env;

        private EnvManager()
        {
            _env = new LuaEnv();
            _env.AddLoader(CustomLuaLoader);
            DoLuaFile("Main");
        }

        /// <summary>
        /// 执行lua语言
        /// </summary>
        /// <param name="luaStr"></param>
        public void Dostring(string luaStr)
        {
            if (_env == null)
            {
                LogMgr.LogError("lua解析器为null");
                return;
            }
            _env.DoString(luaStr);
        }

        /// <summary>
        /// 执行lua脚本
        /// </summary>
        /// <param name="fileName"></param>
        public void DoLuaFile(string fileName)
        {
            if (_env == null)
            {
                LogMgr.LogError("lua解析器为null");
                return;
            }
            _env.DoString(string.Format("require('{0}')", fileName));
        }

        /// <summary>
        /// lua垃圾回收
        /// </summary>
        public void Tick()
        {
            if (_env == null)
            {
                LogMgr.LogError("lua解析器为null");
                return;
            }
            _env.Tick();
        }

        /// <summary>
        /// 销毁lua解析器
        /// </summary>
        public void Dispose()
        {
            if (_env == null)
            {
                LogMgr.LogError("lua解析器为null");
                return;
            }
            _env.Dispose();
        }

        /// <summary>
        /// 自定义Lua加载器
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private byte[] CustomLuaLoader(ref string fileName)
        {
            TextAsset text = AssetBundleLoadManager.Instance.LoadAsset<TextAsset>(E_AssetBundleType.Lua, $"{fileName}.lua");
            if (text != null)
            {
                return text.bytes;
            }
            else
            {
                LogMgr.LogError($"AB包中的lua文件获取失败，文件名：{fileName}");
                return null;
            }
        }

        /// <summary>
        /// 大G表
        /// </summary>
        public LuaTable Global => _env.Global;
    }
}
