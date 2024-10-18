using System;
using System.Collections.Generic;

namespace NeuroInscryption.Utilities.Reflection;

public static class ReflectionHelpers
{
    public static IEnumerable<Type> WalkTypeHierarchy(Type? leaf)
    {
        for (Type? curr = leaf; curr != null; curr = curr.BaseType)
        {
            yield return curr;
        }
    }
}
