using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using Cherry.Tilemaps;
using UnityEditor.Tilemaps;
using System;

namespace Cherry.Editor.Tilemaps
{
	[CreateAssetMenu]
	[CustomGridBrush(false, true, true, "Map Brush")]
	public class MapBrush : GridBrush
	{
		public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
		{
			if (cells.Length > 1)
			{
				EditorUtility.DisplayDialog("Warning", "The operation is invaild,You can not select more than one cell.", "OK");
				return;
			}
			base.Paint(gridLayout, brushTarget, position);
			AddTileMapData(gridLayout, brushTarget, position);
		}

		public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
		{
			if (cells.Length > 1)
			{
				EditorUtility.DisplayDialog("Warning", "The operation is invaild,You can not select more than one cell.", "OK");
				return;
			}
			base.Erase(gridLayout, brushTarget, position);
			ClearTileMapData(gridLayout, brushTarget, position);
		}

		public override void FloodFill(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
		{
			EditorUtility.DisplayDialog("Warning", "The operation is invaild.", "OK");
			return;
		}

		public override void BoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
		{
			if (cells.Length > 1)
			{
				EditorUtility.DisplayDialog("Warning", "The operation is invaild,You can not select more than one cell.", "OK");
				return;
			}
			base.BoxFill(gridLayout, brushTarget, position);
		}

		public override void BoxErase(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
		{
			if (cells.Length > 1)
			{
				EditorUtility.DisplayDialog("Warning", "The operation is invaild,You can not select more than one cell.", "OK");
				return;
			}
			base.BoxErase(gridLayout, brushTarget, position);
		}

		public override void Pick(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, Vector3Int pickStart)
		{
			if (cells.Length > 1)
			{
				EditorUtility.DisplayDialog("Warning", "The operation is invaild,You can not select more than one cell.", "OK");
			}
			base.Pick(gridLayout, brushTarget, position, pickStart);
		}

		/// <summary>
		/// 添加地图数据
		/// </summary>
		/// <param name="grid"></param>
		/// <param name="position"></param>
		private void AddTileMapData(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
		{
			BrushCell cell = cells[0];

			//TODO 对各类Tile进行坐标适配
			TileInfo data = new TileInfo
			{
				//tile的中心点为四个顶点的其中一个点，默认左下角，我们偏移一下保证和其他游戏对象的中心点一致,这里是还原创建Grid时的偏移，保证对象刚好在tile的中心点
				Pos = gridLayout.CellToWorld(position) + TilemapHelper.tileOffset3,
				IntPos = position
			};
			for (int i = 0; i < cells.Length; i++)
			{
				CherryTile xmtile = (CherryTile)cells[i].tile;
				data.Tile = xmtile;
			}
			TilemapHelper.AddData(brushTarget, data.Pos, data);
		}

		/// <summary>
		/// 清除地图数据
		/// </summary>
		/// <param name="position"></param>
		private void ClearTileMapData(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
		{
			Vector3 pos = gridLayout.CellToWorld(position) + TilemapHelper.tileOffset3;
			TilemapHelper.ClearData(brushTarget, pos);
		}
	}
}
