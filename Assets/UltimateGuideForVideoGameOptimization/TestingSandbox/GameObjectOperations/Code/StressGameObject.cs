using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BenchmarkReport;
using Sirenix.OdinInspector;
using UnityEngine;

public class StressGameObject : MonoBehaviour
{
    [SerializeField] private GameObject _nodePrefab;
    [SerializeField] private int _shallowDepth = 3;
    [SerializeField] private int _deepDepth = 7;
    [SerializeField] private int _fullDepth = 100;
    [SerializeField] private int _smallAmount = 30;
    [SerializeField] private int _bigAmount = 1000;
    [Range(0, 1)] [SerializeField] private float _positionNoise = 1f;
    [Range(0, 1)] [SerializeField] private float _rotationNoise = 1f;
    [Range(0, 1)] [SerializeField] private float _scaleNoise = 1f;
    [SerializeField] private int _iterations = 5000;
    [SerializeField] private int _moveIterations = 500;
    [SerializeField] private int _movingAverageSize = 300;

    [SerializeField] private Transform _singleObj;
    [SerializeField] private Transform _singleObj_Op2;
    [SerializeField] private Transform _singleObj_Op3;
    [SerializeField] private Transform _leaf_DeepTree;
    [SerializeField] private Transform _leaf_DeepTree_Op2;
    [SerializeField] private Transform _leaf_DeepTree_Op3;
    [SerializeField] private Transform _root_DeepTree;
    [SerializeField] private Transform _root_DeepTree_Op2;
    [SerializeField] private Transform _root_DeepTree_Op3;
    [SerializeField] private Transform _leaf_DeepTree_WithNoise;
    [SerializeField] private Transform _root_DeepTree_WithNoise;
    [SerializeField] private Transform _leaf_DeepTree_FullDeep;
    [SerializeField] private Transform _root_DeepTree_FullDeep;
    [SerializeField] private Transform _leaf_ShallowTree;
    [SerializeField] private Transform _leaf_Depth_1_Tree;
    [SerializeField] private Transform _leaf_Depth_2_Tree;
    [SerializeField] private Transform _leaf_Depth_3_Tree;
    [SerializeField] private Transform _leaf_Depth_4_Tree;
    [SerializeField] private Transform _leaf_Depth_5_Tree;
    [SerializeField] private Transform _leaf_Depth_6_Tree;
    [SerializeField] private Transform _leaf_Depth_7_Tree;
    [SerializeField] private Transform _leaf_Depth_8_Tree;
    [SerializeField] private Transform _leaf_Depth_9_Tree;
    [SerializeField] private Transform _leaf_Depth_10_Tree;
    [SerializeField] private Transform _root_ShallowTree;
    [SerializeField] private Transform _root_SmallTree;
    [SerializeField] private Transform _leaf_SmallTree;
    [SerializeField] private Transform _root_FlatTree;
    [SerializeField] private Transform[] _leaves_FlatTree;
    [SerializeField] private Transform _move_RootDeep_Deep2Deep_Dest_A;
    [SerializeField] private Transform _move_RootDeep_Deep2Deep_Dest_B;
    [SerializeField] private Transform _move_RootDeep_Deep2Deep_Node;
    [SerializeField] private Transform _move_RootDeep_Flat2Flat_Dest_A;
    [SerializeField] private Transform _move_RootDeep_Flat2Flat_Dest_B;
    [SerializeField] private Transform _move_RootDeep_Flat2Flat_Node;
    [SerializeField] private Transform _move_RootDeep_Single2Single_Dest_A;
    [SerializeField] private Transform _move_RootDeep_Single2Single_Dest_B;
    [SerializeField] private Transform _move_RootDeep_Single2Single_Node;
    [SerializeField] private Transform _move_RootFlat_Single2Single_Dest_A;
    [SerializeField] private Transform _move_RootFlat_Single2Single_Dest_B;
    [SerializeField] private Transform _move_RootFlat_Single2Single_Node;

