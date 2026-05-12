using System;

namespace HotUpdate.Base.Scene
{
    public static class SceneGeneratorHelper
    {
        private static ISceneGenerator _sceneGenerator;

        public static void Init(ISceneGenerator sceneGenerator)
        {
            _sceneGenerator = sceneGenerator;
        }
        
        public static ISceneGenerator GetSceneGenerator()
        {
            return _sceneGenerator ?? throw new NullReferenceException($"{nameof(SceneGeneratorHelper)}.{nameof(GetSceneGenerator)}：SceneGenerator没有初始化");
        }
    }
}
