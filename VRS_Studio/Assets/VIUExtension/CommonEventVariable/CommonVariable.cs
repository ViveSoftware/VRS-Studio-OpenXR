using System;
using System.Collections.Generic;

namespace HTC.UnityPlugin.CommonEventVariable
{
    public abstract class CommonVariableHandlerBase : CommonHandler
    {
        public long LastSetValueTime { get; protected set; } // DateTime.UtcNow.Ticks
        public int SetValueCount { get; protected set; }
        public void ResetSetValueCount() { SetValueCount = 0; }
        public abstract Type ValueType { get; }
        public abstract string PreviousValueString { get; }
        public abstract string CurrentValueString { get; }
        public abstract object PreviousValueObject { get; }
        public abstract object CurrentValueObject { get; }

        public abstract event Action OnChange;
    }

    public abstract class CommonVariableHandler<T> : CommonVariableHandlerBase
    {
        public abstract T PreviousValue { get; }
        public abstract T CurrentValue { get; }
        public sealed override string PreviousValueString { get { return PreviousValue == null ? "(null)" : PreviousValue.ToString(); } }
        public sealed override string CurrentValueString { get { return CurrentValue == null ? "(null)" : CurrentValue.ToString(); } }
        public sealed override object PreviousValueObject { get { return PreviousValue; } }
        public sealed override object CurrentValueObject { get { return CurrentValue; } }
        public abstract bool IsOnChangeComparerOverridden { get; }
        public abstract bool SetValue(T value); // return true if value changed (CurrentValue != PreviousValue)
        public abstract void OverrideOnChangeComparer(Func<T, T, bool> func);
    }

    public static class CommonVariable
    {
        private sealed class Handler<T> : CommonVariableHandler<T>
        {
            private static Dictionary<string, Handler<T>> handlers = new Dictionary<string, Handler<T>>();

            private T previousValue;
            private T currentValue;
            private Func<T, T, bool> overrideEqual;

            public override event Action OnChange;

            public override Type ValueType { get { return typeof(T); } }
            public override T PreviousValue { get { return previousValue; } }
            public override T CurrentValue { get { return currentValue; } }
            public override bool IsOnChangeComparerOverridden { get { return overrideEqual != null; } }

            public override void OverrideOnChangeComparer(Func<T, T, bool> func)
            {
                if (IsOnChangeComparerOverridden)
                {
                    SendDebugMessage("[CommonVariable] " + Name + " overriding OnChangeComparer while it was overridden already");
                }
                overrideEqual = func;
            }

            public override bool SetValue(T value)
            {
                ++SetValueCount;
                SetLastSetValueTime(LastSetValueTime = DateTime.UtcNow.Ticks);
                previousValue = currentValue;
                currentValue = value;

                var changed = overrideEqual == null ? !EqualityComparer<T>.Default.Equals(previousValue, currentValue) : !overrideEqual(previousValue, currentValue);
                if (changed)
                {
                    if (EnableDebugMessage || EnableAllVariableDebugMessage || EnableAllDebugMessage)
                    {
                        SendDebugMessage("[CommonVariable] " + Name + " " + PreviousValueString + " => " + CurrentValueString + " (" + SetValueCount + ")");
                    }

                    if (OnChange != null) { OnChange(); }
                }
                else if (!VariableDebugMessageOnlyOnChange)
                {
                    if (EnableDebugMessage || EnableAllDebugMessage)
                    {
                        SendDebugMessage("[CommonVariable] " + Name + " " + CurrentValueString + " (" + SetValueCount + ")");
                    }
                }

                return changed;
            }

            public static Handler<T> Get(string name)
            {
                Handler<T> handler;
                if (string.IsNullOrEmpty(name)) { throw new ArgumentException("name cannot be null or empty", "name"); }
                if (!handlers.TryGetValue(name, out handler))
                {
                    handler = new Handler<T>() { Name = name };
                    handlers.Add(name, handler);
                    AddHandler(handler);
                }
                return handler;
            }

            public static Handler<T> Get(string name, bool enableDebugMessage)
            {
                var handler = Get(name);
                handler.EnableDebugMessage = enableDebugMessage;
                return handler;
            }

            public static Handler<T> Get(string name, Func<T, T, bool> func)
            {
                var handler = Get(name);
                handler.OverrideOnChangeComparer(func);
                return handler;
            }

            public static Handler<T> Get(string name, bool enableDebugMessage, Func<T, T, bool> func)
            {
                var handler = Get(name);
                handler.EnableDebugMessage = enableDebugMessage;
                handler.OverrideOnChangeComparer(func);
                return handler;
            }

            public static bool TryGet(string name, out CommonVariableHandler<T> handler)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    Handler<T> h;
                    if (handlers.TryGetValue(name, out h))
                    {
                        handler = h;
                        return true;
                    }
                }

                handler = null;
                return false;
            }

            public static bool Exists(string name)
            {
                return !string.IsNullOrEmpty(name) && handlers.ContainsKey(name);
            }
        }

        private static long lastSetValueTime;
        private static List<CommonVariableHandlerBase> handlers = new List<CommonVariableHandlerBase>();

        private static void SetLastSetValueTime(long time) { lastSetValueTime = time; }

        private static void AddHandler(CommonVariableHandlerBase handler) { handlers.Add(handler); }

        public static int VariablesCount() { return handlers.Count; }

        public static long LastSetValueTime() { return lastSetValueTime; }

        public static IEnumerable<CommonVariableHandlerBase> AllVariables() { return handlers; }

        public static CommonVariableHandler<T> Get<T>(string name) { return Handler<T>.Get(name); }
        public static CommonVariableHandler<T> Get<T>(string name, bool enableDebugMessage) { return Handler<T>.Get(name, enableDebugMessage); }
        public static CommonVariableHandler<T> Get<T>(string name, Func<T, T, bool> func) { return Handler<T>.Get(name, func); }
        public static CommonVariableHandler<T> Get<T>(string name, bool enableDebugMessage, Func<T, T, bool> func) { return Handler<T>.Get(name, enableDebugMessage, func); }
        public static bool TryGet<T>(string name, out CommonVariableHandler<T> handler) { return Handler<T>.TryGet(name, out handler); }
        public static bool Exists<T>(string name) { return Handler<T>.Exists(name); }
    }
}
