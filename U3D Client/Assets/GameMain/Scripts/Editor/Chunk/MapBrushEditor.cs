using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Cherry.Tilemaps;
using UnityEngine.Tilemaps;

namespace Cherry.Editor.Tilemaps
{
    /// <summary>
    /// 用于在Inspector面板显示Tile的详细信息
    /// </summary>
    [CustomEditor(typeof(MapBrush))]
    public class MapBrushEditor : UnityEditor.Tilemaps.GridBrushEditor
    {
		//TODO 研究GridBrushEditor
		private MapBrushEditor prefabBrush { get { return target as MapBrushEditor; } }

		public override void PaintPreview(GridLayout grid, GameObject brushTarget, Vector3Int position)
		{
			base.PaintPreview(grid, brushTarget, position);
		}

		public override void OnSelectionInspectorGUI()
		{
			base.OnSelectionInspectorGUI();
			MapBrush brush = target as MapBrush;
			if (brush.cells.Length == 1)
			{
				CherryTile tile = brush.cells[0].tile as CherryTile;
				if (EditorGUILayout.Toggle("IsNpc",tile.IsNpc))
				{
					EditorGUILayout.DelayedTextField("NpcName", tile.NpcName);
				}
				if (EditorGUILayout.Toggle("IsMonsterCreator", tile.IsMonsterCreator))
				{
					EditorGUILayout.DelayedTextField("MonsterName", tile.MonsterName);
				}
			}
		}

		public override void OnPaintInspectorGUI()
		{
			GUILayout.Label("https://github.com/CherryGameStudio/Tilemap2Unity");
		}

		public override void OnPaintSceneGUI(GridLayout grid, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, bool executing)
		{
			base.OnPaintSceneGUI(grid, brushTarget, position, tool, executing);
			Handles.Label(grid.CellToWorld(new Vector3Int(position.x, position.y, position.z)), new Vector3Int(position.x, position.y, position.z).ToString());
			if (ChunkEditor.isDrawLine)
			{
				//左下角，左上角，右下角，右上角
				Vector3 pos1 = new Vector3(TilemapHelper.TargetMapData.ChunkBeginPosX - 0.5f, TilemapHelper.TargetMapData.ChunkBeginPosY - 0.5f);
				Vector3 pos2 = new Vector3(TilemapHelper.TargetMapData.ChunkBeginPosX - 0.5f, TilemapHelper.TargetMapData.ChunkBeginPosY + TilemapHelper.TargetMapData.ChunkHeight - 0.5f);
				Vector3 pos3 = new Vector3(TilemapHelper.TargetMapData.ChunkBeginPosX + TilemapHelper.TargetMapData.ChunkWidth - 0.5f, TilemapHelper.TargetMapData.ChunkBeginPosY - 0.5f);
				Vector3 pos4 = new Vector3(TilemapHelper.TargetMapData.ChunkBeginPosX + TilemapHelper.TargetMapData.ChunkWidth - 0.5f, TilemapHelper.TargetMapData.ChunkBeginPosY + TilemapHelper.TargetMapData.ChunkHeight - 0.5f);
				Debug.DrawLine(pos1, pos2, Color.green);
				Debug.DrawLine(pos1, pos3, Color.green);
				Debug.DrawLine(pos2, pos4, Color.red);
				Debug.DrawLine(pos3, pos4, Color.red);
				Handles.Label(pos1, new Vector3Int((int)TilemapHelper.TargetMapData.ChunkBeginPosX, (int)TilemapHelper.TargetMapData.ChunkBeginPosY, 0).ToString());
				Handles.Label(pos2, new Vector3Int((int)TilemapHelper.TargetMapData.ChunkBeginPosX, (int)(TilemapHelper.TargetMapData.ChunkBeginPosY + TilemapHelper.TargetMapData.ChunkHeight - 1), 0).ToString());
				Handles.Label(pos3, new Vector3Int((int)(TilemapHelper.TargetMapData.ChunkBeginPosX + TilemapHelper.TargetMapData.ChunkWidth - 1), (int)TilemapHelper.TargetMapData.ChunkBeginPosY, 0).ToString());
				Handles.Label(pos4, new Vector3Int((int)(TilemapHelper.TargetMapData.ChunkBeginPosX + TilemapHelper.TargetMapData.ChunkWidth - 1), (int)(TilemapHelper.TargetMapData.ChunkBeginPosY + TilemapHelper.TargetMapData.ChunkHeight - 1), 0).ToString());
			}
		}
	}
}
