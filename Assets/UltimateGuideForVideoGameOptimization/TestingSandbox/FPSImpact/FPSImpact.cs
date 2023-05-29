using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StressBenchmark;
using TMPro;
using UnityEngine;

namespace FPSImpactTesting
{
    public class FPSImpact : MonoBehaviour
    {
        public enum Operation
        {
            None,
            GetComponent,
            GetComponentInChildren,
            SmallDictionaryIntGet,
            SmallDictionaryShortStrGet,
            SmallDictionaryLongStrGet,
            BigDictionaryIntGet,
            BigDictionaryShortStrGet,
            BigDictionaryLongStrGet,
            BigDictionaryShort2StrGet,
            BigDictionaryLong2StrGet,
            List50Foreach,
            Array50Foreach,
            Array5000Get,
            List5000Get,
            List50Get,
        }
        [SerializeField] private int targetFps = -1;
        [SerializeField] private int _iterations = 5000;
        [SerializeField] private int _dictBigSize = 1000;
        [SerializeField] private int _dictSmallSize = 25;
        [SerializeField] private int _longKeySize = 100;
        [SerializeField] private int _shortKeySize = 20;
        [SerializeField] private int _long2KeySize = 100;
        [SerializeField] private int _short2KeySize = 20;
        [SerializeField] private int _collectionBigSize = 5000;
        [SerializeField] private int _collectionSmallSize = 50;
        [SerializeField] private Operation _operation = Operation.None;
        [SerializeField] private TMP_InputField _iterationsInput;
        [SerializeField] private TMP_Text _opText;
        private Dictionary<int, Component> _bigDictInt = new();
        private Dictionary<int, Component> _smallDictInt = new();
        private Dictionary<string, Component> _bigDictShortStr = new();
        private Dictionary<string, Component> _bigDictLongStr = new();
        private List<Component> _list5000 = new();
        private List<Component> _list50 = new();
        private Component[] _array5000;
        private Component[] _array50;
        private string[] _bigDictLongKeys;
        private string[] _bigDictShort2Keys;
        private string[] _bigDictLong2Keys;
        private string[] _bigDictShortKeys;
        private int[] _bigDictIntKeys;
        private int[] _smallDictIntKeys;
        private string[] _smallDictShortKeys;
        private string[] _smallDictLongKeys;
        private int[] _collectionBigIntKeys;
        private int[] _collectionSmallIntKeys;
        private int _loopIndex;
        private string _loopBigDictLongKey;
        private string _loopBigDictShortKey;
        private int _loopBigDictIntKey;
        private int _loopSmallDictIntKey;
        private int _loopCollectionBigIntKey;
        private int _loopCollectionSmallIntKey;
        private Component _component;
        private static Component[] _components;
        private int _componentsInx = 0;
        private Dictionary<string, Component> _smallDictShortStr = new();
        private Dictionary<string, Component> _smallDictLongStr = new();
        private Dictionary<string, Component> _bigDictShort2Str = new();
        private Dictionary<string, Component> _bigDictLong2Str = new();
        private string _loopSmallDictShortKey;
        private string _loopSmallDictLongKey;
        private string _loopBigDictShort2Key;
        private string _loopBigDictLong2Key;

        private Component[] Components => _components ??= FindObjectsByType<Component>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OrderBy(_ => UnityEngine.Random.value)
            .ToArray();

        private Component NextComponent => Components[_componentsInx++ % Components.Length];

