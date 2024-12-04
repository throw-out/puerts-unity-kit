using System;
using System.Collections.Generic;
using System.Linq;
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
                Category.CreateDefaultEverything<Behaviour.Args.Behaviour>(GroupType.Single),
                Category.CreateDefaultEverything<Behaviour.Args.BehaviourBoolean>(GroupType.Single),
                Category.CreateDefaultEverything<Behaviour.Args.Gizmos>(GroupType.Single),
                Category.CreateDefaultEverything<Behaviour.Args.Mouse>(GroupType.Single, GroupType.Union),
                Category.CreateDefaultEverything<Behaviour.Args.EventSystemsPointerEventData>(GroupType.Single),
                Category.CreateDefaultEverything<Behaviour.Args.PhysicsCollider>(GroupType.Single, GroupType.Union),
                Category.CreateDefaultEverything<Behaviour.Args.PhysicsCollider2D>(GroupType.Single, GroupType.Union),
                Category.CreateDefaultEverything<Behaviour.Args.PhysicsCollision>(GroupType.Single, GroupType.Union),
                Category.CreateDefaultEverything<Behaviour.Args.PhysicsCollision2D>(GroupType.Single, GroupType.Union),
            };
        }

        public HashSet<Enum> GenerateTypes()
        {
            if (categories == null || categories.Count <= 0)
                return null;
            HashSet<Enum> enums = new HashSet<Enum>();

            void GenerateCombinations(uint[] array, int index, List<uint> current, List<List<uint>> results)
            {
                results.Add(new List<uint>(current));

                for (int i = index; i < array.Length; i++)
                {
                    current.Add(array[i]);
                    GenerateCombinations(array, i + 1, current, results);
                    current.RemoveAt(current.Count - 1);
                }
            }

            categories.ForEach(category =>
            {
                if (category.Type == null)
                    return;
                var defines = Helper.GetEnumValues(category.Type);
                category.groups?.ForEach(group =>
                {
                    switch (group.type)
                    {
                        case GroupType.Single:
                            for (int i = 0; i < defines.Length; i++)
                            {
                                uint dv = defines[i];
                                if ((group.value & dv) == dv)
                                {
                                    enums.Add((Enum)Enum.ToObject(category.Type, dv));
                                }
                            }
                            break;
                        case GroupType.Union:
                            uint value1 = 0;
                            for (int i = 0; i < defines.Length; i++)
                            {
                                uint dv = defines[i];
                                if ((group.value & dv) == dv)
                                {
                                    value1 |= dv;
                                }
                            }
                            if (value1 > 0)
                            {
                                enums.Add((Enum)Enum.ToObject(category.Type, value1));
                            }
                            break;
                        case GroupType.Combine:
                            uint[] array = defines
                                .Where(dv => (group.value & dv) == dv)
                                .ToArray();
                            uint n = (uint)array.Length;
                            uint m = group.param > n ? n : group.param;

                            List<List<uint>> results = new List<List<uint>>();
                            List<uint> current = new List<uint>();
                            GenerateCombinations(array, 0, current, results);
                            foreach (var result in results)
                            {
                                if (m > 0 && result.Count != m)
                                    continue;
                                uint value2 = 0;
                                for (int i = 0; i < result.Count; i++)
                                {
                                    value2 |= result[i];
                                }
                                if (value2 > 0)
                                {
                                    enums.Add((Enum)Enum.ToObject(category.Type, value2));
                                }
                            }
                            break;
                    }
                });
            });
            return enums;
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

            public static Category CreateDefaultEverything<TEnum>(GroupType frist, params GroupType[] types)
                where TEnum : Enum
            {
                var value = Helper.GetEnumEverything<TEnum>();
                var category = CreateDefault<TEnum>(frist, value);
                category.groups.AddRange(types.Select(t => new Group()
                {
                    type = t,
                    value = value
                }));
                return category;
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
            public static uint[] GetEnumValues(Type enumType)
            {
                List<uint> values = new List<uint>();
                foreach (var define in Enum.GetValues(enumType))
                {
                    uint dv = Convert.ToUInt32(define);
                    if (dv > 0)
                    {
                        values.Add(dv);
                    }
                }
                return values.ToArray();
            }
        }
    }
}
