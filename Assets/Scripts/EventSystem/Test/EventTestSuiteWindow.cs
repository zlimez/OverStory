using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace Chronellium.EventSystem
{
    public class EventTestSuiteWindow : EditorWindow
    {
        private List<bool> _selectedStaticEvents;
        private string _customEventName = "";
        private Vector2 _scrollPosition;
        private string _searchString = "";
        private List<string> _customGameEvents = new List<string>();
        private string _newCustomGameEvent = "";


        [MenuItem("Window/Event Test Suite")]
        public static void ShowWindow()
        {
            GetWindow<EventTestSuiteWindow>("Event Test Suite");
        }

        private void OnEnable()
        {
            _selectedStaticEvents = new List<bool>(new bool[System.Enum.GetNames(typeof(StaticEvent)).Length]);
        }

        private void OnGUI()
        {
            GUILayout.Label("Select Static Events:");

            // Add a search box
            GUILayout.BeginHorizontal();
            GUILayout.Label("Search:", GUILayout.ExpandWidth(false));
            _searchString = GUILayout.TextField(_searchString, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();

            // Add scroll view for the list of StaticEvents
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            for (int i = 0; i < _selectedStaticEvents.Count; i++)
            {
                StaticEvent staticEvent = (StaticEvent)i;
                string eventName = staticEvent.ToString();

                // Show only events matching the search string
                if (string.IsNullOrEmpty(_searchString) || eventName.IndexOf(_searchString, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    _selectedStaticEvents[i] = GUILayout.Toggle(_selectedStaticEvents[i], eventName);
                }
            }
            GUILayout.EndScrollView();

            EditorGUILayout.LabelField("Custom Game Events:", EditorStyles.boldLabel);

            // Add input field for a new custom GameEvent string
            GUILayout.BeginHorizontal();
            GUILayout.Label("New event:", GUILayout.ExpandWidth(false));
            _newCustomGameEvent = GUILayout.TextField(_newCustomGameEvent, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();

            // Button to add the new custom GameEvent string
            if (GUILayout.Button("Add custom event"))
            {
                if (!string.IsNullOrEmpty(_newCustomGameEvent))
                {
                    _customGameEvents.Add(_newCustomGameEvent);
                    _newCustomGameEvent = "";
                }
            }

            // Display custom GameEvent strings and remove buttons
            for (int i = 0; i < _customGameEvents.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(_customGameEvents[i], GUILayout.ExpandWidth(true));

                if (GUILayout.Button("Remove", GUILayout.ExpandWidth(false)))
                {
                    _customGameEvents.RemoveAt(i);
                    i--;
                }

                GUILayout.EndHorizontal();
            }
        }
    }
}
#endif