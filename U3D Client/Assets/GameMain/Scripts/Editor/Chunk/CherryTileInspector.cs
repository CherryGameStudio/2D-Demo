using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Cherry.Tilemaps;

namespace Cherry.Editor.Tilemaps
{
	[CustomEditor(typeof(CherryTile),true)]
	public class CherryTileInspector : UnityEditor.Editor
	{
		private SerializedProperty m_IsNpc;
		private SerializedProperty m_NpcName;
		private SerializedProperty m_IsMonsterCreator;
		private SerializedProperty m_MonsterName;

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			serializedObject.Update();

			CherryTile tileTarget = (CherryTile)target;

			bool isNpc = EditorGUILayout.Toggle("IsNpc", m_IsNpc.boolValue);
			tileTarget.IsNpc = isNpc;
			if (isNpc)
			{
				string npcName = EditorGUILayout.DelayedTextField("NpcName:", m_NpcName.stringValue);
				tileTarget.NpcName = npcName;
			}

			bool isMonsterCreator = EditorGUILayout.Toggle("IsMonsterCreator", m_IsMonsterCreator.boolValue);
			tileTarget.IsMonsterCreator = isMonsterCreator;
			if (isMonsterCreator)
			{
				string monsterName = EditorGUILayout.DelayedTextField("MonsterName", m_MonsterName.stringValue);
				tileTarget.MonsterName = monsterName;
			}

		}

		private void OnEnable()
		{
			m_IsNpc = serializedObject.FindProperty("IsNpc");
			m_NpcName = serializedObject.FindProperty("NpcName");
			m_IsMonsterCreator = serializedObject.FindProperty("IsMonsterCreator");
			m_MonsterName = serializedObject.FindProperty("MonsterName");

			serializedObject.ApplyModifiedProperties();
		}
	}
}
