﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Sleipner.Core.Util
{
    public class ProxiedMethodInvocation<T, TResult> where T : class
    {
        public readonly MethodInfo Method;
        public readonly object[] Parameters;
        private static readonly IDictionary<MethodInfo, DelegateFactory.LateBoundMethod> _lateBoundMethodCache = new Dictionary<MethodInfo, DelegateFactory.LateBoundMethod>();

        public ProxiedMethodInvocation(MethodInfo method, object[] parameters)
        {
            if (method.DeclaringType != typeof(T))
            {
                throw new ArgumentException("Declaring type is not T", "method");
            }

            Method = method;
            Parameters = parameters;
        }

        public TResult Invoke(T implementation)
        {
            var delegateMethod = GetLateBoundMethod(Method);
            return (TResult)delegateMethod(implementation, Parameters);
        }

        public async Task<TResult> InvokeAsync(T implementation)
        {
            var delegateMethod = GetLateBoundMethod(Method);
            return (TResult)delegateMethod(implementation, Parameters);
        }

        private DelegateFactory.LateBoundMethod GetLateBoundMethod(MethodInfo methodInfo)
        {
            DelegateFactory.LateBoundMethod lateBoundMethod;
            if (!_lateBoundMethodCache.TryGetValue(methodInfo, out lateBoundMethod))
            {
                lateBoundMethod = DelegateFactory.Create(methodInfo);
                _lateBoundMethodCache[methodInfo] = lateBoundMethod;
            }

            return lateBoundMethod;
        }

        public bool Equals(ProxiedMethodInvocation<T, TResult> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Equals(Method, other.Method) && Parameters.SequenceEqual(other.Parameters, new CollectionComparer());
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Method != null ? Method.GetHashCode() : 0) * 397);
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ProxiedMethodInvocation<T, TResult>)obj);
        }
    }
}
