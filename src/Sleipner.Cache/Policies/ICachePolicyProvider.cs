﻿using System.Collections.Generic;
using System.Reflection;

namespace Sleipner.Cache.Policies
{
    public interface ICachePolicyProvider<T> where T : class
    {
        CachePolicy GetPolicy(MethodInfo methodInfo, IEnumerable<object> arguments);
        CachePolicy RegisterMethodConfiguration(IConfiguredMethod<T> methodConfiguration);
        CachePolicy GetDefaultPolicy();
    }
}