using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// ≈‰÷√º”‘ÿ∆˜
/// </summary>
public abstract class ConfigLoader : IConfigLoader
{
    public abstract T GetConfig<T>() where T : class;

    public abstract Task LoadConfig();
}
