using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BenchmarkReport;
using UnityEngine;

namespace StressBenchmark
{
    [Flags]
    public enum Juice {Apple, Orange, Pineapple, Guava, Grape, Acai, Caju, Manga, Banana, Mamao}
    public class StressBenchmark : MonoBehaviour
    {
        // TODO:
        // 1. Test moving multiple objects at once (with root object) vs moving them one by one
        // 2. Test moving object that's very deep in the hierarchy vs object that's in root
        // 3. Test moving a complex hierarchy (create automatically based on amount of objects and depth)
        //      see what degrades the performance the most (more elements? more depth?)
        [SerializeField] private int _iterations = 5000;
        [SerializeField] private int _elementsSqrtCount = 75;
        [SerializeField] private int _movingAverageSize = 300;
        [SerializeField] private int _dictSmallSize = 25;
        [SerializeField] private int _dictBigSize = 5000;
        [SerializeField] private int _shortKeySize = 25;
        [SerializeField] private int _longKeySize = 100;
        [SerializeField] private BenchmarkReportUIRefresher _reportRefresher;
        private Component _component;
        private GameObject _obj;
        private IEnumerable<int> _storedEnumerable;
        private IEnumerator<int> _storedEnumerator;
        private Action _storedAction;
        private Action _storedAction10x;
        private event Action _storedEvent;
        private event Action _storedEvent10x;
        private readonly Dictionary<int, Component> _bigDictInt = new();
        private readonly Dictionary<int, Component> _foreachDictInt = new();
        private readonly Dictionary<string, Component> _bigDictShortStr = new();
        private readonly Dictionary<string, Component> _bigDictLongStr = new();
        private readonly Dictionary<string, Component> _smallDictSmallStr = new();
        private readonly Dictionary<Juice, Component> _bigDictEnum = new();
        private List<Component> _list = new();
        private Component[] _array;
        private Component[][] _jaggedArray;
        private Juice _enumKey;
        private Component[,] _multiArray;
        private string[] _smallCountShortKeysDict;
        private string[] _bigCountShortKeysDict;
        private string[] _bigCountLongKeysDict;
        private int[] _bigDictIntKeys;
        private int[] _elementsIntKeys;
        private int[] _elementsMultiIntKeys1;
        private int[] _elementsMultiIntKeys2;
        private string _smallStrKeyMiss;
        private string _bigStrKeyMiss;
        private object[] NoArgs;
        private MethodInfo _runLogicDirectlyMethod;
        private int _elementsCount;
        private IEnumerator<int> _enumerableRangeEnumerator;
        private BenchmarkRunner _runner;
        private int _benchCallInx;
        private int _benchArrayKey;
        private int _benchMultiArrayKey1;
        private int _benchMultiArrayKey2;
        private string _benchSmallCountShortKey;
        private string _benchBigCountLongKey;
        private string _benchBigCountShortKey;
        private int _benchBigDictIntKey;

