using System.IO;
using Config.ActivityConfigSO;
using Core.Serialize.Json;
using UnityEditor;
using UnityEngine;

namespace Editor.Json
{
    public class SOToJsonExporter : EditorWindow
    {
        [MenuItem("GameTool/Export SO To JSON")]
        public static void ExportSelectedSOToJson()
        {
            var selected = Selection.activeObject as ActivityConfig;
            if (selected == null)
            {
                Debug.LogError("请先选择一个ActivityConfigSO");
                return;
            }

            string json;
            if (selected is BattleActivityConfig battleActivityConfig)
            {
                // 序列化数据
                json = JsonManager.Instance.ToJson(battleActivityConfig.BattleConfigEntryColletion);
            }
            else
            {
                Debug.LogError($"不存在该类型的SO：{selected}");
                return;
            }
            
            // 保存到文件
            var path = EditorUtility.SaveFilePanel(
                "保存JSON文件",
                $"{Application.dataPath}/Editor/ArtRes/GameConfig/",
                $"{selected.GetType()}",
                "json");

            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            
            File.WriteAllText(path, json);
            Debug.Log($"已导出到: {path}");
            
            // 刷新AssetDatabase
            AssetDatabase.Refresh();
        }
    }
}