using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class HierarchyCreator : MonoBehaviour
{
    [Button]
    public void GenerateTree(
        int depth = 1,
        int objectsCount = 20,
        int childrenPerNode = 0,
        GameObject prefab = null,
        Transform parent = null,
        float positionNoiseRate = 0f,
        float rotationNoiseRate = 0f,
        float scaleNoiseRate = 0f
    )
    {
        TreeBuilder.Build(
            depth,
            objectsCount,
            childrenPerNode,
            prefab,
            parent ?? this.transform,
            positionNoiseRate,
            rotationNoiseRate,
            scaleNoiseRate
        );
    }
}
