using System;
using System.Collections.Generic;
using System.Linq;

public struct TreeInfo
{
    public int Height { get; private set; }
    public int ChildrenPerNode { get; private set; }
    public ulong NodesCount { get; private set; }
    public ulong LeavesCount { get; private set; }
    public ulong BaseNodesCount { get; private set; }
    public ulong BaseLeavesCount { get; private set; }

    public static TreeInfo Build(int height, int childrenPerNode)
    {
        var tree = new TreeInfo {
            Height = height,
            ChildrenPerNode = childrenPerNode,
        }.Setup();

        // Debug.Log($"Tree: height={tree.Height} perNode={tree.ChildrenPerNode} base={tree.BaseNodesCount}");

        return tree;
    }

    public static TreeInfo FindSmallestTreeThatContains(int objectsCount, int height)
    {
        return EnumerateTreesWithDepth(height, max: objectsCount)
            .TakeWhile(tree => tree.NodesCount <= (ulong) objectsCount)
            .Last();
    }

    private static IEnumerable<TreeInfo> EnumerateTreesWithDepth(int depth, int max=(int)1e8)
    {
        int childrenPerNode = 1;
        while (childrenPerNode <= max)
        {
            yield return TreeInfo.Build(depth, childrenPerNode++);
        }
    }

    public TreeInfo Setup()
    {
        ulong[] layers = EnumerateLayers().ToArray();
        // Debug.Log($"Layers: {string.Join(", ", layers)}");
        NodesCount = Sum(layers);
        LeavesCount = layers[^1];
        BaseNodesCount = NodesCount - LeavesCount;
        BaseLeavesCount = Height > 1 ? layers[^2] : layers[0];
        return this;
    }

    private ulong Sum(ulong[] layers)
    {
        ulong sum = 0;
        foreach (ulong layer in layers)
        {
            sum += layer;
        }
        return sum;
    }

    private IEnumerable<ulong> EnumerateLayers()
    {
        ulong layerSize = 1;
        for (ulong layerInx = 0; layerInx < (ulong)Height; layerInx++)
        {
            yield return layerSize;
            layerSize *= (ulong)ChildrenPerNode;
        }
    }

    public override string ToString()
    {
        return $"TreeInfo: {ChildrenPerNode}^{Height} count={NodesCount} leaves={LeavesCount} | [BaseNodes] count={BaseNodesCount} laves={BaseLeavesCount}";
    }
}
