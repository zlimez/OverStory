using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.IO;
using Tuples;
using Abyss.SceneSystem;
using System.Linq;

#if UNITY_EDITOR

namespace Abyss.EventSystem
{
    [CustomEditor(typeof(EventTestSuite))]
    public class EventTestSuiteEditor : Editor
    {
        private List<bool> _selectedStaticEvents;
        private Dictionary<StaticEvent, int> _staticEventIndices;
        private Vector2 _scrollPosition; private string _staticEventSearchString = "";
        private List<string> _customGameEvents = new List<string>();
        private string _newCustomGameEvent = "";
        private string _dynamicEventSearchString = "";
        private string _eventSetName = "";
        Vector2 _dynamicEventScrollPosition = Vector2.zero;
        private string _parentFilePath = "Assets/ScriptableObjects";
        private string _filePath = "Assets/ScriptableObjects/EventSets";

        private void OnEnable()
        {
            // int staticEventCount = Enum.GetNames(typeof(StaticEvent)).Length;
            // _selectedStaticEvents = new List<bool>(new bool[staticEventCount]);
            var staticEvents = Enum.GetValues(typeof(StaticEvent)).Cast<StaticEvent>().ToArray();
            _selectedStaticEvents = new List<bool>(new bool[staticEvents.Length]);

            _staticEventIndices = new Dictionary<StaticEvent, int>();
            for (int i = 0; i < staticEvents.Length; i++)
            {
                _staticEventIndices[staticEvents[i]] = i;
            }
        }

        public override void OnInspectorGUI()
        {
            EventTestSuite eventTestSuiteObject = (EventTestSuite)target;

            DrawSection("Select Static Events:", ref _staticEventSearchString, ref _scrollPosition, DrawStaticEvents);
            DrawSection("Dynamic Events:", ref _dynamicEventSearchString, ref _dynamicEventScrollPosition, DrawDynamicEvents);
            DrawCustomGameEventsSection();

            if (GUILayout.Button("Reload Current Scene"))
            {
                SceneLoader.Instance.ReloadCurrentSceneWithMaster();
            }

            DrawSaveLoadEventSetsSection();
        }