        private void PrepareForIterationRun()
        {
            _loopSmallDictIntKey = _smallDictIntKeys[_loopIndex % _smallDictIntKeys.Length];
            _loopSmallDictShortKey = _smallDictShortKeys[_loopIndex % _smallDictShortKeys.Length];
            _loopSmallDictLongKey = _smallDictLongKeys[_loopIndex % _smallDictLongKeys.Length];
            _loopBigDictShortKey = _bigDictShortKeys[_loopIndex % _bigDictShortKeys.Length];
            _loopBigDictLongKey = _bigDictLongKeys[_loopIndex % _bigDictLongKeys.Length];
            _loopBigDictShort2Key = _bigDictShort2Keys[_loopIndex % _bigDictShort2Keys.Length];
            _loopBigDictLong2Key = _bigDictLong2Keys[_loopIndex % _bigDictLong2Keys.Length];
            _loopBigDictIntKey = _bigDictIntKeys[_loopIndex % _bigDictIntKeys.Length];

            _loopCollectionBigIntKey = _collectionBigIntKeys[_loopIndex % _collectionBigIntKeys.Length];
            _loopCollectionSmallIntKey = _collectionSmallIntKeys[_loopIndex % _collectionSmallIntKeys.Length];

            _loopIndex = (_loopIndex + 1) % 100000;
        }

        private void Awake()
        {
            _array5000 = new Component[_collectionBigSize];
            _array50 = new Component[_collectionSmallSize];
            _bigDictIntKeys = GenerateDictKeys(amount: 100, domainSize: _dictBigSize, keyFunc: i => i);
            _bigDictShortKeys = GenerateDictKeys(amount: 100, domainSize: _dictBigSize, keyFunc: BuildShortKey);
            _bigDictLongKeys = GenerateDictKeys(amount: 100, domainSize: _dictBigSize, keyFunc: BuildLongKey);
            _bigDictShort2Keys = GenerateDictKeys(amount: 100, domainSize: _dictBigSize, keyFunc: BuildShort2Key);
            _bigDictLong2Keys = GenerateDictKeys(amount: 100, domainSize: _dictBigSize, keyFunc: BuildLong2Key);
            _smallDictIntKeys = GenerateDictKeys(amount: 100, domainSize: _dictSmallSize, keyFunc: i => i);
            _smallDictShortKeys = GenerateDictKeys(amount: 100, domainSize: _dictSmallSize, keyFunc: BuildShortKey);
            _smallDictLongKeys = GenerateDictKeys(amount: 100, domainSize: _dictSmallSize, keyFunc: BuildLongKey);
            _collectionBigIntKeys = GenerateDictKeys(amount: 100, domainSize: _collectionBigSize, keyFunc: i => i);
            _collectionSmallIntKeys = GenerateDictKeys(amount: 100, domainSize: _collectionSmallSize, keyFunc: i => i);
            for (int i = 0; i < _dictBigSize; i++)
            {
                Component c = NextComponent;
                _bigDictInt.Add(i, c);
                _bigDictLongStr.Add(BuildLongKey(i), c);
                _bigDictShortStr.Add(BuildShortKey(i), c);
                _bigDictShort2Str.Add(BuildShort2Key(i), c);
                _bigDictLong2Str.Add(BuildLong2Key(i), c);
            }
            for (int i = 0; i < _dictSmallSize; i++)
            {
                Component c = NextComponent;
                _smallDictInt.Add(i, c);
                _smallDictShortStr.Add(BuildShortKey(i), c);
                _smallDictLongStr.Add(BuildLongKey(i), c);
            }
            for (int i = 0; i < _collectionBigSize; i++)
            {
                Component c = NextComponent;
                _array5000[i] = c;
                _list5000.Add(c);
            }
            for (int i = 0; i < _collectionSmallSize; i++)
            {
                Component c = NextComponent;
                _list50.Add(c);
                _array50[i] = c;
            }
        }

