using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Cherry.Editor
{
	[CustomEditor(typeof(MapComponent))]
	public class MapComponentInspector : UnityEditor.Editor
	{
		private SerializedProperty m_ChunkHelperTypeName = null;
		private SerializedProperty m_ChunkGroups = null;

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.BeginVertical();
			EditorGUILayout.PropertyField(m_ChunkHelperTypeName);
			EditorGUILayout.PropertyField(m_ChunkGroups, true);
			EditorGUILayout.EndVertical();

			serializedObject.ApplyModifiedProperties();

			Repaint();
		}

		private void OnEnable()
		{
			m_ChunkHelperTypeName = serializedObject.FindProperty("m_ChunkHelperTypeName");
			m_ChunkGroups = serializedObject.FindProperty("m_ChunkGroups");
		}
	}
}
