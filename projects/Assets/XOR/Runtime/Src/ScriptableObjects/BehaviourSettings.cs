using System;
using System.Collections.Generic;
using UnityEngine;
using XOR.Behaviour.Args;

namespace XOR
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class BehaviourSettings : ScriptableObjectBase<BehaviourSettings>
    {
        static BehaviourSettings() => ResourceAssetPath = "XOR/BehaviourSettings";

        [SerializeField]
        public List<Category> categories;

        protected override void OnCreate()
        {
            base.OnCreate();
            SetDefault();
        }

        public void SetDefault()
        {
            categories = new List<Category>();
        }
        public void SetPreference()
        {
            categories = new List<Category>()
            {
                Category.CreateDefaultSingle<BehaviourArg0>(),
                Category.CreateDefaultSingle<BehaviourBoolena>(),
                Category.CreateDefaultSingle<GizmosArg0>(),
                Category.CreateDefaultSingle<MouseArg0>(),
                Category.CreateDefaultSingle<EventSystemsPointerEventData>(),
                Category.CreateDefaultUnion<PhysicsCollider>(),
                Category.CreateDefaultUnion<PhysicsCollider2D>(),
                Category.CreateDefaultUnion<PhysicsCollision>(),
                Category.CreateDefaultUnion<PhysicsCollision2D>(),
            };
        }

        public enum GroupType
        {
            /// <summary>生成单独脚本, 每个脚本只拥有属于自己的方法 </summary>
            Single,
            /// <summary>生成单一脚本, 此脚本中包含所有选中的方法 </summary>
            Union,
            /// <summary>任意组合方法, 生成所有可能的组合脚本 </summary>
            Combine,
        }
        [System.Serializable]
        public class Group
        {
            public GroupType type;
            public uint value;
            /// <summary>
            /// 辅助参数, 对于GroupType.Combine类型生效, 指定m参数(0表示任意组合)
            /// </summary>
            public uint param;
        }
        [System.Serializable]
        public class Category
        {
            public string fullname;
            public List<Group> groups;

            private Type type;
            public Type Type
            {
                get
                {
                    if (type != null || string.IsNullOrEmpty(fullname))
                        return type;
                    type = Type.GetType(fullname, false);
                    return type;
                }
                set
                {
                    type = value;
                    fullname = value?.FullName;
                }
            }

            /// <summary>
            /// 获取组合数量
            /// </summary>
            /// <returns></returns>
            public int GetCombineCount(Group group)
            {
                var n = Helper.GetEnumCountByFlags(Type, group.value);
                if (n <= 0)
                    return 0;
                int count = 0;
                switch (group.type)
                {
                    case GroupType.Single:
                        count = n;
                        break;
                    case GroupType.Union:
                        count = 1;
                        break;
                    case GroupType.Combine:
                        if (group.param > 0)
                            count = Helper.GetCombineCountOfNM(n, group.param > n ? n : (int)group.param);
                        else
                            count = Helper.GetCombineCountOfAny(n);
                        break;
                }
                return count;
            }

            public static Category CreateDefaultSingle<TEnum>()
                where TEnum : Enum
            {
                return CreateDefault<TEnum>(GroupType.Single, Helper.GetEnumEverything<TEnum>());
            }
            public static Category CreateDefaultUnion<TEnum>()
                where TEnum : Enum
            {
                return CreateDefault<TEnum>(GroupType.Union, Helper.GetEnumEverything<TEnum>());
            }
            public static Category CreateDefault<TEnum>(GroupType type, uint value)
                where TEnum : Enum
            {
                return new Category()
                {
                    fullname = typeof(TEnum).FullName,
                    type = typeof(TEnum),
                    groups = new List<Group>()
                    {
                        new Group()
                        {
                            type = type,
                            value = value,
                        }
                    }
                };
            }
        }

        public class Helper
        {
            /// <summary>
            /// 获取n个元素中取m个元素的组合数量数量(m大于0且m小于等于n): 公式 n! / (m! * (n - m)!)
            /// </summary>
            /// <param name="n"></param>
            /// <param name="m"></param>
            /// <returns></returns> 
            public static int GetCombineCountOfNM(int n, int m)
            {
                if (m <= 0 || m > n)
                    return 0;
                return Factorial(n) / (Factorial(m) * Factorial(n - m));
            }
            /// <summary>
            /// 获取n个元素取任意个元素的组合数量(长度大于0且小于等于n): 公式 2^n - 1
            /// </summary>
            /// <param name="n"></param>
            /// <returns></returns>
            public static int GetCombineCountOfAny(int n)
            {
                if (n <= 0 || n >= 32)
                    return 0;
                return (2 << (n - 1)) - 1;
            }

            /// <summary>
            /// 获取value的阶乘值
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            static int Factorial(int value)
            {
                if (value <= 0)
                    return 1;
                return value * Factorial(value - 1);
            }

            public static uint GetEnumEverything<TEnum>()
                        where TEnum : Enum
            {
                return GetEnumEverything(typeof(TEnum));
            }
            public static uint GetEnumEverything(Type enumType)
            {
                uint value = 0;
                foreach (var define in Enum.GetValues(enumType))
                {
                    value |= Convert.ToUInt32(define);
                }
                return value;
            }

            public static int GetEnumCountByFlags(Type enumType, uint value)
            {
                if (value <= 0)
                    return 0;
                int count = 0;
                foreach (var define in Enum.GetValues(enumType))
                {
                    uint dv = Convert.ToUInt32(define);
                    if ((value & dv) == dv)
                    {
                        count++;
                    }
                }
                return count;
            }
        }
    }
}