    [SerializeField] private Transform _move_Single_Deep2Deep_Dest_A;
    [SerializeField] private Transform _move_Single_Deep2Deep_Dest_B;
    [SerializeField] private Transform _move_Single_Deep2Deep_Node;
    [SerializeField] private Transform _move_Single_Flat2Flat_Dest_A;
    [SerializeField] private Transform _move_Single_Flat2Flat_Dest_B;
    [SerializeField] private Transform _move_Single_Flat2Flat_Node;
    [SerializeField] private Transform _move_Single_Flat2Flat_Big_Dest_A;
    [SerializeField] private Transform _move_Single_Flat2Flat_Big_Dest_B;
    [SerializeField] private Transform _move_Single_Flat2Flat_Big_Node;
    [SerializeField] private Transform _move_Single_Single2Single_Dest_A;
    [SerializeField] private Transform _move_Single_Single2Single_Dest_B;
    [SerializeField] private Transform _move_Single_Single2Single_Node;
    [SerializeField] private List<Transform> _generatedTrees = new();
    private BenchmarkRunner _runner;
    private Vector3 _movement;
    private Vector3[] _movements;
    private Rigidbody _componentHolder;

//    private void Awake()
//    {
//        _movements = Enumerable.Range(0, 100)
//            .Select(_ => UnityEngine.Random.insideUnitSphere + Vector3.one * 0.1f)
//            .ToArray();
//        _runner = new BenchmarkRunner(
//            target: this,
//            iterations: _iterations,
//            movingAverageSize: _movingAverageSize,
//            noneCall: nameof(Bench_None),
//            prepareForCall: PrepareForBenchCall
//        )
//        .AddCall(nameof(Bench_GetComponent))
//        .AddCall(nameof(Bench_SingleObject), isReference: true)
//        //.AddCall(nameof(Bench_SingleObject_Op2), isReference: true)
//        //.AddCall(nameof(Bench_SingleObject_Op3), isReference: true)
//        //.AddCall(nameof(Bench_Leaf_SmallTree), isReference: true)
//        .AddCall(nameof(Bench_Leaf_ShallowTree), isReference: true)
//        //.AddCall(nameof(Bench_Leaf_DeepTree_WithNoise))
//        .AddCall(nameof(Bench_Leaf_DeepTree))
//        //.AddCall(nameof(Bench_Leaf_DeepTree_Op2))
//        //.AddCall(nameof(Bench_Leaf_DeepTree_Op3));
//        .AddCall(nameof(Bench_Leaf_DeepTree_FullDeep))
//        .AddCall(nameof(Bench_Leaf_Depth_1_Tree))
//        .AddCall(nameof(Bench_Leaf_Depth_2_Tree))
//        .AddCall(nameof(Bench_Leaf_Depth_3_Tree))
//        .AddCall(nameof(Bench_Leaf_Depth_4_Tree))
//        //.AddCall(nameof(Bench_Leaf_Depth_5_Tree))
//        //.AddCall(nameof(Bench_Leaf_Depth_6_Tree))
//        //.AddCall(nameof(Bench_Leaf_Depth_7_Tree))
//        //.AddCall(nameof(Bench_Leaf_Depth_8_Tree))
//        //.AddCall(nameof(Bench_Leaf_Depth_9_Tree))
//        .AddCall(nameof(Bench_Leaf_Depth_10_Tree))
//
//        .AddCall(nameof(Bench_Move_RootDeep_Deep2Deep), iterations: _moveIterations)
//        .AddCall(nameof(Bench_Move_RootDeep_Flat2Flat), iterations: _moveIterations)
//        .AddCall(nameof(Bench_Move_RootDeep_Single2Single), iterations: _moveIterations, isReference: true)
//        .AddCall(nameof(Bench_Move_RootFlat_Single2Single), iterations: _moveIterations)
//        .AddCall(nameof(Bench_Move_Single_Deep2Deep), iterations: _moveIterations)
//        .AddCall(nameof(Bench_Move_Single_Flat2Flat), iterations: _moveIterations)
//        .AddCall(nameof(Bench_Move_Single_Flat2Flat_Big), iterations: _moveIterations)
//        .AddCall(nameof(Bench_Move_Single_Single2Single), iterations: _moveIterations, isReference: true);
//
//        //.AddCall(nameof(Bench_Leaves_FlatTree))
//
//        //.AddCall(nameof(Bench_Root_FlatTree), isReference: true)
//        //.AddCall(nameof(Bench_Root_SmallTree))
//        //.AddCall(nameof(Bench_Root_ShallowTree))
//        //.AddCall(nameof(Bench_Root_DeepTree_WithNoise))
//        //.AddCall(nameof(Bench_Root_DeepTree), isReference: true)
//        //.AddCall(nameof(Bench_Root_DeepTree_Op2), isReference: true)
//        //.AddCall(nameof(Bench_Root_DeepTree_Op3), isReference: true)
//        //.AddCall(nameof(Bench_Root_DeepTree_FullDeep));
//
//        FindObjectOfType<BenchmarkReportUIRefresher>().SetRunner(_runner);
//    }
//
//    private void Update()
//    {
//        if (!Input.GetKey(KeyCode.Space)) return;
//        _runner.Run();
//        // for (int i = 0; i < _iterations; i++)
//        // {
//        //     PrepareForBenchCall(i);
//        //     RunAllBenchMethods();
//        // }
//    }
//
//    private void RunAllBenchMethods()
//    {
//        Bench_SingleObject();
//        Bench_Leaves_FlatTree();
//        Bench_Root_FlatTree();
//        Bench_Leaf_SmallTree();
//        Bench_Root_SmallTree();
//        Bench_Root_ShallowTree();
//        Bench_Leaf_ShallowTree();
//        Bench_Root_DeepTree_FullDeep();
//        Bench_Root_DeepTree_WithNoise();
//        Bench_Leaf_DeepTree_WithNoise();
//        Bench_Root_DeepTree();
//        Bench_Leaf_DeepTree();
//        Bench_SingleObject();
//    }
//
//    private void PrepareForBenchCall(int i)
//    {
//        _movement = _movements[i % _movements.Length];
//    }
//
//    private void OnDisable()
//    {
//        Debug.Log(_componentHolder);
//    }
//
//    private void Bench_None()
//    {
//        DoNothing();
//    }
//
//    private void Bench_GetComponent()
//    {
//        DoGetComponent();
//    }
//
//    private void Bench_SingleObject()
//    {
//        ApplyOperationOn(_singleObj);
//    }
//    private void Bench_SingleObject_Op2()
//    {
//        ApplyOperation2On(_singleObj_Op2);
//    }
//    private void Bench_SingleObject_Op3()
//    {
//        ApplyOperation3On(_singleObj_Op3);
//    }
//    private void Bench_Leaf_DeepTree()
//    {
//        ApplyOperationOn(_leaf_DeepTree);
//    }
//    private void Bench_Leaf_DeepTree_Op2()
//    {
//        ApplyOperation2On(_leaf_DeepTree_Op2);
//    }
//    private void Bench_Leaf_DeepTree_Op3()
//    {
//        ApplyOperation3On(_leaf_DeepTree_Op3);
//    }
//    private void Bench_Root_DeepTree()
//    {
//        ApplyOperationOn(_root_DeepTree);
//    }
//    private void Bench_Root_DeepTree_Op2()
//    {
//        ApplyOperation2On(_root_DeepTree_Op2);
//    }
//    private void Bench_Root_DeepTree_Op3()
//    {
//        ApplyOperation3On(_root_DeepTree_Op3);
//    }
//    private void Bench_Leaf_DeepTree_WithNoise()
//    {
//        ApplyOperationOn(_leaf_DeepTree_WithNoise);
//    }
//    private void Bench_Root_DeepTree_WithNoise()
//    {
//        ApplyOperationOn(_root_DeepTree_WithNoise);
//    }
//    private void Bench_Root_DeepTree_FullDeep()
//    {
//        ApplyOperationOn(_root_DeepTree_FullDeep);
//    }
//    private void Bench_Leaf_DeepTree_FullDeep()
//    {
//        ApplyOperationOn(_leaf_DeepTree_FullDeep);
//    }
//    private void Bench_Leaf_ShallowTree()
//    {
//        ApplyOperationOn(_leaf_ShallowTree);
//    }
//    private void Bench_Leaf_Depth_1_Tree()
//    {
//        ApplyOperationOn(_leaf_Depth_1_Tree);
//    }
//    private void Bench_Leaf_Depth_2_Tree()
//    {
//        ApplyOperationOn(_leaf_Depth_2_Tree);
//    }
//    private void Bench_Leaf_Depth_3_Tree()
//    {
//        ApplyOperationOn(_leaf_Depth_3_Tree);
//    }
//    private void Bench_Leaf_Depth_4_Tree()
//    {
//        ApplyOperationOn(_leaf_Depth_4_Tree);
//    }
//    private void Bench_Leaf_Depth_5_Tree()
//    {
//        ApplyOperationOn(_leaf_Depth_5_Tree);
//    }
//    private void Bench_Leaf_Depth_6_Tree()
//    {
//        ApplyOperationOn(_leaf_Depth_6_Tree);
//    }
//    private void Bench_Leaf_Depth_7_Tree()
//    {
//        ApplyOperationOn(_leaf_Depth_7_Tree);
//    }
//    private void Bench_Leaf_Depth_8_Tree()
//    {
//        ApplyOperationOn(_leaf_Depth_8_Tree);
//    }
//    private void Bench_Leaf_Depth_9_Tree()
//    {
//        ApplyOperationOn(_leaf_Depth_9_Tree);
//    }
//    private void Bench_Leaf_Depth_10_Tree()
//    {
//        ApplyOperationOn(_leaf_Depth_10_Tree);
//    }
//    private void Bench_Root_ShallowTree()
//    {
//        ApplyOperationOn(_root_ShallowTree);
//    }
//    private void Bench_Root_SmallTree()
//    {
//        ApplyOperationOn(_root_SmallTree);
//    }
//    private void Bench_Leaf_SmallTree()
//    {
//        ApplyOperationOn(_leaf_SmallTree);
//    }
//    private void Bench_Root_FlatTree()
//    {
//        ApplyOperationOn(_root_FlatTree);
//    }
//    private void Bench_Move_RootDeep_Deep2Deep()
//    {
//        MoveBetween(
//            node: _move_RootDeep_Deep2Deep_Node,
//            destA: _move_RootDeep_Deep2Deep_Dest_A,
//            destB: _move_RootDeep_Deep2Deep_Dest_B
//        );
//    }
//    private void Bench_Move_Single_Deep2Deep()
//    {
//        MoveBetween(
//            node: _move_Single_Deep2Deep_Node,
//            destA: _move_Single_Deep2Deep_Dest_A,
//            destB: _move_Single_Deep2Deep_Dest_B
//        );
//    }
//
//    private void Bench_Move_RootDeep_Flat2Flat()
//    {
//        MoveBetween(
//            node: _move_RootDeep_Flat2Flat_Node,
//            destA: _move_RootDeep_Flat2Flat_Dest_A,
//            destB: _move_RootDeep_Flat2Flat_Dest_B
//        );
//    }
//
//    private void Bench_Move_RootDeep_Single2Single()
//    {
//        MoveBetween(
//            node:  _move_RootDeep_Single2Single_Node,
//            destA: _move_RootDeep_Single2Single_Dest_A,
//            destB: _move_RootDeep_Single2Single_Dest_B
//        );
//    }
//
//    private void Bench_Move_Single_Flat2Flat()
//    {
//        MoveBetween(
//            node: _move_Single_Flat2Flat_Node,
//            destA: _move_Single_Flat2Flat_Dest_A,
//            destB: _move_Single_Flat2Flat_Dest_B
//        );
//    }
//
//    private void Bench_Move_Single_Flat2Flat_Big()
//    {
//        MoveBetween(
//            node:  _move_Single_Flat2Flat_Big_Node,
//            destA: _move_Single_Flat2Flat_Big_Dest_A,
//            destB: _move_Single_Flat2Flat_Big_Dest_B
//        );
//    }
//
//    private void Bench_Move_RootFlat_Single2Single()
//    {
//        MoveBetween(
//            node:  _move_RootFlat_Single2Single_Node,
//            destA: _move_RootFlat_Single2Single_Dest_A,
//            destB: _move_RootFlat_Single2Single_Dest_B
//        );
//    }
//
//    private void Bench_Move_Single_Single2Single()
//    {
//        MoveBetween(
//            node: _move_Single_Single2Single_Node,
//            destA: _move_Single_Single2Single_Dest_A,
//            destB: _move_Single_Single2Single_Dest_B
//        );
//    }
//
//    private void DoGetComponent()
//    {
//        _componentHolder = GetComponent<Rigidbody>();
//    }
//
//    private void DoNothing()
//    {
//        _componentHolder = null;
//    }
//
//    private void MoveBetween(Transform node, Transform destA, Transform destB)
//    {
//        node.SetParent(node.parent == destA ? destB : destA);
//    }
//    private void Bench_Leaves_FlatTree()
//    {
//        foreach (Transform obj in _leaves_FlatTree)
//        {
//            ApplyOperationOn(obj);
//        }
//    }
//
//    private void ApplyOperationOn(Transform obj)
//    {
//        obj.Translate(_movement);
//    }
//
//    private void ApplyOperation2On(Transform obj)
//    {
//        obj.Rotate(_movement);
//    }
//
//    private void ApplyOperation3On(Transform obj)
//    {
//        obj.localScale = _movement;
//    }
//
//#region Build
//    [Button]
//    public void BuildTrees()
//    {
//        ClearGenerated();
//
//        _singleObj = Build_SingleObj();
//        _singleObj_Op2 = Build_SingleObj();
//        _singleObj_Op3 = Build_SingleObj();
//        _root_DeepTree = Build_DeepTree();
//        _root_DeepTree_Op2 = Build_DeepTree();
//        _root_DeepTree_Op3 = Build_DeepTree();
//        _root_DeepTree_WithNoise = Build_DeepTree_WithNoise();
//        _root_DeepTree_FullDeep = Build_DeepTree_FullDeep();
//        _root_ShallowTree = Build_ShallowTree();
//        _root_SmallTree = Build_SmallTree();
//        _root_FlatTree = Build_FlatTree();
//
//        _leaf_DeepTree = GetLeaf(Build_DeepTree());
//        _leaf_DeepTree_Op2 = GetLeaf(Build_DeepTree());
//        _leaf_DeepTree_Op3 = GetLeaf(Build_DeepTree());
//        _leaf_DeepTree_WithNoise = GetLeaf(Build_DeepTree_WithNoise());
//        _leaf_DeepTree_FullDeep = GetLeaf(Build_DeepTree_FullDeep());
//        _leaf_ShallowTree = GetLeaf(Build_ShallowTree());
//        _leaf_Depth_1_Tree = GetLeaf(Build_DepthTree_Small(1));
//        _leaf_Depth_2_Tree = GetLeaf(Build_DepthTree_Small(2));
//        _leaf_Depth_3_Tree = GetLeaf(Build_DepthTree_Small(3));
//        _leaf_Depth_4_Tree = GetLeaf(Build_DepthTree_Small(4));
//        _leaf_Depth_5_Tree = GetLeaf(Build_DepthTree_Small(5));
//        _leaf_Depth_6_Tree = GetLeaf(Build_DepthTree_Small(6));
//        _leaf_Depth_7_Tree = GetLeaf(Build_DepthTree_Small(7));
//        _leaf_Depth_8_Tree = GetLeaf(Build_DepthTree_Small(8));
//        _leaf_Depth_9_Tree = GetLeaf(Build_DepthTree_Small(9));
//        _leaf_Depth_10_Tree = GetLeaf(Build_DepthTree_Small(10));
//        _leaf_SmallTree = GetLeaf(Build_SmallTree());
//        _leaves_FlatTree = GetLeaves(Build_FlatTree());
//
//        _move_RootDeep_Deep2Deep_Dest_A = GetLeaf(Build_DeepTree_Small());
//        _move_RootDeep_Deep2Deep_Dest_B = GetLeaf(Build_DeepTree_Small());
//        _move_RootDeep_Deep2Deep_Node = Build_DeepTree_Small();
//
//        _move_RootDeep_Flat2Flat_Dest_A = GetLeaf(Build_FlatTree_Small());
//        _move_RootDeep_Flat2Flat_Dest_B = GetLeaf(Build_FlatTree_Small());
//        _move_RootDeep_Flat2Flat_Node = Build_DeepTree_Small();
//
//        _move_RootDeep_Single2Single_Dest_A = Build_SingleObj();
//        _move_RootDeep_Single2Single_Dest_B = Build_SingleObj();
//        _move_RootDeep_Single2Single_Node = Build_DeepTree_Small();
//
//        _move_RootFlat_Single2Single_Dest_A = Build_SingleObj();
//        _move_RootFlat_Single2Single_Dest_B = Build_SingleObj();
//        _move_RootFlat_Single2Single_Node = Build_FlatTree_Small();
//
//        _move_Single_Single2Single_Dest_A = Build_SingleObj();
//        _move_Single_Single2Single_Dest_B = Build_SingleObj();
//        _move_Single_Single2Single_Node = Build_SingleObj();
//
//        _move_Single_Flat2Flat_Dest_A = GetLeaf(Build_FlatTree_Small());
//        _move_Single_Flat2Flat_Dest_B = GetLeaf(Build_FlatTree_Small());
//        _move_Single_Flat2Flat_Node = Build_SingleObj();
//
//        _move_Single_Flat2Flat_Big_Dest_A = GetLeaf(Build_FlatTree());
//        _move_Single_Flat2Flat_Big_Dest_B = GetLeaf(Build_FlatTree());
//        _move_Single_Flat2Flat_Big_Node = Build_SingleObj();
//
//        _move_Single_Deep2Deep_Dest_A = GetLeaf(Build_DeepTree_Small());
//        _move_Single_Deep2Deep_Dest_B = GetLeaf(Build_DeepTree_Small());
//        _move_Single_Deep2Deep_Node = Build_SingleObj();
//    }
//
//    private Transform Build_SingleObj()
//    {
//        GameObject obj = _nodePrefab == null ? new GameObject() : Instantiate(_nodePrefab);
//        obj.name = "SingleObj";
//        return RecordTree(obj.transform);
//    }
//
//    private Transform Build_DeepTree()
//    {
//        return RecordTree(TreeBuilder.Build(
//            namePrefix: "DeepTree",
//            prefab: _nodePrefab,
//            depth: _deepDepth,
//            objectsCount: _bigAmount
//        ).transform);
//    }
//
//    private Transform Build_DeepTree_Small()
//    {
//        return RecordTree(TreeBuilder.Build(
//            namePrefix: "DeepTree_Small",
//            prefab: _nodePrefab,
//            depth: _deepDepth,
//            objectsCount: _smallAmount
//        ).transform);
//    }
//
//    private Transform Build_DeepTree_WithNoise()
//    {
//        return RecordTree(TreeBuilder.Build(
//            namePrefix: "DeepTree_WithNoise",
//            prefab: _nodePrefab,
//            depth: _deepDepth,
//            objectsCount: _bigAmount,
//            positionNoiseRate: _positionNoise,
//            rotationNoiseRate: _rotationNoise,
//            scaleNoiseRate: _scaleNoise
//        ).transform);
//    }
//
//    private Transform Build_DeepTree_FullDeep()
//    {
//        return RecordTree(TreeBuilder.Build(
//            namePrefix: "DeepTree_FullDeep",
//            prefab: _nodePrefab,
//            depth: _fullDepth,
//            objectsCount: _bigAmount
//        ).transform);
//    }
//
//    private Transform Build_ShallowTree()
//    {
//        return RecordTree(TreeBuilder.Build(
//            namePrefix: "ShallowTree",
//            prefab: _nodePrefab,
//            depth: _shallowDepth,
//            objectsCount: _bigAmount
//        ).transform);
//    }
//
//    private Transform Build_DepthTree_Small(int depth)
//    {
//        return RecordTree(TreeBuilder.Build(
//            namePrefix: $"Depth_{depth}_Tree",
//            prefab: _nodePrefab,
//            depth: depth,
//            objectsCount: _smallAmount
//        ).transform);
//    }
//
//    private Transform Build_SmallTree()
//    {
//        return RecordTree(TreeBuilder.Build(
//            namePrefix: "SmallTree",
//            prefab: _nodePrefab,
//            depth: _shallowDepth,
//            objectsCount: _smallAmount
//        ).transform);
//    }
//
//    private Transform Build_FlatTree()
//    {
//        return RecordTree(TreeBuilder.Build(
//            namePrefix: "FlatTree",
//            prefab: _nodePrefab,
//            depth: 1,
//            objectsCount: _bigAmount
//        ).transform);
//    }
//
//    private Transform Build_FlatTree_Small()
//    {
//        return RecordTree(TreeBuilder.Build(
//            namePrefix: "FlatTree_Small",
//            prefab: _nodePrefab,
//            depth: 1,
//            objectsCount: _smallAmount
//        ).transform);
//    }
//
//    private Transform GetLeaf(Transform transform)
//    {
//        return TreeNode
//            .EachLeaf(transform)
//            .OrderBy(_ => UnityEngine.Random.value)
//            .First()
//            .Transform;
//    }
//
//    private Transform[] GetLeaves(Transform transform)
//    {
//        return TreeNode
//            .EachLeaf(transform)
//            .Select(node => node.Transform)
//            .ToArray();
//    }
//
//    private void ClearGenerated()
//    {
//        foreach (Transform tree in _generatedTrees)
//        {
//            if (tree == null || tree.gameObject == null) continue;
//            DestroyImmediate(tree.gameObject);
//        }
//        _generatedTrees.Clear();
//    }
//
//    private Transform RecordTree(Transform root)
//    {
//        _generatedTrees.Add(root);
//        return root;
//    }
//#endregion    
}