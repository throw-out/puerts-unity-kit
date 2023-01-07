using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

/// <summary>
/// 委托工具类
/// 根据MemberInfo创建Delegate实例
/// </summary>
public static class DelegateUtil
{
    public static TDelegate CreateMethodDelegate<TDelegate>(MethodInfo memberInfo, object firstArgument = null, bool throwOnBindFailure = true)
        where TDelegate : Delegate
    {
        if (memberInfo.IsGenericMethodDefinition)
        {

        }
        return Delegate.CreateDelegate(typeof(TDelegate), firstArgument, memberInfo, throwOnBindFailure) as TDelegate;
    }
    public static TDelegate CreatePropertyDelegate<TDelegate>(PropertyInfo memberInfo, object firstArgument = null, bool throwOnBindFailure = true)
        where TDelegate : Delegate
    {
        Type returnType = Helper.GetDelegateInvoke(typeof(TDelegate)).ReturnType;
        if (memberInfo.CanWrite && returnType == typeof(void))
        {
            return CreateMethodDelegate<TDelegate>(memberInfo.SetMethod, firstArgument, throwOnBindFailure);
        }
        if (memberInfo.CanRead && returnType != typeof(void))
        {
            return CreateMethodDelegate<TDelegate>(memberInfo.GetMethod, firstArgument, throwOnBindFailure);
        }
        if (throwOnBindFailure)
        {
            throw Helper.BindFailureException();
        }
        return null;
    }
    public static TDelegate CreateConstructorDelegate<TDelegate>(ConstructorInfo memberInfo, object firstArgument = null, bool throwOnBindFailure = true)
      where TDelegate : Delegate
    {
        MethodInfo invoke = Helper.GetDelegateInvoke(typeof(TDelegate));
        Type returnType = invoke.ReturnType;
        Type[] parameterTypes = invoke.GetParameters().Select(p => p.ParameterType).ToArray();
        Type[] ctorParameterTypes = memberInfo.GetParameters().Select(p => p.ParameterType).ToArray();

        if (returnType != typeof(void) &&
            Helper.IsAssignable(memberInfo.DeclaringType, returnType) &&
            Helper.IsAssignableParameters(parameterTypes, ctorParameterTypes))
        {
            //创建构造函数调用
            ParameterExpression[] ctorParameters = ctorParameterTypes.Select(p => Expression.Parameter(p)).ToArray();
            Expression ctorCall = Expression.New(memberInfo, ctorParameters);
            ctorCall = Expression.Return(Expression.Label(), ctorCall, memberInfo.DeclaringType);

            ParameterExpression[] parameters = parameterTypes.Select(p => Expression.Parameter(p)).ToArray();
            return Expression.Lambda<TDelegate>(ctorCall, parameters).Compile();
        }
        if (throwOnBindFailure)
        {
            throw Helper.BindFailureException();
        }
        return null;
    }
    public static TDelegate CreateFieldDelegate<TDelegate>(FieldInfo memberInfo, object firstArgument = null, bool throwOnBindFailure = true)
        where TDelegate : Delegate
    {
        MethodInfo invoke = Helper.GetDelegateInvoke(typeof(TDelegate));

        if (Helper.IsAssignableSetter(memberInfo, invoke, firstArgument))   //write field
        {
            ParameterExpression[] parameters = invoke.GetParameters().Select(p => Expression.Parameter(p.ParameterType)).ToArray();

            Expression thisArgument = memberInfo.IsStatic ? Expression.Empty() :
                parameters.Length >= 2 ? (Expression)parameters[0] :
                Expression.Constant(firstArgument);
            Expression valueArgument = parameters.Length >= 2 ? parameters[1] : parameters[0];

            return Expression.Lambda<TDelegate>(
                Expression.Assign(Expression.Field(thisArgument, memberInfo), valueArgument),
                parameters
            ).Compile();
        }
        else if (Helper.IsAssignableGetter(memberInfo, invoke, firstArgument))  //read field
        {
            ParameterExpression[] parameters = invoke.GetParameters().Select(p => Expression.Parameter(p.ParameterType)).ToArray();

            Expression thisArgument = memberInfo.IsStatic ? Expression.Empty() :
                parameters.Length >= 1 ? (Expression)parameters[0] :
                Expression.Constant(firstArgument);

            return Expression.Lambda<TDelegate>(
                Expression.Field(thisArgument, memberInfo),
                parameters
            ).Compile();
        }
        if (throwOnBindFailure)
        {
            throw Helper.BindFailureException();
        }
        return null;
    }

