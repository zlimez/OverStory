#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CorgiGodRays
{
    [CustomEditor(typeof(GodRaysRenderFeature))]
    public class GodRaysRenderFeatureEditor : Editor
    {
        private SerializedProperty renderData;
        private SerializedProperty Jitter;
        private SerializedProperty textureQuality;
        private SerializedProperty stepQuality;
        private SerializedProperty blur;
        private SerializedProperty BlurCount;
        private SerializedProperty blurSamples;
        private SerializedProperty depthAwareUpsampling;
        private SerializedProperty allowMainLight;
        private SerializedProperty allowAdditionalLights;
        private SerializedProperty useUnityDepthDirectly;
        private SerializedProperty supportUnityScreenSpaceShadows;
        private SerializedProperty renderAfterTransparents;
        private SerializedProperty useVariableIntensity;
        private SerializedProperty variableIntensityCurve;
        private SerializedProperty maxDistance;
        private SerializedProperty encodeLightColor;
        private SerializedProperty enableHighQualityTextures;

        private void OnEnable()
        {
            var settings = serializedObject.FindProperty("settings");
            renderData = settings.FindPropertyRelative("renderData");
            Jitter = settings.FindPropertyRelative("Jitter");
            textureQuality = settings.FindPropertyRelative("textureQuality");
            stepQuality = settings.FindPropertyRelative("stepQuality");
            blur = settings.FindPropertyRelative("blur");
            BlurCount = settings.FindPropertyRelative("BlurCount");
            blurSamples = settings.FindPropertyRelative("blurSamples");
            depthAwareUpsampling = settings.FindPropertyRelative("depthAwareUpsampling");
            allowMainLight = settings.FindPropertyRelative("allowMainLight");
            allowAdditionalLights = settings.FindPropertyRelative("allowAdditionalLights");
            useUnityDepthDirectly = settings.FindPropertyRelative("useUnityDepthDirectly");
            supportUnityScreenSpaceShadows = settings.FindPropertyRelative("supportUnityScreenSpaceShadows");
            renderAfterTransparents = settings.FindPropertyRelative("renderAfterTransparents");
            useVariableIntensity = settings.FindPropertyRelative("useVariableIntensity");
            variableIntensityCurve = settings.FindPropertyRelative("variableIntensityCurve");
            maxDistance = settings.FindPropertyRelative("maxDistance");
            encodeLightColor = settings.FindPropertyRelative("encodeLightColor");
            enableHighQualityTextures = settings.FindPropertyRelative("enableHighQualityTextures");
        }



        public override void OnInspectorGUI()
        {
            var instance = (GodRaysRenderFeature) target;

            EditorGUILayout.BeginVertical("GroupBox");
            {
                EditorGUILayout.LabelField("Quality Settings", EditorStyles.boldLabel); 
                EditorGUILayout.PropertyField(maxDistance);
                EditorGUILayout.PropertyField(encodeLightColor);
                EditorGUILayout.PropertyField(enableHighQualityTextures);
                EditorGUILayout.PropertyField(textureQuality);
                if (instance.settings.textureQuality != GodRaysRenderFeature.VolumeTextureQuality.High)
                { 
                    EditorGUILayout.PropertyField(depthAwareUpsampling);
                }
                EditorGUILayout.PropertyField(stepQuality);
                EditorGUILayout.PropertyField(blur);
                if(instance.settings.blur)
                {
                    EditorGUILayout.PropertyField(BlurCount);
                    EditorGUILayout.PropertyField(blurSamples);
                }
                EditorGUILayout.PropertyField(Jitter);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Feature Settings", EditorStyles.boldLabel); 
                EditorGUILayout.PropertyField(allowMainLight);
                EditorGUILayout.PropertyField(allowAdditionalLights);
                EditorGUILayout.PropertyField(useUnityDepthDirectly);
                EditorGUILayout.PropertyField(supportUnityScreenSpaceShadows);
                EditorGUILayout.PropertyField(renderAfterTransparents);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Misc Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(useVariableIntensity);
                if (instance.settings.useVariableIntensity)
                {
                    EditorGUILayout.PropertyField(variableIntensityCurve);
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Internal Data", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(renderData);

                if(!instance.settings.allowMainLight && !instance.settings.allowAdditionalLights)
                {
                    EditorGUILayout.HelpBox("Both allowMainLight and allowAdditionalLights are disabled, please enable one!", MessageType.Warning); 
                }
            }
            EditorGUILayout.EndVertical();

            if(GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(instance); 
            }
        }
    }
}
#endif