        private void DrawSaveLoadEventSetsSection()
        {
            EditorGUILayout.LabelField("Save/Load Event Sets:", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Event Set Name:", GUILayout.ExpandWidth(false));
            _eventSetName = GUILayout.TextField(_eventSetName, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save Event Set"))
            {
                SaveEventSet(_eventSetName);
            }

            if (GUILayout.Button("Load Event Set"))
            {
                LoadEventSet(_eventSetName);
            }
            GUILayout.EndHorizontal();

            DrawStoredEventSets();
        }

        private void SaveEventSet(string eventSetName)
        {
            if (string.IsNullOrEmpty(eventSetName))
            {
                EditorUtility.DisplayDialog("Error", "Event set name cannot be empty.", "OK");
                return;
            }

            string eventSetPath = $"{this._filePath}/{eventSetName}.asset";
            if (File.Exists(eventSetPath))
            {
                if (!EditorUtility.DisplayDialog("Warning", $"An event set named \"{eventSetName}\" already exists. Do you want to overwrite it?", "Yes", "No"))
                {
                    return;
                }
            }

            EventSetAsset newEventSet = ScriptableObject.CreateInstance<EventSetAsset>();

            Debug.Log("Saving past events:");
            Debug.Log(newEventSet.LoopedPastEvents);
            Debug.Log(newEventSet.NormalPastEvents);

            if (!AssetDatabase.IsValidFolder($"{this._filePath}"))
            {
                AssetDatabase.CreateFolder($"{this._parentFilePath}", "EventSets");
            }

            AssetDatabase.CreateAsset(newEventSet, eventSetPath);
            AssetDatabase.SaveAssets();
        }

        private void LoadEventSet(string eventSetName)
        {
            string eventSetPath = $"{this._filePath}/{eventSetName}.asset";
            EventSetAsset eventSet = AssetDatabase.LoadAssetAtPath<EventSetAsset>(eventSetPath);

            if (eventSet != null)
            {
                Debug.Log("Loading past events:");
                Debug.Log(eventSet.LoopedPastEvents);
                Debug.Log(eventSet.NormalPastEvents);

                Dictionary<GameEvent, int> loopedPastEvents = new Dictionary<GameEvent, int>();
                foreach (var entry in eventSet.LoopedPastEvents)
                {
                    loopedPastEvents.Add(entry.Head, entry.Tail);
                }

                Dictionary<GameEvent, int> normalPastEvents = new Dictionary<GameEvent, int>();
                foreach (var entry in eventSet.NormalPastEvents)
                {
                    normalPastEvents.Add(entry.Head, entry.Tail);
                }

            }
            else
            {
                EditorUtility.DisplayDialog("Error", $"Event set \"{eventSetName}\" not found.", "OK");
            }
        }

        private void DrawStoredEventSets()
        {
            EditorGUILayout.LabelField("Stored Event Sets:", EditorStyles.boldLabel);
            string[] eventSetGUIDs = AssetDatabase.FindAssets("t:EventSetAsset", new[] { $"{this._filePath}" });

            foreach (string eventSetGUID in eventSetGUIDs)
            {
                string eventSetPath = AssetDatabase.GUIDToAssetPath(eventSetGUID);
                EventSetAsset eventSet = AssetDatabase.LoadAssetAtPath<EventSetAsset>(eventSetPath);

                GUILayout.BeginHorizontal();
                GUILayout.Label(eventSet.name, GUILayout.ExpandWidth(true));

                if (GUILayout.Button("Load", GUILayout.ExpandWidth(false)))
                {
                    LoadEventSet(eventSet.name);
                }

                GUILayout.EndHorizontal();
            }
        }

        private void DrawSection(string title, ref string searchString, ref Vector2 scrollPosition, Action<string> drawContentAction)
        {
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

            searchString = DrawSearchBox(searchString);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(EditorGUIUtility.currentViewWidth - 30), GUILayout.MaxHeight(200));
            drawContentAction(searchString);
            GUILayout.EndScrollView();
        }

        private void DrawSection(string title, Dictionary<GameEvent, int> eventDictionary)
        {
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

            if (eventDictionary == null)
            {
                EditorGUILayout.LabelField("No events to display.");
                return;
            }

            foreach (var gameEventEntry in eventDictionary)
            {
                EditorGUILayout.LabelField($"{gameEventEntry.Key}: {gameEventEntry.Value}");
            }
        }

        private string DrawSearchBox(string searchString)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Search:", GUILayout.ExpandWidth(false));
            searchString = GUILayout.TextField(searchString, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();
            return searchString;
        }

        private void DrawStaticEvents(string searchString)
        {
            foreach (StaticEvent staticEvent in _staticEventIndices.Keys)
            {
                int index = _staticEventIndices[staticEvent];
                string eventName = staticEvent.ToString();

                if (FilterEvents(eventName, searchString))
                {
                    _selectedStaticEvents[index] = GUILayout.Toggle(_selectedStaticEvents[index], eventName);
                }
            }
        }


        private void DrawDynamicEvents(string searchString)
        {
            FieldInfo[] dynamicEventFields = typeof(DynamicEvent).GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (FieldInfo field in dynamicEventFields)
            {
                GameEvent dynamicEvent = (GameEvent)field.GetValue(null);
                string dynamicEventName = dynamicEvent.EventName;

                if (FilterEvents(dynamicEventName, searchString))
                {
                    if (GUILayout.Button(dynamicEventName, EditorStyles.label))
                    {
                        _newCustomGameEvent = dynamicEventName;
                    }
                }
            }
        }

        private void DrawCustomGameEventsSection()
        {
            EditorGUILayout.LabelField("Custom Game Events:", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            GUILayout.Label("New event:", GUILayout.ExpandWidth(false));
            _newCustomGameEvent = GUILayout.TextField(_newCustomGameEvent, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Add custom event"))
            {
                if (!string.IsNullOrEmpty(_newCustomGameEvent))
                {
                    _customGameEvents.Add(_newCustomGameEvent);
                    _newCustomGameEvent = "";
                }
            }

            DrawCustomGameEvents();
        }

        private void DrawCustomGameEvents()
        {
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

        private bool FilterEvents(string eventName, string searchString)
        {
            return string.IsNullOrEmpty(searchString) || eventName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}

#endif