    public static TDelegate CreateDelegate<TDelegate>(this MemberInfo memberInfo)
        where TDelegate : Delegate
    {
        return CreateDelegate<TDelegate>(memberInfo, null, true);
    }
    public static TDelegate CreateDelegate<TDelegate>(this MemberInfo memberInfo, bool throwOnBindFailure)
        where TDelegate : Delegate
    {
        return CreateDelegate<TDelegate>(memberInfo, null, throwOnBindFailure);
    }
    public static TDelegate CreateDelegate<TDelegate>(this MemberInfo memberInfo, object firstArgument, bool throwOnBindFailure)
        where TDelegate : Delegate
    {
        if (memberInfo is ConstructorInfo)
        {
            return CreateConstructorDelegate<TDelegate>((ConstructorInfo)memberInfo, firstArgument, throwOnBindFailure);
        }
        if (memberInfo is MethodInfo)
        {
            return CreateMethodDelegate<TDelegate>((MethodInfo)memberInfo, firstArgument, throwOnBindFailure);
        }
        if (memberInfo is PropertyInfo)
        {
            return CreatePropertyDelegate<TDelegate>((PropertyInfo)memberInfo, firstArgument, throwOnBindFailure);
        }
        if (memberInfo is FieldInfo)
        {
            return CreateFieldDelegate<TDelegate>((FieldInfo)memberInfo, firstArgument, throwOnBindFailure);
        }

        if (throwOnBindFailure)
        {
            throw Helper.BindFailureException();
        }
        return null;
    }


    static class Helper
    {
        public static Exception BindFailureException()
        {
            return new Exception("Delegate bind failure: ");
        }

        public static MethodInfo GetDelegateInvoke(Type type)
        {
            return type.GetMethod("Invoke");
        }
        public static bool IsGenericParameterDefinition(Type type, bool finite = true)
        {
            //type.IsGenericParameter
            //var parameterConstraints = type.GetGenericParameterConstraints();

            return false;
        }

        /// <summary>
        /// 前者类型是否可分配给后者类型
        /// </summary>
        /// <param name="from"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsAssignable(Type from, Type target)
        {
            return target.IsAssignableFrom(from);
        }
        /// <summary>
        /// 检查数组中每一个参数类型是否允许对应的分配
        /// </summary>
        /// <param name="from"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsAssignableParameters(Type[] from, Type[] target)
        {
            if (from.Length != target.Length)
                return false;
            for (int i = 0; i < target.Length; i++)
            {
                if (!IsAssignable(from[i], target[i]))
                    return false;
            }
            return true;
        }
        /// <summary>
        /// 检查FieldInfo Write能否分配给指定Delegate类型
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="delegateInvoke"></param>
        /// <param name="firstArgument">指定this绑定, 优先级低于传参</param>
        /// <returns></returns>
        public static bool IsAssignableSetter(FieldInfo memberInfo, MethodInfo delegateInvoke, object firstArgument)
        {
            if (delegateInvoke.ReturnType != typeof(void))
                return false;

            ParameterInfo[] parameters = delegateInvoke.GetParameters();
            if (memberInfo.IsStatic)
            {
                return parameters.Length >= 1 &&
                    IsAssignable(parameters[0].ParameterType, memberInfo.FieldType);
            }
            if (parameters.Length >= 2)
            {
                return IsAssignable(parameters[0].ParameterType, memberInfo.DeclaringType) &&
                    IsAssignable(parameters[1].ParameterType, memberInfo.FieldType);
            }
            if (firstArgument != null && parameters.Length >= 1)
            {
                return IsAssignable(firstArgument.GetType(), memberInfo.DeclaringType) &&
                    IsAssignable(parameters[0].ParameterType, memberInfo.FieldType);
            }
            return false;
        }
        /// <summary>
        /// 检查FieldInfo Read能否分配给指定Delegate类型
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="delegateInvoke"></param>
        /// <param name="firstArgument">指定this绑定, 优先级低于传参</param>
        /// <returns></returns>
        public static bool IsAssignableGetter(FieldInfo memberInfo, MethodInfo delegateInvoke, object firstArgument)
        {
            if (!IsAssignable(memberInfo.FieldType, delegateInvoke.ReturnType))
                return false;
            if (memberInfo.IsStatic)
            {
                return true;
            }
            ParameterInfo[] parameters = delegateInvoke.GetParameters();
            return parameters.Length >= 1 ? IsAssignable(parameters[0].ParameterType, memberInfo.DeclaringType) :
                firstArgument != null ? IsAssignable(firstArgument.GetType(), memberInfo.DeclaringType) :
                false;
        }
    }
}
