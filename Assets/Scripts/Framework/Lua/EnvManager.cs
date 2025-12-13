using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using XLua;

namespace Framework
{
    /// <summary>
    /// lua管理器
    /// </summary>
    public class EnvManager : SingletonBase<EnvManager>
    {
        // 文件名到lua的映射
        private readonly Dictionary<string, byte[]> _nameToLuaMap = new Dictionary<string, byte[]>();
        // lua解析器
        private readonly LuaEnv _env;

        private EnvManager()
        {
            _env = new LuaEnv();
        }

        /// <summary>
        /// 异步初始化Lua
        /// </summary>
        /// <param name="fileNames"></param>
        /// <returns></returns>
        public async Task InitLuaAsync(params string[] fileNames)
        {
            if (fileNames == null || fileNames.Length == 0)
            {
                return;
            }

            foreach (string fileName in fileNames)
            {
                TextAsset luaAsset = await AssetBundleManager.Instance.LoadAssetAsync<TextAsset>(E_AssetBundleType.Lua, $"{fileName}.lua");
                _nameToLuaMap.Add(fileName, luaAsset.bytes);
            }
            // 添加加载器
            _env.AddLoader(CustomLuaLoader);
            // 执行主模块
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
                LogManager.LogError("lua解析器为null");
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
                LogManager.LogError("lua解析器为null");
                return;
            }
            _env.DoString($"require('{fileName}')");
        }

        /// <summary>
        /// lua垃圾回收
        /// </summary>
        public void Tick()
        {
            if (_env == null)
            {
                LogManager.LogError("lua解析器为null");
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
                LogManager.LogError("lua解析器为null");
                return;
            }
            _env.Dispose();
        }

        /// <summary>
        /// 清理
        /// </summary>
        public void Clear()
        {
            _nameToLuaMap.Clear();
            Tick();
        }

        /// <summary>
        /// 自定义Lua加载器
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private byte[] CustomLuaLoader(ref string fileName)
        {
            // 从缓存中获取Lua数据
            if (_nameToLuaMap.TryGetValue(fileName, out var lua))
            {
                return lua;
            }
            else
            {
                LogManager.LogError($"lua文件加载失败，文件名：{fileName}");
                return null;
            }
        }

        /// <summary>
        /// 大G表
        /// </summary>
        public LuaTable Global => _env.Global;
    }
}
