using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ClosureGarbage
{
    public struct Candy
    {
        public Color Color;
        public string Flavor;
        public float Price;
        public float Weight;
        public int Calories;
        public int Amount;
    }

    public class GarbageGeneralStress : MonoBehaviour
    {
        private static readonly string AnimatorPropName = "isOn";
        private static readonly int AnimatorPropHash = Animator.StringToHash("isOn");
        private static readonly string Tag = "Player";
        private const float FloatConst = 10f;
        private static readonly float FloatStaticReadonly = 10f;
        private static float FloatStatic = 10f;
        private float _floatInstance = 10f;
        private readonly float _floatInstanceReadonly = 10f;
        private float[] _floatArrayInstance = new float[]{ 10f, 5f, 2f, 1f, 0f };
        private static float[] _floatArrayStatic = new float[]{ 10f, 5f, 2f, 1f, 0f };
        private Dictionary<int, int> _dict = new() { { 1, 1 }, { 2, 2 }, { 3, 3 }, { 4, 4 }, { 5, 5 }, { 6, 6 } };
        private float _v;
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            for (var i = 0; i < 500; i++)
            {
                _v = Bench_Closure_None();

                _v = Bench_Closure_Static_Array();
                _v = Bench_Closure_Instance_Array();
                _v = Bench_Closure_Instance();
                _v = Bench_Closure_Instance_Readonly();
                _v = Bench_Closure_Static();
                _v = Bench_Closure_Static_Readonly();
                _v = Bench_Closure_Const();
                _v = Bench_DelegatesParam_FromStaticMethod();

                _v = Bench_Closure_Local();
                _v = Bench_Stackalloc_Array();
                _v = Bench_Create_Struct();
                _v = Bench_Create_Vector3();
                _v = Bench_Create_Quaternion();
                _v = Bench_Create_Array();
                _v = Bench_Create_String();
                _v = Bench_String_Concat();
                _v = Bench_String_Join_WithVarArgs();
                _v = Bench_String_Format();
                _v = Bench_String_Interpolation();
                _v = Bench_String_Comparison_EqualEqual();
                _v = Bench_String_Comparison_Equals();
                _v = Bench_Animator_Set_String();
                _v = Bench_Animator_Set_Hash();
                _v = Bench_Tag_CompareTag();
                _v = Bench_Tag_EqualEqual();
                _v = Bench_Float_ToString();
                _v = Bench_Int_Parse();
                _v = Bench_Create_Float();
                _v = Bench_Create_BoxedFloat();
                _v = (float) Bench_Create_Int();
                _v = Bench_Closure_LocallyCreated();
                _v = Bench_LINQ_Fluid();
                _v = Bench_LINQ_Fluid_Complex();
                _v = Bench_LINQ_Query();
                _v = Bench_Call_VarArgs();
                _v = Bench_Dict_Foreach_Values();
            }
        }

        private float Bench_String_Comparison_EqualEqual()
        {
            return "strstrstr" == "strstrstx" ? 1f : 0f;
        }

        private float Bench_String_Comparison_Equals()
        {
            return "strstrstr".Equals("strstrstx") ? 1f : 0f;
        }

        private float Bench_Animator_Set_String()
        {
            _animator.SetBool(AnimatorPropName, true);
            return 0f;
        }

        private float Bench_Animator_Set_Hash()
        {
            _animator.SetBool(AnimatorPropHash, true);
            return 0f;
        }

        private float Bench_Tag_CompareTag()
        {
            return this.CompareTag(Tag) ? 1f : 0f;
        }

        private float Bench_Tag_EqualEqual()
        {
            return this.tag == Tag ? 1f : 0f;
        }

        private float Bench_Dict_Foreach_Values()
        {
            float localFloat = 0f;
            foreach (int val in _dict.Values)
            {
                localFloat = (float) val;
            }
            return localFloat;
        }

        private float Bench_Int_Parse()
        {
            string localString = "9282123";
            int localInt = int.Parse(localString);;
            return (float) localInt;
        }

        private float Bench_Float_ToString()
        {
            float localFloat = 0.123456f;
            string localString = localFloat.ToString();
            return (float) localString[2];
        }

        private float Bench_String_Format()
        {
            float localFloat = 0.123456f;
            string localString = string.Format("0.00", localFloat);
            return (float) localString[2];
        }

        private float Bench_String_Interpolation()
        {
            float localFloat = 0.123456f;
            string localString = $"{localFloat:0.00} {localFloat} {localFloat} {localFloat} {localFloat} {localFloat} {localFloat}";
            return (float) localString[5];
        }

        private float Bench_String_Concat()
        {
            string localString = "";
            localString += "xpto";
            localString += "hey";
            localString += "ho";
            localString += "lets";
            localString += "go";
            return (float) localString[10];
        }

        private float Bench_Call_VarArgs()
        {
            return CallWithVarArgs(10f, 20f, 1f, 5f, 99f);
        }

        private float CallWithVarArgs(params float[] localFloats)
        {
            return localFloats[2];
        }

        private float Bench_String_Join_WithVarArgs()
        {
            string localString = string.Join("", "xpto", "hey", "ho", "lets", "go");
            return (float) localString[10];
        }

        private float Bench_Stackalloc_Array()
        {
            Span<float> localFloatArray = stackalloc float[] { 10f, 5f, 2f, 1f, 0f };
            return localFloatArray[2];
        }

        private float Bench_Create_Struct()
        {
            return new Candy {
                Color = Color.red,
                Flavor = "Strawberry",
                Price = 0.10f,
                Weight = 5f,
                Calories = 100,
                Amount = 1
            }.Price;
        }

        private float Bench_Create_Vector3()
        {
            return new Vector3(10f, 1f, 2f).x;
        }

        private float Bench_Create_Quaternion()
        {
            return new Quaternion(10f, 1f, 2f, 0f).x;
        }

        private float Bench_LINQ_Fluid()
        {
            return _floatArrayStatic.Where(x => x % 2 == 0).First();
        }
        private float Bench_LINQ_Fluid_Complex()
        {
            return _floatArrayStatic
                .Where(x => x % 2 == 0)
                .Select(x => (ulong) x)
                .Select(x => x |= 0xa1b2c3)
                .Append(300ul)
                .OrderBy(x => x & 0xff)
                .ThenByDescending(x => x >> 8)
                .First();
        }
        private float Bench_LINQ_Query()
        {
            return (
                from x in _floatArrayStatic
                where x % 2 == 0
                select x
            ).First();
        }

        private float Bench_Create_BoxedFloat()
        {
            object localFloat = 99f;
            return ((float)localFloat) + 27f;
        }

        private float Bench_Create_Float()
        {
            float localFloat = 99f;
            return localFloat + 27f;
        }

        private float Bench_Create_Int()
        {
            int localInt = 99;
            return localInt + 27;
        }

        private float Bench_Create_String()
        {
            string localString = "hey ho lets go";
            return (float) localString[3];
        }

        private float Bench_Create_Array()
        {
            float[] localFloatArray = new float[]{ 10f, 5f, 2f, 1f, 0f };
            return localFloatArray[3];
        }

        private float Bench_Closure_LocallyCreated()
        {
            float[] localFloatArray = new float[]{ 10f, 5f, 2f, 1f, 0f };
            return RunFunc(() => Logic(default, localFloatArray));
        }

        public float Bench_Closure_None()
        {
            return RunFunc(() => Logic(default, default));
        }

        public float Bench_Closure_Local()
        {
            float localFloat = 99f;
            return RunFunc(() => Logic(localFloat, default));
        }

        public float Bench_Closure_Const()
        {
            return RunFunc(() => Logic(FloatConst, default));
        }

        public float Bench_DelegatesParam_FromStaticMethod()
        {
            return RunFunc(SimpleLogicStatic);
        }

        private static float SimpleLogicStatic()
        {
            return Logic(default, default);
        }

        public float Bench_Closure_Static_Readonly()
        {
            return RunFunc(() => Logic(FloatStaticReadonly, default));
        }

        public float Bench_Closure_Static()
        {
            return RunFunc(() => Logic(FloatStatic, default));
        }

        public float Bench_Closure_Instance_Readonly()
        {
            return RunFunc(() => Logic(_floatInstanceReadonly, default));
        }

        public float Bench_Closure_Instance()
        {
            return RunFunc(() => Logic(_floatInstance, default));
        }

        public float Bench_Closure_Instance_Array()
        {
            return RunFunc(() => Logic(default, _floatArrayInstance));
        }

        public float Bench_Closure_Static_Array()
        {
            return RunFunc(() => Logic(default, _floatArrayStatic));
        }

        public static float Logic(float single, float[] array)
        {
            if (array == null) return single + 10f;
            return single + array[0];
        }

        private float RunFunc(Func<float> func)
        {
            return func.Invoke();
        }
    }
}