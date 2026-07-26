using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LODTest : MonoBehaviour
{
    private LODGroup lodGroup;
    
    private void Awake()
    {
        lodGroup = GetComponent<LODGroup>();
    }

    // Start is called before the first frame update
    void Start()
    {
        lodGroup.ForceLOD(1);
    }
}
