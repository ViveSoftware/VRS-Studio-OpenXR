using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HTC.UnityPlugin.Utility
{
    public static class ExecuteEventExtension
    {
        public static bool Execute<THandler>(this GameObject target, Action<THandler> func, bool activeInHierarchyOnly = false) where THandler : IEventSystemHandler
        {
            if (target == null) { return false; }

            if (activeInHierarchyOnly && target.activeInHierarchy) { return false; }

            var handlers = ListPool<THandler>.Get();
            try
            {
                target.GetComponents(handlers);
                foreach (var handler in handlers)
                {
                    try
                    {
                        if (handler != null) { func(handler); };
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }

                return handlers.Count > 0;
            }
            finally
            {
                ListPool<THandler>.Release(handlers);
            }
        }

        public static GameObject ExecuteHierarchy<THandler>(this GameObject target, Action<THandler> func, bool activeInHierarchyOnly = false) where THandler : IEventSystemHandler
        {
            // find first target that has handlers in target event chain
            for (var t = target == null ? null : target.transform; t != null; t = t.parent)
            {
                if (Execute(t.gameObject, func, activeInHierarchyOnly))
                {
                    return t.gameObject;
                }
            }

            return null;
        }
    }
}