using UnityEditor;
using UnityEngine;

namespace Util.TextTween {
    [CustomEditor(typeof(TMP_TextTween), true)]
    public sealed class TMP_TextTweenInspector : Editor {
        private TMP_TextTween textTween;
        private SerializedProperty textComponentSerializedProperty;
        private SerializedProperty animationControlledSerializedProperty;
        private SerializedProperty playWhenReadySerializedProperty;
        private SerializedProperty loopSerializedProperty;
        private SerializedProperty playForeverSerializedProperty;
        private SerializedProperty durationSerializedProperty;
        private SerializedProperty delaySerializedProperty;
        private SerializedProperty progressSerializedProperty;

        private void OnEnable() {
            textTween = (TMP_TextTween) target;
            textComponentSerializedProperty = serializedObject.FindProperty("tmpText");
            durationSerializedProperty = serializedObject.FindProperty("duration");
            delaySerializedProperty = serializedObject.FindProperty("delay");
            progressSerializedProperty = serializedObject.FindProperty("progress");
            playWhenReadySerializedProperty = serializedObject.FindProperty("playWhenReady");
            loopSerializedProperty = serializedObject.FindProperty("loop");
            playForeverSerializedProperty = serializedObject.FindProperty("playForever");
            animationControlledSerializedProperty = serializedObject.FindProperty("animationControlled");
        }

        public override void OnInspectorGUI() {
            EditorGUILayout.PropertyField(textComponentSerializedProperty);
            EditorGUILayout.PropertyField(durationSerializedProperty);
            EditorGUILayout.PropertyField(delaySerializedProperty);
            EditorGUILayout.PropertyField(animationControlledSerializedProperty);
            if (animationControlledSerializedProperty.boolValue) {
                EditorGUILayout.PropertyField(progressSerializedProperty);
            }
            else {
                EditorGUILayout.PropertyField(playWhenReadySerializedProperty);
                EditorGUILayout.PropertyField(loopSerializedProperty);
                EditorGUILayout.PropertyField(playForeverSerializedProperty);
            }

            serializedObject.ApplyModifiedProperties();
            
            if (animationControlledSerializedProperty.boolValue) return;
            if (!Application.isPlaying) return;

            if (!textTween.IsPlaying) {
                if (GUILayout.Button("Play")) {
                    textTween.Play();
                }
            }
            else {
                if (GUILayout.Button("Stop")) {
                    textTween.Stop();
                }
            }
        }
    }
}