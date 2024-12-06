using System;
using System.Collections.Generic;

namespace HTC.UnityPlugin.CommonEventVariable
{
    public abstract class CommonEventHandlerBase : CommonHandler
    {
        public long LastTriggerTime { get; protected set; } // DateTime.UtcNow.Ticks
        public int TriggerCount { get; protected set; }
        public void ResetTriggerCount() { TriggerCount = 0; }

        public abstract int ParamCount { get; }
        public abstract Type ParamType(int index);
    }

    public abstract class CommonEventHandler : CommonEventHandlerBase
    {
        public const int PARAM_COUNT = 0;

        public abstract void Trigger();
        public abstract event Action OnTrigger;

        public sealed override int ParamCount { get { return PARAM_COUNT; } }
        public sealed override Type ParamType(int index)
        {
            return default(Type);
        }
    }

    public abstract class CommonEventHandler<T> : CommonEventHandlerBase
    {
        public const int PARAM_COUNT = 1;

        public abstract void Trigger(T value);
        public abstract event Action<T> OnTrigger;

        public sealed override int ParamCount { get { return PARAM_COUNT; } }
        public sealed override Type ParamType(int index)
        {
            switch (index)
            {
                case 0: return typeof(T);
                default: return default(Type);
            }
        }
    }

    public abstract class CommonEventHandler<T1, T2> : CommonEventHandlerBase
    {
        public const int PARAM_COUNT = 2;

        public abstract void Trigger(T1 value1, T2 value2);
        public abstract event Action<T1, T2> OnTrigger;

        public sealed override int ParamCount { get { return PARAM_COUNT; } }
        public sealed override Type ParamType(int index)
        {
            switch (index)
            {
                case 0: return typeof(T1);
                case 1: return typeof(T2);
                default: return default(Type);
            }
        }
    }

    public static class CommonEvent
    {
        private sealed class Handler : CommonEventHandler
        {
            private static Dictionary<string, Handler> handlers = new Dictionary<string, Handler>();

            public override event Action OnTrigger;
            public override void Trigger()
            {
                ++TriggerCount;
                SetLastTriggerTime(PARAM_COUNT, LastTriggerTime = DateTime.UtcNow.Ticks);
                if (EnableDebugMessage || EnableAllEventDebugMessage || EnableAllDebugMessage)
                {
                    SendDebugMessage("[CommonEvent] " + Name + " (" + TriggerCount + ")");
                }
                if (OnTrigger != null) { OnTrigger(); }
            }

            public static Handler Get(string name)
            {
                Handler handler;
                if (string.IsNullOrEmpty(name)) { throw new ArgumentException("name cannot be null or empty", "name"); }
                if (!handlers.TryGetValue(name, out handler))
                {
                    handler = new Handler() { Name = name };
                    handlers.Add(name, handler);
                    AddHandler(PARAM_COUNT, handler);
                }
                return handler;
            }

            public static Handler Get(string name, bool enableDebugMessage)
            {
                var handler = Get(name);
                handler.EnableDebugMessage = enableDebugMessage;
                return handler;
            }

            public static bool TryGet(string name, out CommonEventHandler handler)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    Handler h;
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

        [Serializable]
        private sealed class Handler<T> : CommonEventHandler<T>
        {
            private static Dictionary<string, Handler<T>> handlers = new Dictionary<string, Handler<T>>();

            public override event Action<T> OnTrigger;

            public override void Trigger(T value)
            {
                ++TriggerCount;
                SetLastTriggerTime(PARAM_COUNT, LastTriggerTime = DateTime.UtcNow.Ticks);
                if (EnableDebugMessage || EnableAllEventDebugMessage || EnableAllDebugMessage)
                {
                    SendDebugMessage("[CommonEvent] " + Name + " " + value.ToString() + " (" + TriggerCount + ")");
                }
                if (OnTrigger != null) { OnTrigger(value); }
            }

            public static Handler<T> Get(string name)
            {
                Handler<T> handler;
                if (!handlers.TryGetValue(name, out handler))
                {
                    handler = new Handler<T>() { Name = name };
                    handlers.Add(name, handler);
                    AddHandler(PARAM_COUNT, handler);
                }
                return handler;
            }

            public static Handler<T> Get(string name, bool enableDebugMessage)
            {
                var handler = Get(name);
                handler.EnableDebugMessage = enableDebugMessage;
                return handler;
            }

            public static bool TryGet(string name, out CommonEventHandler<T> handler)
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