        private void Awake()
        {
            NoArgs = new object[0];
            _runLogicDirectlyMethod = GetRunDirectlyMethod();
            _elementsCount = _elementsSqrtCount * _elementsSqrtCount;
            _obj = this.gameObject;
            _storedEnumerable = EnumerableMethod();
            _storedEnumerator = EnumeratorMethod();
            AppendDelegates(ref _storedAction, Logic, times: 1);
            AppendDelegates(ref _storedAction10x, Logic, times: 10);
            AppendEvents(ref _storedEvent, Logic, times: 1);
            AppendEvents(ref _storedEvent10x, Logic, times: 10);
            _smallCountShortKeysDict = GenerateDictKeys(amount: 100, domainSize: _dictSmallSize, keyFunc: BuildShortKey);
            _bigCountShortKeysDict = GenerateDictKeys(amount: 100, domainSize: _dictBigSize, keyFunc: BuildShortKey);
            _bigCountLongKeysDict = GenerateDictKeys(amount: 100, domainSize: _dictBigSize, keyFunc: BuildLongKey);
            _bigDictIntKeys = GenerateDictKeys(amount: 100, domainSize: _dictBigSize, keyFunc: i => i);
            _elementsIntKeys = GenerateDictKeys(amount: 100, domainSize: _elementsCount, keyFunc: i => i);
            _elementsMultiIntKeys1 = GenerateDictKeys(amount: 100, domainSize: _elementsSqrtCount, keyFunc: i => i);
            _elementsMultiIntKeys2 = GenerateDictKeys(amount: 100, domainSize: _elementsSqrtCount, keyFunc: i => i);
            _smallStrKeyMiss = BuildShortKey(257002123);
            _bigStrKeyMiss = BuildLongKey(712314183);
            _enumKey = Juice.Pineapple | Juice.Orange;
            _array = new Component[_elementsCount];
            _multiArray = new Component[_elementsSqrtCount, _elementsSqrtCount];
            _jaggedArray = new Component[_elementsSqrtCount][];
            Component item = null; // this
            for (int i = 0; i < _dictSmallSize; i++)
            {
                _smallDictSmallStr.Add(BuildShortKey(i), item);
            }
            for (int i = 0; i < _dictBigSize; i++)
            {
                _bigDictShortStr.Add(BuildShortKey(i), item);
                _bigDictLongStr.Add(BuildLongKey(i), item);
                _bigDictInt.Add(i, item);
                _bigDictEnum.Add((Juice)i, item);
            }
            for (int i = 0; i < _elementsSqrtCount; i++)
            {
                _jaggedArray[i] = new Component[_elementsSqrtCount];
            }
            for (int i = 0; i < _elementsCount; i++)
            {
                _list.Add(item);
                _array[i] = item;
                _multiArray[i % _elementsSqrtCount, i / _elementsSqrtCount] = item;
                _jaggedArray[i % _elementsSqrtCount][i / _elementsSqrtCount] = item;
                _foreachDictInt.Add(i, item);
            }
            _runner = new BenchmarkRunner(
                iterations: _iterations,
                movingAverageSize: _movingAverageSize,
                prepareForCall: PrepareForBenchCall
            )
            //.AddCall(nameof(Bench_GetComponentInChildren))
            //.AddCall(nameof(Bench_GetComponent_Rigidbody2D))
            .AddCall("GetMeshRenderer", () => GetComponent<MeshRenderer>(), isReference: true)
            .AddCall("Nothing", () => {})
            //.AddCall("MethodCall", () => Logic(_obj))
            .AddCall("Instance_Call_Direct", () => Logic(), isReference: true)
            .AddCall("Static_Call_Direct", () => StaticLogic())
            .AddCall("Instance_Call_Reflection_Stored", () => _runLogicDirectlyMethod.Invoke(this, NoArgs))
            .AddCall("Instance_Call_Reflection", () => GetRunDirectlyMethod().Invoke(this, NoArgs))
            .AddCall("Static_Call_AnonymousArrow", () => {
                Action action = () => StaticLogic();
                action();
            })
            .AddCall("Instance_Call_AnonymousArrow", () => {
                Action action = () => Logic();
                action();
            })

            .AddCall("StoredEvents_Instance_Call_StoredEvent10x",
                () => _storedEvent10x(),
                iterations: _iterations / 10,
                timeMultiplier: 0.1f)
            .AddCall("StoredEvents_Instance_Call_StoredEvent", () => _storedEvent())
            .AddCall("StoredEvents_Instance_Call_StoredAction", () => _storedAction())
            .AddCall("StoredEvents_Instance_Call_StoredAction10x",
                () => _storedAction10x(),
                iterations: _iterations / 10,
                timeMultiplier: 0.1f)

            .AddCall("Collections_Array", 
                () => _component = _array[_benchArrayKey],
                isReference: true)
            .AddCall("Collections_MultiArray",
                () => _component = _multiArray[_benchMultiArrayKey1, _benchMultiArrayKey2])
            .AddCall("Collections_JaggedArray",
                () => _component = _jaggedArray[_benchMultiArrayKey1][_benchMultiArrayKey2])
            .AddCall("Collections_List",
                () => _component =_list[_benchArrayKey])
            .AddCall("Collections_Dictionary_BigStr_Try_Hit",
                () => _component = _bigDictLongStr[_benchBigCountLongKey])
            .AddCall("Collections_Dictionary_SmallStr_Try_Hit",
                () => _component = _bigDictShortStr.GetValueOrDefault(_benchBigCountShortKey))
            .AddCall("Collections_Dictionary_SmallStr_SmallDict_Try_Hit",
                () => _component = _smallDictSmallStr.GetValueOrDefault(_benchSmallCountShortKey))
            .AddCall("Collections_Dictionary_Int_Try_Hit",
                () => _component = _bigDictInt.GetValueOrDefault(_benchBigDictIntKey), isReference: true);

            //.AddCall(nameof(Bench_Enumerations_List_Foreach))
            //.AddCall(nameof(Bench_Enumerations_List_For))
            //.AddCall(nameof(Bench_Enumerations_Array_Foreach))
            //.AddCall(nameof(Bench_Enumerations_Array_For), isReference: true)
            //.AddCall(nameof(Bench_Enumerations_Dictionary_Int_Foreach_Values))
            //.AddCall(nameof(Bench_Enumerations_Dictionary_Int_Foreach_Keys));


            // EXTREMELLY SLOW
            //.AddCall(nameof(Bench_Enumerations_Enumerator_DeepMethod_MoveNext))
            //.AddCall(nameof(Bench_Enumerations_Enumerable_DeepMethod_Foreach))
            //.AddCall(nameof(Bench_Enumerations_Enumerator_Method_MoveNext))
            //.AddCall(nameof(Bench_Enumerations_Enumerable_Method_Foreach))
            //.AddCall(nameof(Bench_Enumerations_Enumerable_Stored_Foreach))
            //.AddCall(nameof(Bench_Enumerations_Enumerable_Range))
        }

