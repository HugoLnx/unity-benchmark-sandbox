using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GetComponentTest
{
    public class HugeComponent : MonoBehaviour
    {
        public int[] _array = new int[1000000];
    }

    public class GetComponentTest : MonoBehaviour
    {
        [SerializeField] private int _numberOfRuns = 5000;
        private Transform _transform;
        private Rigidbody _rigidbody;
        private Rigidbody2D _rigidbody2d;
        private HugeComponent _hugeComponent;
        private Dictionary<Type, Component> _childrenByType;
        private Dictionary<string, Component> _childrenByName;
        private Dictionary<int, Component> _childrenByHash;
        private Component _component;

        private void Awake()
        {
            _childrenByType = GetComponentsInChildren<Component>()
                .GroupBy(c => c.GetType())
                .ToDictionary(g => g.Key, g => g.First());
            _childrenByName = GetComponentsInChildren<Component>()
                .GroupBy(c => c.GetType().Name)
                .ToDictionary(g => g.Key, g => g.First());
            _childrenByHash = GetComponentsInChildren<Component>()
                .GroupBy(c => c.GetType().GetHashCode())
                .ToDictionary(g => g.Key, g => g.First());
        }

        public HugeComponent GetHugeComponentWithGenerics()
        {
            for (var i = 0; i < _numberOfRuns; i++)
            {
                _hugeComponent =  GetComponent<HugeComponent>();
            }
            return _hugeComponent;
        }
        private readonly string TransformName = nameof(Transform);
        private readonly Type TransformType = typeof(Transform);
        private readonly int TransformHash = typeof(Transform).GetHashCode();
        public Component GetTransformWithTypeDictionary()
        {
            for (var i = 0; i < _numberOfRuns; i++)
            {
                _component =  _childrenByType[TransformType];
            }
            return _component;
        }
        public Component GetTransformWithNameDictionary()
        {
            for (var i = 0; i < _numberOfRuns; i++)
            {
                _component =  _childrenByName[TransformName];
            }
            return _component;
        }
        public Component GetTransformWithHashDictionary()
        {
            for (var i = 0; i < _numberOfRuns; i++)
            {
                _component =  _childrenByHash[TransformHash];
            }
            return _component;
        }
        public Transform GetTransformWithGenerics()
        {
            for (var i = 0; i < _numberOfRuns; i++)
            {
                _transform =  GetComponent<Transform>();
            }
            return _transform;
        }
        public Transform GetTransformWithString()
        {
            for (var i = 0; i < _numberOfRuns; i++)
            {
                _transform = GetComponent("Transform") as Transform;
            }
            return _transform;
        }
        public Transform GetTransformWithTypeOf()
        {
            for (var i = 0; i < _numberOfRuns; i++)
            {
                _transform = GetComponent(typeof(Transform)) as Transform;
            }
            return _transform;
        }
        public Transform GetTransformWithNameOf()
        {
            for (var i = 0; i < _numberOfRuns; i++)
            {
                _transform = GetComponent(nameof(Transform)) as Transform;
            }
            return _transform;
        }
        public Rigidbody GetRigidbodyWithGenerics()
        {
            for (var i = 0; i < _numberOfRuns; i++)
            {
                _rigidbody =  GetComponent<Rigidbody>();
            }
            return _rigidbody;
        }
        public Rigidbody GetRigidbodyWithString()
        {
            for (var i = 0; i < _numberOfRuns; i++)
            {
                _rigidbody = GetComponent("Rigidbody") as Rigidbody;
            }
            return _rigidbody;
        }
        public Rigidbody GetRigidbodyWithTypeOf()
        {
            for (var i = 0; i < _numberOfRuns; i++)
            {
                _rigidbody = GetComponent(typeof(Rigidbody)) as Rigidbody;
            }
            return _rigidbody;
        }
        public Rigidbody GetRigidbodyWithNameOf()
        {
            for (var i = 0; i < _numberOfRuns; i++)
            {
                _rigidbody = GetComponent(nameof(Rigidbody)) as Rigidbody;
            }
            return _rigidbody;
        }
        public Transform GetTransformInChildrenWithGenerics()
        {
            for (var i = 0; i < _numberOfRuns; i++)
            {
                _transform =  GetComponentInChildren<Transform>();
            }
            return _transform;
        }
        public Transform GetTransformInChildrenWithTypeOf()
        {
            for (var i = 0; i < _numberOfRuns; i++)
            {
                _transform = GetComponentInChildren(typeof(Transform)) as Transform;
            }
            return _transform;
        }
        public Rigidbody GetRigidbodyInChildrenWithGenerics()
        {
            for (var i = 0; i < _numberOfRuns; i++)
            {
                _rigidbody =  GetComponentInChildren<Rigidbody>();
            }
            return _rigidbody;
        }
        public Rigidbody GetRigidbodyInChildrenWithTypeOf()
        {
            for (var i = 0; i < _numberOfRuns; i++)
            {
                _rigidbody = GetComponentInChildren(typeof(Rigidbody)) as Rigidbody;
            }
            return _rigidbody;
        }
        public Rigidbody2D GetRigidbody2DInChildrenWithGenerics()
        {
            for (var i = 0; i < _numberOfRuns; i++)
            {
                _rigidbody2d =  GetComponentInChildren<Rigidbody2D>();
            }
            return _rigidbody2d;
        }
        public Rigidbody2D GetRigidbody2DInChildrenWithTypeOf()
        {
            for (var i = 0; i < _numberOfRuns; i++)
            {
                _rigidbody2d = GetComponentInChildren(typeof(Rigidbody2D)) as Rigidbody2D;
            }
            return _rigidbody2d;
        }

        private string GenerateGarbage()
        {
            string str = "";
            for (var i = 0; i < _numberOfRuns; i++)
            {
                str += UnityEngine.Random.Range(0, 255).ToString();
            }
            return str;
        }

        private int GetTransformHashCode()
        {
            int hashCode = 0;
            for (var i = 0; i < _numberOfRuns; i++)
            {
                hashCode = typeof(Transform).GetHashCode();
            }
            return hashCode;
        }

        private static readonly int constantInt = 14123123;

        private int GetIntHashCode()
        {
            int hashCode = 0;
            for (var i = 0; i < _numberOfRuns; i++)
            {
                hashCode = constantInt.GetHashCode();
            }
            return hashCode;
        }

        private static readonly string smallString = "small string";
        private static readonly string hugeString = @"Lorem ipsum dolor sit amet,
        consectetur adipiscing elit. Donec a diam lectus. Sed sit amet ipsum mauris.
        Maecenas congue ligula ac quam viverra nec consectetur ante hendrerit. Donec et mollis dolor.
        consectetur adipiscing elit. Donec a diam lectus. Sed sit amet ipsum mauris.
        consectetur adipiscing elit. Donec a diam lectus. Sed sit amet ipsum mauris.
        consectetur adipiscing elit. Donec a diam lectus. Sed sit amet ipsum mauris.
        consectetur adipiscing elit. Donec a diam lectus. Sed sit amet ipsum mauris.
        Maecenas congue ligula ac quam viverra nec consectetur ante hendrerit. Donec et mollis dolor.
        consectetur adipiscing elit. Donec a diam lectus. Sed sit amet ipsum mauris.
        Maecenas congue ligula ac quam viverra nec consectetur ante hendrerit. Donec et mollis dolor.
        consectetur adipiscing elit. Donec a diam lectus. Sed sit amet ipsum mauris.
        consectetur adipiscing elit. Donec a diam lectus. Sed sit amet ipsum mauris.
        consectetur adipiscing elit. Donec a diam lectus. Sed sit amet ipsum mauris.
        Maecenas congue ligula ac quam viverra nec consectetur ante hendrerit. Donec et mollis dolor.
        consectetur adipiscing elit. Donec a diam lectus. Sed sit amet ipsum mauris.
        consectetur adipiscing elit. Donec a diam lectus. Sed sit amet ipsum mauris.
        Maecenas congue ligula ac quam viverra nec consectetur ante hendrerit. Donec et mollis dolor.
        consectetur adipiscing elit. Donec a diam lectus. Sed sit amet ipsum mauris.
        consectetur adipiscing elit. Donec a diam lectus. Sed sit amet ipsum mauris.
        Maecenas congue ligula ac quam viverra nec consectetur ante hendrerit. Donec et mollis dolor.
        Maecenas congue ligula ac quam viverra nec consectetur ante hendrerit. Donec et mollis dolor.
        consectetur adipiscing elit. Donec a diam lectus. Sed sit amet ipsum mauris.
        Maecenas congue ligula ac quam viverra nec consectetur ante hendrerit. Donec et mollis dolor.
        consectetur adipiscing elit. Donec a diam lectus. Sed sit amet ipsum mauris.
        Maecenas congue ligula ac quam viverra nec consectetur ante hendrerit. Donec et mollis dolor.
        consectetur adipiscing elit. Donec a diam lectus. Sed sit amet ipsum mauris.
        Maecenas congue ligula ac quam viverra nec consectetur ante hendrerit. Donec et mollis dolor.
        consectetur adipiscing elit. Donec a diam lectus. Sed sit amet ipsum mauris.
        Maecenas congue ligula ac quam viverra nec consectetur ante hendrerit. Donec et mollis dolor.
        consectetur adipiscing elit. Donec a diam lectus. Sed sit amet ipsum mauris.
        Maecenas congue ligula ac quam viverra nec consectetur ante hendrerit. Donec et mollis dolor.
        Maecenas congue ligula ac quam viverra nec consectetur ante hendrerit. Donec et mollis dolor.
        Maecenas congue ligula ac quam viverra nec consectetur ante hendrerit. Donec et mollis dolor.
        Maecenas congue ligula ac quam viverra nec consectetur ante hendrerit. Donec et mollis dolor.
        consectetur adipiscing elit. Donec a diam lectus. Sed sit amet ipsum mauris.
        Maecenas congue ligula ac quam viverra nec consectetur ante hendrerit. Donec et mollis dolor.
        Maecenas congue ligula ac quam viverra nec consectetur ante hendrerit. Donec et mollis dolor.
        Maecenas congue ligula ac quam viverra nec consectetur ante hendrerit. Donec et mollis dolor.
        consectetur adipiscing elit. Donec a diam lectus. Sed sit amet ipsum mauris.
        Maecenas congue ligula ac quam viverra nec consectetur ante hendrerit. Donec et mollis dolor.
        Maecenas congue ligula ac quam viverra nec consectetur ante hendrerit. Donec et mollis dolor.
        consectetur adipiscing elit. Donec a diam lectus. Sed sit amet ipsum mauris.
        Maecenas congue ligula ac quam viverra nec consectetur ante hendrerit. Donec et mollis dolor.
        Maecenas congue ligula ac quam viverra nec consectetur ante hendrerit. Donec et mollis dolor.
        Maecenas congue ligula ac quam viverra nec consectetur ante hendrerit. Donec et mollis dolor.
        Praesent et diam eget libero egestas mattis sit amet vitae augue.";
        private int GetSmallStringHashCode()
        {
            int hashCode = 0;
            for (var i = 0; i < _numberOfRuns; i++)
            {
                hashCode = "small string".GetHashCode();
            }
            return hashCode;
        }

        private int GetHugeStringHashCode()
        {
            int hashCode = 0;
            for (var i = 0; i < _numberOfRuns; i++)
            {
                hashCode = "small string".GetHashCode();
            }
            return hashCode;
        }

        private void Update()
        {
            if (!Input.GetKey(KeyCode.Space)) return;
            GetTransformWithGenerics();
            GetTransformWithString();
            GetTransformWithTypeOf();
            GetTransformWithNameOf();
            GetTransformWithTypeDictionary();
            GetTransformWithNameDictionary();
            GetTransformWithHashDictionary();
            GetTransformHashCode();
            GetIntHashCode();
            GetSmallStringHashCode();
            GetHugeStringHashCode();
            // GetRigidbodyWithGenerics();
            // GetRigidbodyWithString();
            // GetRigidbodyWithTypeOf();
            // GetRigidbodyWithNameOf();
            // GetTransformInChildrenWithGenerics();
            // GetTransformInChildrenWithTypeOf();
            // GetRigidbodyInChildrenWithGenerics();
            // GetRigidbodyInChildrenWithTypeOf();
            // GetRigidbody2DInChildrenWithGenerics();
            // GetRigidbody2DInChildrenWithTypeOf();
            // GetHugeComponentWithGenerics();
            // GenerateGarbage();
        }
    }
}