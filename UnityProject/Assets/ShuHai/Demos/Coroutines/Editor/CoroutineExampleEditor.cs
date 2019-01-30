using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace ShuHai.Unity.Coroutines.Demos.Editor
{
    [CustomEditor(typeof(CoroutineExample))]
    public class CoroutineExampleEditor : UnityEditor.Editor
    {
        private void OnEnable()
        {
            InitializeNull();
            InitializeWaitSeconds();
            InitializeNested();
            InitializeMixed();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            nullContext.GUI();
            waitSecondsContext.GUI();
            nestedContext.GUI();
            mixedContext.GUI();
        }

        public override bool RequiresConstantRepaint() { return true; }

        #region Null

        private Context nullContext;

        private void InitializeNull() { nullContext = new Context("Null", Null); }

        private IEnumerator Null()
        {
            Debug.Log("Null Begin");
            yield return null;
            Debug.Log("Null End");
        }

        #endregion

        #region WaitSeconds

        private Context waitSecondsContext;

        private float waitSecondsDuration = 2;

        private void InitializeWaitSeconds()
        {
            waitSecondsContext = new Context("WaitSeconds", WaitForSeconds, WaitForSecondsGUI);
        }

        private void WaitForSecondsGUI()
        {
            waitSecondsDuration = EditorGUILayout.FloatField("Duration", waitSecondsDuration);
        }

        private IEnumerator WaitForSeconds()
        {
            Debug.Log("WaitForSeconds Begin");
            yield return new WaitSeconds(waitSecondsDuration);
            Debug.Log("WaitForSeconds End");
        }

        #endregion WaitSeconds

        #region Nested

        private Context nestedContext;

        private void InitializeNested() { nestedContext = new Context("Nested", Nested); }

        private IEnumerator Nested()
        {
            Debug.Log("Nested Begin");
            yield return new Coroutine(Nested_1());
            Debug.Log("Nested Nested_1 -> Nested_2");
            yield return new Coroutine(Nested_2());
            Debug.Log("Nested End");
        }

        private IEnumerator Nested_1()
        {
            Debug.Log("Nested_1 Begin");
            yield return new WaitSeconds(0);
            Debug.Log("Nested_1 End");
        }

        private IEnumerator Nested_2()
        {
            Debug.Log("Nested_2 Begin");
            yield return new WaitSeconds(0);
            Debug.Log("Nested_2 End");
        }

        #endregion Nested

        #region Mixed

        private bool mixed_1_1_break;
        private float mixed_1_1_duration = 2;

        private Context mixedContext;

        private void InitializeMixed() { mixedContext = new Context("Mixed", Mixed, MixedGUI); }

        private void MixedGUI()
        {
            mixed_1_1_break = EditorGUILayout.Toggle("mixed_1_1 break", mixed_1_1_break);
            mixed_1_1_duration = EditorGUILayout.FloatField("mixed_1_1 duration", mixed_1_1_duration);
        }

        private IEnumerator Mixed()
        {
            Debug.Log("Mixed Begin");
            yield return new Coroutine(Mixed_1());
            Debug.Log("Mixed End");
        }

        private IEnumerator Mixed_1()
        {
            Debug.Log("Mixed_1 Begin");
            yield return new Coroutine(Mixed_1_1());
            Debug.Log("Mixed_1_1 -> Mixed_1_2");
            yield return new Coroutine(Mixed_1_2());
            Debug.Log("Mixed_1 End");
        }

        private IEnumerator Mixed_1_1()
        {
            Debug.Log("Mixed_1_1 Begin");

            if (mixed_1_1_break)
            {
                Debug.Log("Mixed_1_1 Break");
                yield break;
            }
            yield return new WaitSeconds(mixed_1_1_duration);

            Debug.Log("Mixed_1_1 End");
        }

        private IEnumerator Mixed_1_2()
        {
            Debug.Log("Mixed_1_2 Begin");
            yield return null;
            Debug.Log("Mixed_1_2 End");
        }

        #endregion Mixed

        #region Misc

        private void Execute() { Coroutine.Start(Routine1()); }

        private int executeCount;

        private IEnumerator Routine1()
        {
            yield return new WaitForSeconds(2);
            for (int i = 0; i < 42; ++i)
            {
                if (i % 3 == 0)
                    yield return new Coroutine(Routine2());
                yield return new WaitUntil(() => Time.realtimeSinceStartup > 20);
            }
            executeCount++;
        }

        private IEnumerator Routine2()
        {
            yield return new Coroutine(Routine3());
            executeCount++;
        }

        private IEnumerator Routine3()
        {
            Debug.Log(executeCount);
            yield return null;
            executeCount++;
        }

        #endregion Misc

        private class Context
        {
            public string Name;

            public Func<IEnumerator> Method;

            public Action CustomGUI;

            public Coroutine coroutine;

            public event Action CoroutineDone;

            public Context(string name, Func<IEnumerator> method, Action customGUI = null)
            {
                Name = name;
                Method = method;
                CustomGUI = customGUI;
            }

            public void GUI()
            {
                EditorGUILayout.Space();

                EditorGUILayout.LabelField(Name, EditorStyles.boldLabel);

                using (new EditorGUI.IndentLevelScope(1))
                {
                    if (CustomGUI != null)
                        CustomGUI();

                    if (coroutine == null)
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            if (GUILayout.Button("Start", EditorStyles.miniButton))
                                coroutine = Coroutine.Start(Method(), OnCoroutineDone);

                            if (GUILayout.Button("Finish", EditorStyles.miniButton))
                            {
                                coroutine = new Coroutine(Method(), OnCoroutineDone);
                                coroutine.Finish();
                            }
                        }
                    }
                    else
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            if (GUILayout.Button("Stop", EditorStyles.miniButton))
                                coroutine.Stop();
                            if (GUILayout.Button("Finish", EditorStyles.miniButton))
                                coroutine.Finish();
                        }
                    }
                }
            }

            private void OnCoroutineDone()
            {
                Debug.LogFormat("<{0}> Done", coroutine.Name);

                coroutine = null;
                if (CoroutineDone != null)
                    CoroutineDone();
            }
        }
    }
}