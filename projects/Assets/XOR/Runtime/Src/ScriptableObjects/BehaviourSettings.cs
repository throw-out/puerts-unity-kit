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
            categories = new List<Category>()
            {
                Category.CreateDefaultSingle<BehaviourArg0>(),
                Category.CreateDefaultSingle<BehaviourBoolena>(),
                Category.CreateDefaultSingle<GizmosArg0>(),
                Category.CreateDefaultSingle<MouseArg0>(),
                Category.CreateDefaultSingle<EventSystemsPointerEventData>(),
                Category.CreateDefaultSingle<PhysicsCollider>(),
                Category.CreateDefaultSingle<PhysicsCollider2D>(),
                Category.CreateDefaultSingle<PhysicsCollision>(),
                Category.CreateDefaultSingle<PhysicsCollision2D>(),
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
            /// <summary>辅助参数, 对于GroupType.Combine类型需要以多少个方法组合生成脚本</summary>
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

            public static Category CreateDefaultSingle<TEnum>()
                where TEnum : Enum
            {
                return CreateDefault<TEnum>(GroupType.Single, GetEnumValue<TEnum>());
            }
            public static Category CreateDefaultUnion<TEnum>()
                where TEnum : Enum
            {
                return CreateDefault<TEnum>(GroupType.Union, GetEnumValue<TEnum>());
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
            public static uint GetEnumValue<TEnum>()
               where TEnum : Enum
            {
                return GetEnumValue(typeof(TEnum));
            }
            public static uint GetEnumValue(Type enumType)
            {
                uint value = 0;
                foreach (var define in Enum.GetValues(enumType))
                {
                    if (define is int i1)
                    {
                        value |= (uint)i1;
                    }
                    else if (define is uint i2)
                    {
                        value |= i2;
                    }
                }
                return value;
            }
        }
    }
}
