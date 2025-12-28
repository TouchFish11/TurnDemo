using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 编辑器资源管理器接口
/// </summary>
public interface IEditorResManager
{
    Dictionary<string, Sprite> LoadAllSprite(string spritesName);
    T LoadEditorAsset<T>(string assetName, string suffixName = "") where T : Object;
    Sprite LoadSprite(string spritesName, string spriteName);
}
