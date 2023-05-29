using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct TreeNode
{
    public static readonly string Tag = "TreeNode";
    public Transform Transform;
    public GameObject GameObject => Transform.gameObject;
    public int ChildCount;
    public int LayerInx;

    public static IEnumerable<TreeNode> EachNode(Transform root)
    {
        if (!root.CompareTag(Tag))
        {
            yield break;
        }
        Queue<(Transform nodeTransform, int depth)> queue = new();
        queue.Enqueue((root, depth: 0));
        while (queue.Any())
        {
            (Transform nodeTransform, int depth) = queue.Dequeue();
            int childCount = 0;
            foreach (Transform child in nodeTransform)
            {
                if (child.CompareTag(Tag))
                {
                    queue.Enqueue((child, depth + 1));
                    childCount++;
                }
            }
            yield return new TreeNode {
                Transform = nodeTransform,
                ChildCount = childCount,
                LayerInx = depth
            };
        }
    }

    public static IEnumerable<TreeNode> EachLeaf(Transform root)
    {
        return EachNode(root).Where(node => node.ChildCount == 0);
    }

    public static IEnumerable<TreeNode> EachNode(GameObject root) => EachNode(root.transform);
    public static IEnumerable<TreeNode> EachLeaf(GameObject root) => EachLeaf(root.transform);
}
