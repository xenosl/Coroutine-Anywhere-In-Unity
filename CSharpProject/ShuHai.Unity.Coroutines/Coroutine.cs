using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ShuHai.Unity.Coroutines
{
    using CoroutineDict = Dictionary<IEnumerator, Coroutine>;

    /// <summary>
    ///     Represents a coroutine. Provides functionalities for controling coroutines.
    /// </summary>
    public sealed class Coroutine : IYield
    {
        /// <summary>
        ///     The coroutine enumerator that current instance represents.
        /// </summary>
        public readonly IEnumerator Routine;

        /// <summary>
        ///     Current yield object of current instance (if the current instance is started).
        /// </summary>
        public IYield CurrentYield { get; private set; }

        #region Constructors

        /// <summary>
        ///     Initialize a new coroutine with optional callback on its execution done.
        /// </summary>
        /// <param name="routine">The coroutine to start.</param>
        /// <param name="done">Callback executes when the coroutine is done.</param>
        public Coroutine(IEnumerator routine, Action done = null) : this(routine, null, done) { }

        /// <summary>
        ///     Initialize a new coroutine with specified name and optional callback on its execution done.
        /// </summary>
        /// <param name="routine">The coroutine to start.</param>
        /// <param name="name">Name of the coroutine, commonly used for debug log.</param>
        /// <param name="done">Callback executes when the coroutine is done.</param>
        public Coroutine(IEnumerator routine, string name, Action done = null) : this(routine, name, 1, done) { }

        /// <summary>
        ///     Initialize a new coroutine with specified name, update multiplier and optional callback on its execution done.
        /// </summary>
        /// <param name="routine">The coroutine to start.</param>
        /// <param name="name">Name of the coroutine, commonly used for debug log.</param>
        /// <param name="updateMultiplier">Determines how many times the coroutine is executed per frame.</param>
        /// <param name="done">Callback executes when the coroutine is done.</param>
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

        /// <summary>
        ///     Name of the coroutine. The value is assigned from constructor or extract from <see cref="Routine" /> object.
        ///     Commonly used for debug when <see cref="DebugLogEnabled" /> is <see langword="true" />.
        /// </summary>
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

        /// <inheritdoc />
        public override string ToString() => typeof(Coroutine).Name + '<' + Name + '>';

        #endregion Name

        #region Flow Control

        #region Start

        /// <summary>
        ///     Starts the coroutine.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if the coroutine successfully starts; otherwise, <see langword="false" /> if the
        ///     coroutine is already started.
        /// </returns>
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
        /// <param name="routine">The coroutine to start.</param>
        /// <param name="done">Callback that executes when the coroutine stops.</param>
        public static Coroutine Start(IEnumerator routine, Action done = null) { return Start(routine, null, done); }

        /// <summary>
        ///     Initialize a new coroutine and start it.
        /// </summary>
        /// <param name="routine">The coroutine to start.</param>
        /// <param name="name">Name of the coroutine, commonly used for debug log.</param>
        /// <param name="done">Callback that executes when the coroutine stops.</param>
        /// <returns></returns>
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

        /// <summary>
        ///     Event that triggers when the current coroutine instance is done (stopped).
        /// </summary>
        public event Action Done;

        /// <summary>
        ///     Stops the coroutine.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if the coroutine successfully stopped; otherwise, <see langword="false" /> if the
        ///     coroutine is not running.
        /// </returns>
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
        ///     Stops the coroutine.
        /// </summary>
        /// <param name="routine">The coroutine to stop.</param>
        /// <returns>
        ///     <see langword="true" /> if the coroutine successfully stopped; otherwise, <see langword="false" /> if the
        ///     coroutine is not running.
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

        /// <summary>
        ///     Finish executing the coroutine immediately (execute the coroutine until it is done).
        /// </summary>
        /// <remarks>
        ///     Whether the coroutine is done or not is determined by the return value of <see cref="IEnumerator.MoveNext" />,
        ///     this makes the coroutine never stop until <see cref="IEnumerator.MoveNext" /> of <see cref="Routine" /> field
        ///     returns <see langword="false" />. This means that if your coroutine never ends, call this method leads to a DEAD
        ///     LOOP!
        /// </remarks>
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
        ///     Initializes a new coroutine and execute until it is done.
        /// </summary>
        /// <param name="routine">The coroutine to execute.</param>
        /// <param name="done">Callback that executes when the coroutine stops.</param>
        /// <remarks>
        ///     Whether the coroutine is done or not is determined by the return value of <see cref="IEnumerator.MoveNext" />,
        ///     this makes the coroutine never stop until <see cref="IEnumerator.MoveNext" /> of <see cref="Routine" /> field
        ///     returns <see langword="false" />. This means that if your coroutine never ends, call this method leads to a DEAD
        ///     LOOP!
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

        #region Update

        /// <summary>
        ///     Determines how many times the current coroutine is updated per frame.
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
                UpdateImpl();
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

        /// <summary>
        ///     Indicates whether the current instance is done.
        /// </summary>
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

        /// <summary>
        ///     Set the yield adapter for specified type of objects that not implemented <see cref="IYield" />. In this way you can
        ///     treat any object as yield object for the coroutine.
        /// </summary>
        /// <param name="type">Type of object to be converted for the adapter.</param>
        /// <param name="adapter">
        ///     The adapter that converts objects from specified <paramref name="type" /> to <see cref="IYield" />.
        /// </param>
        public static void SetYieldAdapter(Type type, IYieldAdapter adapter)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            yieldAdapters[type] = adapter ?? throw new ArgumentNullException(nameof(adapter));
        }

        /// <summary>
        ///     Get the yield adapter for specified type.
        /// </summary>
        /// <param name="type">Type of object to be converted for the adapter.</param>
        /// <param name="useBaseOnNotFound">
        ///     Base type of <paramref name="type" /> is used if adapter for <paramref name="type" /> not found.
        /// </param>
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
                var target = type.GetCustomAttribute<YieldAdapterTargetAttribute>();
                if (target != null)
                    SetYieldAdapter(target.Type, instance);

                var targets = type.GetCustomAttribute<YieldAdapterTargetsAttribute>();
                if (targets != null)
                {
                    foreach (var t in targets.Types)
                        SetYieldAdapter(t, instance);
                }
            }
        }

        #endregion Yield Adapters

        static Coroutine() { InitializeYieldAdapters(); }

        #region Debug

        /// <summary>
        ///     Indicates whether debug log is enabled.
        /// </summary>
        /// <remarks>
        ///     If <see cref="DebugLogEnabled" /> is set to <see langword="true" />, <see cref="Debug.Log(object)" /> is called
        ///     when any coroutine starts or stops with its name.
        /// </remarks>
        public static bool DebugLogEnabled;

        /// <summary>
        ///     Determine whether the debug messages is logged. All messages are logged if the value is set to
        ///     <see langword="null" />; or messages are logged when the delegate returns <see langword="true" />.
        /// </summary>
        public static Func<Coroutine, bool> DebugLogPrecondition;

        private static void Log(Coroutine coroutine, string message)
        {
            if (DebugLogEnabled && (DebugLogPrecondition?.Invoke(coroutine) ?? false))
                Debug.Log(message);
        }

        private void LogStart() { Log(this, Name + " Start"); }

        private void LogStop() { Log(this, Name + " Stop"); }

        #endregion Debug
    }
}