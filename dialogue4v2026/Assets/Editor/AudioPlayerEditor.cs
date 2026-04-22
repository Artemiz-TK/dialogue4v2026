using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(AudioPlayer))]
    public class AudioPlayerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            AudioPlayer player = (AudioPlayer)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Runtime Controls (only work in Play Mode)", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Play"))
            {
                player.Play();
            }

            if (GUILayout.Button("Pause"))
            {
                player.Pause();
            }

            if (GUILayout.Button("Resume"))
            {
                player.Resume();
            }

            if (GUILayout.Button("Stop"))
            {
                player.Stop();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Selected Index (applies to AudioManager when Play is pressed)");
            int newIndex = EditorGUILayout.IntField("Clip Index", player.clipIndex);
            if (newIndex != player.clipIndex)
            {
                Undo.RecordObject(player, "Change Clip Index");
                player.clipIndex = Mathf.Max(0, newIndex);
                EditorUtility.SetDirty(player);
            }

            if (GUILayout.Button("Apply Index to AudioManager (Play Mode Only)"))
            {
                player.ApplyIndexToManager();
            }
        }
    }
}
