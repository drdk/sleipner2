﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sleipner.Core.Util;

namespace Sleipner.Cache.Policies
{
    public class BasicConfigurationProvider<T> : ICachePolicyProvider<T> where T : class
    {
        private CachePolicy _default;
        private readonly IList<ConfiguredMethodHandle<T>> _configuredMethods = new List<ConfiguredMethodHandle<T>>(); 

        public CachePolicy GetPolicy(MethodInfo methodInfo, IEnumerable<object> arguments)
        {
            var policyHandle = _configuredMethods.FirstOrDefault(a => a.ConfiguredMethod.IsMatch(methodInfo, arguments));
            if (policyHandle == null)
                return _default;

            return policyHandle.Policy;
        }

        public CachePolicy GetPolicy<TResult>(ProxiedMethodInvocation<T, TResult> invocation)
        {
            return GetPolicy(invocation.Method, invocation.Parameters);
        }

        public CachePolicy RegisterMethodConfiguration(IConfiguredMethod<T> methodConfiguration)
        {
            var methodHandle = new ConfiguredMethodHandle<T>()
                                   {
                                       ConfiguredMethod = methodConfiguration
                                   };
            _configuredMethods.Add(methodHandle);

            return methodHandle.Policy;
        }

        public CachePolicy GetDefaultPolicy()
        {
            return _default ?? (_default = new CachePolicy());
        }
    }

    internal class ConfiguredMethodHandle<T> where T : class
    {
        public IConfiguredMethod<T> ConfiguredMethod;
        public CachePolicy Policy = new CachePolicy();
    }
}