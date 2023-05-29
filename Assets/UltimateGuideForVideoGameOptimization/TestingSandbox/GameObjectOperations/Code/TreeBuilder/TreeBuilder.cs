using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TreeBuilder
{
    private static string TreeNodeTag => TreeNode.Tag;

    public static GameObject Build(
        int depth = 1,
        int objectsCount = 20,
        int childrenPerNode = 0,
        GameObject prefab = null,
        Transform parent = null,
        float positionNoiseRate = 0f,
        float rotationNoiseRate = 0f,
        float scaleNoiseRate = 0f,
        string namePrefix = null)
    {
        NodeBuilder nodeBuilder = new(positionNoiseRate, rotationNoiseRate, scaleNoiseRate, TreeNodeTag);
        int height = depth + 1;
        TreeInfo tree = childrenPerNode > 0
            ? TreeInfo.Build(height, childrenPerNode)
            : TreeInfo.FindSmallestTreeThatContains(objectsCount, height);

        Debug.Log($"TreeInfo: ${tree}");
        
        GameObject root = BuildBaseTree(tree, prefab, parent, nodeBuilder);
        BuildLeaves(tree, objectsCount, prefab, root, nodeBuilder);

        string spec = $"{nodeBuilder.TotalGenerated} {tree.ChildrenPerNode}^{tree.Height}";
        root.name = $"{spec} | {root.name}";
        if (namePrefix != null) root.name = $"{namePrefix} | {root.name}";
        return root;
    }

    private static GameObject BuildBaseTree(TreeInfo tree, GameObject prefab, Transform parent, NodeBuilder nodeBuilder)
    {
        GameObject root = nodeBuilder.Build(prefab, parent);
        Queue<GameObject> queue = new();
        queue.Enqueue(root);
        ulong countCreated = 1;
        while (countCreated < tree.BaseNodesCount)
        {
            GameObject node = queue.Dequeue();
            for (int i = 0; i < tree.ChildrenPerNode; i++)
            {
                GameObject child = nodeBuilder.Build(prefab, parent: node.transform);
                queue.Enqueue(child);
                countCreated++;
            }
        }
        return root;
    }

    private static void BuildLeaves(TreeInfo tree, int objectsCount, GameObject prefab, GameObject root, NodeBuilder nodeBuilder)
    {
        GameObject[] baseLeaves = TreeNode.EachLeaf(root).Select(node => node.GameObject).ToArray();

        ulong leavesCount = (ulong)objectsCount - tree.BaseNodesCount;
        ulong minLeavesPerBaseLeaf = leavesCount / tree.BaseLeavesCount;
        ulong amountOfBaseLeavesWithExtraLeaf = leavesCount - ((ulong)baseLeaves.Length * minLeavesPerBaseLeaf);

        for (ulong baseLeafInx = 0; baseLeafInx < (ulong)baseLeaves.Length; baseLeafInx++)
        {
            GameObject baseLeaf = baseLeaves[baseLeafInx];
            ulong childrenAmount = minLeavesPerBaseLeaf;
            if (baseLeafInx < amountOfBaseLeavesWithExtraLeaf) childrenAmount++;
            for (ulong i = 0; i < childrenAmount; i++)
            {
                nodeBuilder.Build(prefab, parent: baseLeaf.transform);
            }
        }
    }
}