        private void DoNothing()
        {
        }

        private static MethodInfo GetRunDirectlyMethod()
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            return typeof(StressBenchmark).GetMethod(nameof(DoNothing), flags);
        }

        private void PrepareForBenchCall(int inx)
        {
            _benchSmallCountShortKey = _smallCountShortKeysDict[_benchCallInx % _smallCountShortKeysDict.Length];
            _benchBigCountLongKey = _bigCountLongKeysDict[_benchCallInx % _bigCountLongKeysDict.Length];
            _benchBigCountShortKey = _bigCountShortKeysDict[_benchCallInx % _bigCountShortKeysDict.Length];
            _benchArrayKey = _elementsIntKeys[_benchCallInx % _elementsIntKeys.Length];
            _benchMultiArrayKey1 = _elementsMultiIntKeys1[_benchCallInx % _elementsMultiIntKeys1.Length];
            _benchMultiArrayKey2 = _elementsMultiIntKeys2[_benchCallInx % _elementsMultiIntKeys2.Length];
            _benchBigDictIntKey = _bigDictIntKeys[_benchCallInx % _bigDictIntKeys.Length];

            _benchCallInx = (_benchCallInx + 1) % 10000;
        }

        private T[] GenerateDictKeys<T>(int amount, int domainSize, Func<int, T> keyFunc)
        {
            return Enumerable.Range(1, amount)
                .Select(_ => UnityEngine.Random.Range(0, domainSize))
                .Select(keyFunc)
                .OrderBy((_) => UnityEngine.Random.value)
                .ToArray();
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(3f);
            _reportRefresher.SetRunner(_runner);
        }

        private string BuildLongKey(int i) => StringGenerator.Build(_longKeySize, suffix: $"-{i}", seed: 379 * (i + 1));

        private string BuildShortKey(int i) => StringGenerator.Build(_shortKeySize, suffix: $"-{i}", seed: 127 * (i + 1));

        public void Update()
        {
            if (!Input.GetKey(KeyCode.Space)) return;
            _runner.Run();
        }

//        private Component Bench_Instance_Call_MethodParameter()
//        {
//            return RunLogic(Logic);
//        }
//
//        private Component Bench_Instance_Call_AnonymousArrow()
//        {
//            return RunLogic((obj) => Logic(obj));
//        }
//
//        private Component Bench_Instance_Call_Reflection()
//        {
//            return (Component) GetRunDirectlyMethod().Invoke(this, _emptyParams);
//        }
//
//        private Component Bench_Instance_Call_Reflection_Stored()
//        {
//            return (Component) _runLogicDirectlyMethod.Invoke(this, _emptyParams);
//        }
//
//#region Static
//        private Component Bench_Static_Call_AnonymousArrow()
//        {
//            return RunLogic((obj) => StaticLogic(obj));
//        }
//#endregion        
//
//
//#region StoredEvents
//        private Component Bench_StoredEvents_Instance_Call_StoredEvent10x()
//        {
//            return RunLogic(_storedEvent10x);
//        }
//        private Component Bench_StoredEvents_Instance_Call_StoredEvent_10times()
//        {
//            return RunLogic10times(_storedEvent);
//        }
//#endregion

#region Collections
        private Component Bench_Enumerations_Dictionary_Int_Foreach_Values()
        {
            Component c = default;
            foreach (Component item in _foreachDictInt.Values)
            {
                c = item;
            }
            return c;
        }

        private Component Bench_Enumerations_Dictionary_Int_Foreach_Keys()
        {
            int key = 0;
            foreach (int item in _foreachDictInt.Keys)
            {
                key = item;
            }
            return null;
        }
        #endregion

        #region Enumerations
        private Component Bench_Enumerations_Array_For()
        {
            for (int i = 0; i < _array.Length; i++)
            {
                _component = _array[i];
            }
            return _component;
        }

        private Component Bench_Enumerations_Array_Foreach()
        {
            foreach (var item in _array)
            {
                _component = item;
            }
            return _component;
        }

        private Component Bench_Enumerations_List_For()
        {
            for (int i = 0; i < _list.Count; i++)
            {
                _component = _list[i];
            }
            return _component;
        }

        private Component Bench_Enumerations_List_Foreach()
        {
            foreach (var item in _list)
            {
                _component = item;
            }
            return _component;
        }

        private Component Bench_Enumerations_Enumerable_Range()
        {
            return RunEnumerableRangeForeach();
        }

