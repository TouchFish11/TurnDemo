using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ×é¼þÅäÖÃ
/// </summary>
[CreateAssetMenu()]
public class ComponentConfig : ScriptableObject
{
    public List<int> compnentIds = new List<int>();

    public bool netMoveComponent;

    public bool localMoveComponent;

    public bool animComponent;

    public bool skillComponent;

    public bool propertyComponent;

    public bool uiComponent;

    private void OnValidate()
    {
        compnentIds.Clear();

        if (netMoveComponent)
        {
            compnentIds.Add(1);
        }
        else
        {
            compnentIds.Remove(1);
        }

        if(localMoveComponent)
        {
            compnentIds.Add(2);
        }
        else
        {
            compnentIds.Remove(2);
        }

        if (animComponent)
        {
            compnentIds.Add(3);
        }
        else
        {
            compnentIds.Remove(3);
        }

        if (skillComponent)
        {
            compnentIds.Add(4);
        }
        else
        {
            compnentIds.Remove(4);
        }

        if (propertyComponent)
        {
            compnentIds.Add(5);
        }
        else
        {
            compnentIds.Remove(5);
        }

        if (uiComponent)
        {
            compnentIds.Add(6);
        }
        else
        {
            compnentIds.Remove(6);
        }
    }
}
