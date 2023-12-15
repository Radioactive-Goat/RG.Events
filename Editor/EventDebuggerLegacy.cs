using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RG.Events
{
    public class EventDebuggerLegacy : EditorWindow
    {
        private Vector2 _mainHorizontalScrollPosition;
        private Vector2 _invokeStackScrollPosition;
        private Type _currentSelection = null;

        [MenuItem("Radioactive Goat/Event System/Event Debugger (Legacy)")]
        static void Init()
        {
            var window = (EventDebuggerLegacy)GetWindow(typeof(EventDebuggerLegacy));
            window.titleContent = new GUIContent("Event Debugger (Legacy)");
            window.Show();
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        private void OnGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Editor not in Play Mode!", MessageType.Error);
                return;
            }

            if(EventSystem.Instance == null)
            {
                EditorGUILayout.HelpBox("Event System not running!", MessageType.Error);
                return;
            }

            _mainHorizontalScrollPosition = EditorGUILayout.BeginScrollView(_mainHorizontalScrollPosition);
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(); //Event List
            EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            foreach(var item in EventSystem.Instance.Events.ToList())
            {
                if (GUILayout.Button(new GUIContent(item.Key.Name)))
                {
                    _currentSelection = item.Key;
                }
            }
            EditorGUILayout.EndVertical(); //Event List

            EditorGUILayout.BeginVertical(); //Callback List
            EditorGUILayout.LabelField("Callback List", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            if (_currentSelection != null)
            {
                foreach (var callback in EventSystem.Instance.Events[_currentSelection].Callbacks)
                {
                    EditorGUILayout.LabelField(callback);
                }
            }
            EditorGUILayout.EndVertical(); //Calback List

            EditorGUILayout.BeginVertical(); //Invoke Stack
            EditorGUILayout.LabelField("Invoke Stack", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Buffer Size");
            EventSystem.Instance.invokeStackBufferSize = (uint)EditorGUILayout.IntField((int)EventSystem.Instance.invokeStackBufferSize);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            _invokeStackScrollPosition = EditorGUILayout.BeginScrollView(_invokeStackScrollPosition);
            EditorGUILayout.BeginVertical();
            foreach(var invoke in EventSystem.Instance.invokeStack) 
            {
                EditorGUILayout.LabelField(invoke.EventName, EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginVertical();
                foreach (var field in invoke.ArgumentData.GetType().GetFields())
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(field.Name);
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(field.GetValue(invoke.ArgumentData).ToString());
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
                EditorGUI.indentLevel--;
                EditorGUILayout.Separator();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical(); //Invoke Stack

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }


    }
}
