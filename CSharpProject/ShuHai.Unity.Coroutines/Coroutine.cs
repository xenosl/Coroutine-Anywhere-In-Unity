using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ShuHai.Unity.Coroutines
{
    using CoroutineDict = Dictionary<IEnumerator, Coroutine>;

    public sealed class Coroutine : IYield
    {
        public readonly IEnumerator Routine;

        public IYield CurrentYield { get; private set; }

        #region Constructors

        public Coroutine(IEnumerator routine, Action done = null) : this(routine, null, done) { }

        public Coroutine(IEnumerator routine, string name, Action done = null) : this(routine, name, 1, done) { }

        public Coroutine(IEnumerator routine, string name, int updateMultiplier, Action done = null)
        {
            Routine = routine ?? throw new ArgumentNullException(nameof(routine));

            this.name = name;

            UpdateMultiplier = updateMultiplier;

            if (done != null)
                Done += done;
        }

        #endregion Constructors

        #region Name

        public string Name
        {
            get
            {
                if (name != null)
                    return name;

                var str = Routine.ToString();
                var splitted = str.Split('<', '>');
                name = splitted.Length == 3 ? splitted[1] : str;

                return name;
            }
        }

        private string name;

        public override string ToString() => typeof(Coroutine).Name + '<' + Name + '>';

        #endregion Name

        #region Flow Control

        #region Start

        public bool Start()
        {
            if (startedInstances.ContainsKey(Routine))
                return false; // Already running.
            StartImpl();
            return true;
        }

        private void StartImpl()
        {
            LogStart();

            startedInstances.Add(Routine, this);

            updates += Update;

            MoveNext();
        }

        /// <summary>
        ///     Initialzies a new coroutine and start it.
        /// </summary>
        /// <param name="routine">The coroutine enumerator.</param>
        /// <param name="done">Event triggered when the coroutine stopped.</param>
        public static Coroutine Start(IEnumerator routine, Action done = null) { return Start(routine, null, done); }

        public static Coroutine Start(IEnumerator routine, string name, Action done = null)
        {
            if (routine == null)
                throw new ArgumentNullException(nameof(routine));

            if (startedInstances.TryGetValue(routine, out var coroutine))
                return coroutine;

            coroutine = new Coroutine(routine, name, done);
            coroutine.StartImpl();
            return coroutine;
        }

        #endregion Start

        #region Stop

        public event Action Done;

        public bool Stop()
        {
            if (!startedInstances.ContainsKey(Routine))
                return false; // Not running.
            StopImpl(false);
            return true;
        }

        private void StopImpl(bool yieldStopped)
        {
            if (!yieldStopped)
                StopCurrentYield();
            CurrentYield = null;

            updates -= Update;

            startedInstances.Remove(Routine);

            Done?.Invoke();
            Done = null;

            LogStop();
        }

        /// <summary>
        ///     Stops a coroutine.
        /// </summary>
        /// <param name="routine">The coroutine enumerator.</param>
        /// <returns>
        ///     <see langword="true" /> if the coroutine successfully stopped; or <see langword="false" /> if the specified
        ///     coroutine sequence already stopped.
        /// </returns>
        public static bool Stop(IEnumerator routine)
        {
            if (routine == null)
                throw new ArgumentNullException(nameof(routine));

            if (!startedInstances.TryGetValue(routine, out var coroutine))
                return false;
            coroutine.StopImpl(false);
            return true;
        }

        #endregion Stop

        #region Finish

        public void Finish()
        {
            Start();
            FinishImpl();
        }

        private void FinishImpl()
        {
            while (!IsDone)
            {
                if (CurrentYield is Coroutine coroutineYield && !coroutineYield.IsDone)
                    coroutineYield.Finish();

                MoveNext();

                FinishImpl();
            }
        }

        /// <summary>
        ///     Initializes a new coroutine and execute it to the end immediately.
        /// </summary>
        /// <param name="routine">Enumerator of the specified coroutine.</param>
        /// <param name="done">Event triggered when the coroutine is done.</param>
        /// <remarks>
        ///     This means that treat the coroutine as normal execution sequence and synchronal executed.
        ///     Note that <see cref="IYield.Update" /> method of all yield instructions would not be called during the execution.
        /// </remarks>
        public static void Finish(IEnumerator routine, Action done = null)
        {
            if (routine == null)
                throw new ArgumentNullException(nameof(routine));

            if (!startedInstances.TryGetValue(routine, out var coroutine))
                coroutine = new Coroutine(routine);

            coroutine.Done += done;
            coroutine.Finish();
        }

        #endregion Finish

        #region Pause

        public bool SelfPaused;

        #endregion Pause

        #region Update

        /// <summary>
        ///     Decide how many times the current instance is updated in one frame.
        /// </summary>
        public int UpdateMultiplier
        {
            get => updateMultiplier;
            set => updateMultiplier = Mathf.Clamp(value, 1, int.MaxValue);
        }

        private int updateMultiplier = 1;

        private void Update()
        {
            for (int i = 0; i < UpdateMultiplier; ++i)
            {
                if (!SelfPaused)
                    UpdateImpl();
            }
        }

        private void UpdateImpl()
        {
            if (CurrentYield == null)
            {
                MoveNext();
            }
            else
            {
                CurrentYield.Update();
                if (CurrentYield.IsDone)
                    MoveNext();
            }
        }

        private static event Action updates;

        internal static void UpdateAll() { updates?.Invoke(); }

        #endregion Update

        private bool moveEnded;

        private void MoveNext()
        {
            StopCurrentYield();

            moveEnded = !Routine.MoveNext();
            if (!moveEnded)
            {
                CurrentYield = ToYield(Routine.Current);
                StartCurrentYield();
            }
            else
            {
                StopImpl(true);
            }
        }

        private void StartCurrentYield() { CurrentYield?.Start(); }

        private void StopCurrentYield() { CurrentYield?.Stop(); }

        private static readonly CoroutineDict startedInstances = new CoroutineDict();

        private static IYield ToYield(object obj)
        {
            if (obj == null)
                return null;

            if (obj is IYield yield)
                return yield;

            var type = obj.GetType();
            var adapter = GetYieldAdapter(type);
            if (adapter != null)
                yield = adapter.ToYield(obj);
            else
                throw new NotSupportedException($@"Yield ""{type}"" is not supported");

            return yield;
        }

        #endregion Flow Control

        #region As Yield Instruction

        public bool IsDone => CurrentYield != null ? CurrentYield.IsDone && moveEnded : moveEnded;

        void IYield.Start()
        {
            if (!Start())
                throw new InvalidOperationException("Coroutine already started");
        }

        void IYield.Stop() { Stop(); }

        void IYield.Update() { }

        #endregion As Yield Instruction

        #region Yield Adapters

        public static void SetYieldAdapter(Type type, IYieldAdapter adapter)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            yieldAdapters[type] = adapter ?? throw new ArgumentNullException(nameof(adapter));
        }

        public static IYieldAdapter GetYieldAdapter(Type type, bool useBaseOnNotFound = true)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (!useBaseOnNotFound)
                return yieldAdapters.TryGetValue(type, out var adapter) ? adapter : null;

            var t = type;
            while (t != null)
            {
                if (yieldAdapters.TryGetValue(t, out var adapter))
                    return adapter;
                t = t.BaseType;
            }
            return null;
        }

        private static readonly Dictionary<Type, IYieldAdapter> yieldAdapters = new Dictionary<Type, IYieldAdapter>();

        private static void InitializeYieldAdapters()
        {
            var rootType = typeof(IYieldAdapter);
            var adapterTypes = rootType.Assembly.GetTypes()
                .Where(t => !t.IsInterface && !t.IsAbstract && rootType.IsAssignableFrom(t));

            foreach (var type in adapterTypes)
            {
                var instance = (IYieldAdapter)Activator.CreateInstance(type);
                var target = type.GetCustomAttribute<TargetTypeAttribute>();
                if (target != null)
                    SetYieldAdapter(target.Value, instance);

                var targets = type.GetCustomAttribute<TargetTypesAttribute>();
                if (targets != null)
                {
                    foreach (var t in targets.Values)
                        SetYieldAdapter(t, instance);
                }
            }
        }

        #endregion Yield Adapters

        static Coroutine() { InitializeYieldAdapters(); }

        #region Debug

        public static bool DebugLogEnabled;

        public static Func<bool> DebugLogPrecondition;

        private static void Log(string message)
        {
            if (DebugLogEnabled && (DebugLogPrecondition?.Invoke() ?? false))
                Debug.Log(message);
        }

        private void LogStart() { Log(Name + " Start"); }

        private void LogStop() { Log(Name + " Stop"); }

        #endregion Debug
    }
}