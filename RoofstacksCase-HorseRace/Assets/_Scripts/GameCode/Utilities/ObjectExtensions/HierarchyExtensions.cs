using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameCode.Utilities.ObjectExtensions
{
    public static class HierarchyExtensions
    {
        public static IEnumerable<T> FindObjectsOfType<T>()
        {
            return Object.FindObjectsOfType<MonoBehaviour>().OfType<T>();
        }
    }
}
