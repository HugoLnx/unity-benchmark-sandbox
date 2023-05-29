using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StressBenchmark
{
    public class StressDelegatesFull : MonoBehaviour
    {
        // TODO:
        // 1. test event calls
        // 2. test event multicast calls
        // 3. test multicast delegates calls
        // 4. test attribution
        [SerializeField] private int _iterations = 5000;
        private Component _component;
        private GameObject _obj;
        private Func<GameObject, Component> _storedFunc;
        private Func<GameObject, Component> _storedStaticFunc;
        private static Func<GameObject, Component> _staticStoredFunc;
        private static Func<GameObject, Component> _staticStoredStaticFunc;

        private Func<GameObject, Component> _storedFunc2x;
        private Func<GameObject, Component> _storedFunc5x;
        private Func<GameObject, Component> _storedFunc10x;
        // TODO Test static stored funcs and events
        private event Func<GameObject, Component> _storedEvent;
        private event Func<GameObject, Component> _storedEvent2x;
        private event Func<GameObject, Component> _storedEvent5x;
        private event Func<GameObject, Component> _storedEvent10x;

        private void Awake()
        {
            _obj = this.gameObject;
            _storedFunc = Logic;
            _storedStaticFunc = StaticLogic;
            _staticStoredFunc = Logic;
            _staticStoredStaticFunc = StaticLogic;
        }

        public void Update()
        {
            if (!Input.GetKey(KeyCode.Space)) return;

            Component c;
            c = BenchStaticDirectCall();
            c = Bench_Static_Call_StaticStoredFunc();
            c = Bench_Static_Call_StoredFunc();
            c = Bench_Static_Call_AnonymousDelegates();
            c = Bench_Static_Call_AnonymousArrow();
            c = Bench_Static_Call_MethodParameter();
            c = Bench_Instance_Call_Direct();
            c = Bench_Instance_Call_StaticStoredFunc();
            c = Bench_Instance_Call_StoredFunc();
            c = Bench_Instance_Call_AnonymousDelegates();
            c = Bench_Instance_Call_AnonymousArrow();
            c = Bench_Instance_Call_MethodParameter();
            c = Bench_GetComponentInChildren();
            c = Bench_GetComponent();
        }

        private Component Bench_Instance_Call_MethodParameter()
        {
            for (int i = 0; i < _iterations; i++)
            {
                _component = RunLogic(Logic);
            }
            return _component;
        }

        private Component Bench_Instance_Call_AnonymousArrow()
        {
            for (int i = 0; i < _iterations; i++)
            {
                _component = RunLogic((obj) => Logic(obj));
            }
            return _component;
        }

        private Component Bench_Instance_Call_AnonymousDelegates()
        {
            for (int i = 0; i < _iterations; i++)
            {
                _component = RunLogic(delegate(GameObject obj){ return Logic(obj);});
            }
            return _component;
        }

        private Component Bench_Instance_Call_StoredFunc()
        {
            for (int i = 0; i < _iterations; i++)
            {
                _component = RunLogic(_storedFunc);
            }
            return _component;
        }

        private Component Bench_Instance_Call_StaticStoredFunc()
        {
            for (int i = 0; i < _iterations; i++)
            {
                _component = RunLogic(_staticStoredFunc);
            }
            return _component;
        }

        private Component Bench_Instance_Call_Direct()
        {
            for (int i = 0; i < _iterations; i++)
            {
                _component = Logic(_obj);
            }
            return _component;
        }

#region Static
        private Component Bench_Static_Call_MethodParameter()
        {
            for (int i = 0; i < _iterations; i++)
            {
                _component = RunLogic(StaticLogic);
            }
            return _component;
        }

        private Component Bench_Static_Call_AnonymousArrow()
        {
            for (int i = 0; i < _iterations; i++)
            {
                _component = RunLogic((obj) => StaticLogic(obj));
            }
            return _component;
        }

        private Component Bench_Static_Call_AnonymousDelegates()
        {
            for (int i = 0; i < _iterations; i++)
            {
                _component = RunLogic(delegate(GameObject obj){ return StaticLogic(obj);});
            }
            return _component;
        }

        private Component Bench_Static_Call_StoredFunc()
        {
            for (int i = 0; i < _iterations; i++)
            {
                _component = RunLogic(_storedStaticFunc);
            }
            return _component;
        }

        private Component Bench_Static_Call_StaticStoredFunc()
        {
            for (int i = 0; i < _iterations; i++)
            {
                _component = RunLogic(_staticStoredStaticFunc);
            }
            return _component;
        }

        private Component BenchStaticDirectCall()
        {
            for (int i = 0; i < _iterations; i++)
            {
                _component = StaticLogic(_obj);
            }
            return _component;
        }
#endregion        

#region StoredDelegates
        private Component Bench_StoredDelegates_Instance_Call_StoredFunc()
        {
            for (int i = 0; i < _iterations; i++)
            {
                _component = _storedFunc.Invoke(_obj);
            }
            return _component;
        }

        private Component Bench_StoredDelegates_Instance_Call_StoredFunc2x()
        {
            for (int i = 0; i < _iterations/2; i++)
            {
                _component = _storedFunc2x.Invoke(_obj);
            }
            return _component;
        }

        private Component Bench_StoredDelegates_Instance_Call_StoredFunc5x()
        {
            for (int i = 0; i < _iterations/5; i++)
            {
                _component = _storedFunc5x.Invoke(_obj);
            }
            return _component;
        }

        private Component Bench_StoredDelegates_Instance_Call_StoredFunc10x()
        {
            for (int i = 0; i < _iterations/10; i++)
            {
                _component = _storedFunc10x.Invoke(_obj);
            }
            return _component;
        }
#endregion


#region StoredEvents
        private Component Bench_StoredEvents_Instance_Call_StoredEvent()
        {
            for (int i = 0; i < _iterations; i++)
            {
                _component = _storedEvent.Invoke(_obj);
            }
            return _component;
        }

        private Component Bench_StoredEvents_Instance_Call_StoredEvent2x()
        {
            for (int i = 0; i < _iterations/2; i++)
            {
                _component = _storedEvent2x.Invoke(_obj);
            }
            return _component;
        }

        private Component Bench_StoredEvents_Instance_Call_StoredEvent5x()
        {
            for (int i = 0; i < _iterations/5; i++)
            {
                _component = _storedEvent5x.Invoke(_obj);
            }
            return _component;
        }

        private Component Bench_StoredEvents_Instance_Call_StoredEvent10x()
        {
            for (int i = 0; i < _iterations/10; i++)
            {
                _component = _storedEvent10x.Invoke(_obj);
            }
            return _component;
        }
#endregion

        private Component Bench_GetComponentInChildren()
        {
            for (int i = 0; i < _iterations; i++)
            {
                _component = GetComponentInChildren<Rigidbody>();
            }
            return _component;
        }

        private Component Bench_GetComponent()
        {
            for (int i = 0; i < _iterations; i++)
            {
                _component = GetComponent<Rigidbody2D>();
            }
            return _component;
        }

        private Component Logic(GameObject obj)
        {
            return null;
            //return obj.GetComponent<RigidBody>();
        }


        private static Component StaticLogic(GameObject obj)
        {
            return null;
            //return obj.GetComponent<RigidBody>();
        }

        private Component RunLogic(Func<GameObject, Component> logic)
        {
            return logic(_obj);
        }
    }
}