        private Component Bench_Enumerations_Enumerable_Method_Foreach()
        {
            return RunEnumerableMethodForeach();
        }

        private Component Bench_Enumerations_Enumerable_Stored_Foreach()
        {
            return RunEnumerableStoredForeach();
        }


        private Component Bench_Enumerations_Enumerable_ReusableEnumerator_Foreach()
        {
            return RunEnumerableReusableEnumeratorForeach();
        }

        private Component Bench_Enumerations_Enumerator_Method_MoveNext()
        {
            return RunEnumeratorMethodMoveNext();
        }

        private Component Bench_Enumerations_Enumerator_Stored_MoveNext()
        {
            return RunEnumeratorStoredMoveNext();
        }

        private Component Bench_Enumerations_Enumerable_DeepMethod_Foreach()
        {
            return RunEnumerableDeepMethodForeach();
        }

        private Component Bench_Enumerations_Enumerator_DeepMethod_MoveNext()
        {
            return RunEnumeratorDeepMethodMoveNext();
        }

        private Component RunEnumerableRangeForeach()
        {
            Component c = null;
            foreach (int item in Enumerable.Range(0, _elementsCount))
            {
                c = _component;
            }
            return c;
        }

        private Component RunEnumerableReusableEnumeratorForeach()
        {
            _enumerableRangeEnumerator ??= Enumerable.Range(0, _elementsCount).GetEnumerator();
            _enumerableRangeEnumerator.Reset();
            Component c = null;
            int i = 0;
            while (_enumerableRangeEnumerator.MoveNext())
            {
                c = _component;
                i = _enumerableRangeEnumerator.Current;
            }
            return c;
        }

        private Component RunEnumerableMethodForeach()
        {
            Component c = null;
            foreach (int i in EnumerableMethod())
            {
                c = _component;
            }
            return c;
        }

        private Component RunEnumeratorMethodMoveNext()
        {
            Component c = null;
            var enumerator = EnumeratorMethod();
            var i = 0;
            while (enumerator.MoveNext())
            {
                i = enumerator.Current;
                c = _component;
            }
            return c;
        }

        private Component RunEnumerableStoredForeach()
        {
            Component c = null;
            foreach (int i in _storedEnumerable)
            {
                c = _component;
            }
            return c;
        }

        private Component RunEnumeratorStoredMoveNext()
        {
            Component c = null;
            _storedEnumerator.Reset();
            var i = 0;
            while (_storedEnumerator.MoveNext())
            {
                i = _storedEnumerator.Current;
                c = _component;
            }
            return c;
        }

        private Component RunEnumerableDeepMethodForeach()
        {
            Component c = null;
            foreach (int i in EnumerableDeepMethod())
            {
                c = _component;
            }
            return c;
        }

        private IEnumerable<int> EnumerableDeepMethod(int i = 0)
        {
            if (i >= _elementsCount) yield break;
            yield return i;
            foreach (int j in EnumerableDeepMethod(i + 1))
            {
                yield return j;
            }
        }

        private Component RunEnumeratorDeepMethodMoveNext()
        {
            Component c = null;
            var enumerator = EnumeratorDeepMethod();
            var i = 0;
            while (enumerator.MoveNext())
            {
                i = enumerator.Current;
                c = _component;
            }
            return c;
        }

        private IEnumerator<int> EnumeratorDeepMethod(int i = 0)
        {
            if (i >= _elementsCount) yield break;
            yield return i;
            var enumerator = EnumeratorDeepMethod(i + 1);
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }

        private IEnumerable<int> EnumerableMethod()
        {
            for (int i = 0; i < _elementsCount; i++)
            {
                yield return i;
            }
        }

        private IEnumerator<int> EnumeratorMethod()
        {
            for (int i = 0; i < _elementsCount; i++)
            {
                yield return i;
            }
        }
#endregion

        private Component Bench_GetComponentInChildren()
        {
            return GetComponentInChildren<Rigidbody>();
        }

        private Component Bench_GetComponent_Rigidbody2D()
        {
            return GetComponent<Rigidbody2D>();
        }

        private void Logic()
        {
            return;
        }


        private static void StaticLogic()
        {
            return;
        }

        private void RunLogic10times(Action logic)
        {
            logic(); // 1
            logic(); // 2
            logic(); // 3
            logic(); // 4
            logic(); // 5
            logic(); // 6
            logic(); // 7
            logic(); // 8
            logic(); // 9
            logic(); // 10
            
        }

        private void AppendDelegates(ref Action stored, Action logic, int times)
        {
            stored = logic;
            for (var i = 0; i < times - 1; i++)
            {
                stored += logic;
            }
        }

        private void AppendEvents(ref Action stored, Action logic, int times)
        {
            for (var i = 0; i < times; i++)
            {
                stored += logic;
            }
        }
    }
}