        [Serializable]
        private sealed class Handler<T1, T2> : CommonEventHandler<T1, T2>
        {
            private static Dictionary<string, Handler<T1, T2>> handlers = new Dictionary<string, Handler<T1, T2>>();

            public override event Action<T1, T2> OnTrigger;

            public override void Trigger(T1 value1, T2 value2)
            {
                ++TriggerCount;
                SetLastTriggerTime(PARAM_COUNT, LastTriggerTime = DateTime.UtcNow.Ticks);
                if (EnableDebugMessage || EnableAllEventDebugMessage || EnableAllDebugMessage)
                {
                    SendDebugMessage("[CommonEvent] " + Name + " " + value1.ToString() + " " + value2.ToString() + " (" + TriggerCount + ")");
                }
                if (OnTrigger != null) { OnTrigger(value1, value2); }
            }

            public static Handler<T1, T2> Get(string name)
            {
                Handler<T1, T2> handler;
                if (!handlers.TryGetValue(name, out handler))
                {
                    handler = new Handler<T1, T2>() { Name = name };
                    handlers.Add(name, handler);
                    AddHandler(PARAM_COUNT, handler);
                }
                return handler;
            }

            public static Handler<T1, T2> Get(string name, bool enableDebugMessage)
            {
                var handler = Get(name);
                handler.EnableDebugMessage = enableDebugMessage;
                return handler;
            }

            public static bool TryGet(string name, out CommonEventHandler<T1, T2> handler)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    Handler<T1, T2> h;
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

        private static List<long> lastTriggerTime = new List<long>();
        private static List<List<CommonEventHandlerBase>> handlersLists = new List<List<CommonEventHandlerBase>>();

        private static void SetLastTriggerTime(int paramCount, long time)
        {
            while (paramCount >= lastTriggerTime.Count) { lastTriggerTime.Add(0L); }
            lastTriggerTime[paramCount] = time;
        }

        private static void AddHandler(int paramCount, CommonEventHandlerBase handler)
        {
            while (paramCount >= handlersLists.Count) { handlersLists.Add(null); }
            if (handlersLists[paramCount] == null) { handlersLists[paramCount] = new List<CommonEventHandlerBase>(); }
            handlersLists[paramCount].Add(handler);
        }

        public static int MaxExistingEventsParamCount { get { return handlersLists.Count; } }

        public static int EventsCount(int paramCount)
        {
            return paramCount >= 0 && paramCount < handlersLists.Count && handlersLists[paramCount] != null ? handlersLists[paramCount].Count : 0;
        }

        public static long LastTriggerTime(int paramCount)
        {
            return paramCount >= 0 && paramCount < lastTriggerTime.Count ? lastTriggerTime[paramCount] : 0L;
        }

        public static IEnumerable<CommonEventHandlerBase> AllEvents(int paramCount)
        {
            return paramCount >= 0 && paramCount < handlersLists.Count ? handlersLists[paramCount] : System.Linq.Enumerable.Empty<CommonEventHandlerBase>();
        }

        public static CommonEventHandler Get(string name) { return Handler.Get(name); }
        public static CommonEventHandler Get(string name, bool enableDebugMessage) { return Handler.Get(name, enableDebugMessage); }
        public static bool TryGet(string name, out CommonEventHandler handler) { return Handler.TryGet(name, out handler); }
        public static bool Exists(string name) { return Handler.Exists(name); }

        public static CommonEventHandler<T> Get<T>(string name) { return Handler<T>.Get(name); }
        public static CommonEventHandler<T> Get<T>(string name, bool enableDebugMessage) { return Handler<T>.Get(name, enableDebugMessage); }
        public static bool TryGet<T>(string name, out CommonEventHandler<T> handler) { return Handler<T>.TryGet(name, out handler); }
        public static bool Exists<T>(string name) { return Handler<T>.Exists(name); }

        public static CommonEventHandler<T1, T2> Get<T1, T2>(string name) { return Handler<T1, T2>.Get(name); }
        public static CommonEventHandler<T1, T2> Get<T1, T2>(string name, bool enableDebugMessage) { return Handler<T1, T2>.Get(name, enableDebugMessage); }
        public static bool TryGet<T1, T2>(string name, out CommonEventHandler<T1, T2> handler) { return Handler<T1, T2>.TryGet(name, out handler); }
        public static bool Exists<T1, T2>(string name) { return Handler<T1, T2>.Exists(name); }
    }
}