        private void Start()
        {
            RefreshOperationUI();
            _iterationsInput.text = _iterations.ToString();
        }
        public void Update()
        {
            Application.targetFrameRate = targetFps;
            for (int i = 0; i < _iterations; i++)
            {
                PrepareForIterationRun();
                // Component component = _operation switch
                // {
                //     Operation.None => null,
                //     Operation.GetComponent => GetComponent<Rigidbody>(),
                //     Operation.GetComponentInChildren => GetComponentInChildren<Rigidbody2D>(),
                //     Operation.BigDictionaryIntGet => _dictBig[_loopDictBigIntKey],
                //     Operation.SmallDictionaryIntGet => _dictSmall[_loopDictSmallIntKey],
                //     Operation.BigDictionaryLongStrGet => _dictLongStr[_loopDictLongKey],
                //     Operation.BigDictionaryShortStrGet => _dictShortStr[_loopDictShortKey],
                //     Operation.Array5000Get => _array5000[_loopCollectionBigIntKey],
                //     Operation.List5000Get => _list5000[_loopCollectionBigIntKey],
                //     Operation.List50Get => _list50[_loopCollectionSmallIntKey],
                //     _ => null,
                // };
                Component component = null;
                switch (_operation) {
                case Operation.None:
                    component = null;
                    break;
                case Operation.GetComponent:
                    component = GetComponent<MeshRenderer>();
                    break;
                case Operation.GetComponentInChildren:
                    component = GetComponentInChildren<Rigidbody>();
                    break;
                case Operation.SmallDictionaryIntGet:
                    component = _smallDictInt[_loopSmallDictIntKey];
                    break;
                case Operation.SmallDictionaryShortStrGet:
                    component = _smallDictShortStr[_loopSmallDictShortKey];
                    break;
                case Operation.SmallDictionaryLongStrGet:
                    component = _smallDictLongStr[_loopSmallDictLongKey];
                    break;
                case Operation.BigDictionaryIntGet:
                    component = _bigDictInt[_loopBigDictIntKey];
                    break;
                case Operation.BigDictionaryLongStrGet:
                    component = _bigDictLongStr[_loopBigDictLongKey];
                    break;
                case Operation.BigDictionaryShortStrGet:
                    component = _bigDictShortStr[_loopBigDictShortKey];
                    break;
                case Operation.BigDictionaryShort2StrGet:
                    component = _bigDictShort2Str[_loopBigDictShort2Key];
                    break;
                case Operation.BigDictionaryLong2StrGet:
                    component = _bigDictLong2Str[_loopBigDictLong2Key];
                    break;
                case Operation.Array5000Get:
                    component = _array5000[_loopCollectionBigIntKey];
                    break;
                case Operation.List5000Get:
                    component = _list5000[_loopCollectionBigIntKey];
                    break;
                case Operation.List50Get:
                    component = _list50[_loopCollectionSmallIntKey];
                    break;
                case Operation.List50Foreach:
                    foreach (var item in _list50)
                    {
                        component = item;
                        _component = component;
                    }
                    break;
                case Operation.Array50Foreach:
                    foreach (var item in _array50)
                    {
                        component = item;
                        _component = component;
                    }
                    break;
                }
                _component = component;
            }
        }
        private void OnDisable()
        {
            Debug.Log(_component);
        }

        public void UpdateIterationsFromUI()
        {
            int.TryParse(_iterationsInput.text, out _iterations);
        }

        public void NextOperation()
        {
            _operation = (Operation)(((int)_operation + 1) % Enum.GetValues(typeof(Operation)).Length);
            RefreshOperationUI();
        }

        private void RefreshOperationUI()
        {
            _opText.text = Enum.GetName(typeof(Operation), _operation);
        }

        private string BuildLongKey(int i) => StringGenerator.Build(size: _longKeySize, suffix: $"-{i}", seed: 127 * (i + 1));
        private string BuildShortKey(int i) => StringGenerator.Build(size: _shortKeySize, suffix: $"-{i}", seed: 379 * (i + 1));
        private string BuildLong2Key(int i) => StringGenerator.Build(size: _long2KeySize, suffix: $"-{i}", seed: 127 * (i + 1));
        private string BuildShort2Key(int i) => StringGenerator.Build(size: _short2KeySize, suffix: $"-{i}", seed: 379 * (i + 1));
        private T[] GenerateDictKeys<T>(int amount, int domainSize, Func<int, T> keyFunc)
        {
            return Enumerable.Range(1, amount)
                .Select(_ => UnityEngine.Random.Range(0, domainSize))
                .Select(keyFunc)
                .OrderBy((_) => UnityEngine.Random.value)
                .ToArray();
        }
    }
}