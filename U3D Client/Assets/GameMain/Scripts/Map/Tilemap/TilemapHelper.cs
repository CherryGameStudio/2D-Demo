using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.IO;

#if UNITY_EDITOR
namespace Cherry.Tilemaps
{
	public static class TilemapHelper
	{
		#region new
		private static string TilemapSourceDataPath = "Assets/GameMain/Scripts/Editor/Chunk/SourceTilemapData.asset";
		private static string TilemapTargetDataPath = "Assets/GameMain/Scripts/Editor/Chunk/TargetTilemapData.asset";

		private static TilemapData m_SourceData;
		public static TilemapData SourceMapData
		{
			get
			{
				if (m_SourceData == null)
				{
					m_SourceData = AssetDatabase.LoadAssetAtPath<TilemapData>(TilemapSourceDataPath);
				}

				return m_SourceData;
			}
		}

		private static TilemapData m_TargetData;
		public static TilemapData TargetMapData
		{
			get
			{
				if (m_TargetData == null)
				{
					m_TargetData = AssetDatabase.LoadAssetAtPath<TilemapData>(TilemapTargetDataPath);
				}

				return m_TargetData;
			}

			set
			{
				m_TargetData = value;
			}
		}

		/// <summary>
		/// 添加地图数据。
		/// </summary>
		/// <param name="brushTarget">笔刷</param>
		/// <param name="pos">世界坐标</param>
		/// <param name="data">单位数据</param>
		public static void AddData(GameObject brushTarget, Vector3 pos, TileInfo data)
		{
			Tilemap tilemap = brushTarget.GetComponent<Tilemap>();
			if (tilemap.name != TargetMapData.TilemapName)
			{
				return;
			}

			bool isadd = true;
			for (int i = 0; i < TargetMapData.TileInfoList.Count; i++)
			{
				//如果在相同位置，则只修改TileInfo
				if (TargetMapData.TileInfoList[i].Pos == pos)
				{
					isadd = false;
					TargetMapData.TileInfoList[i] = data;
					break;
				}
			}

			if (isadd)
			{
				TargetMapData.TileInfoList.Add(data);
			}
		}

		/// <summary>
		/// 清除地图数据。
		/// </summary>
		/// <param name="brushTarget">笔刷</param>
		/// <param name="pos">世界坐标</param>
		public static void ClearData(GameObject brushTarget, Vector3 pos)
		{
			Tilemap tilemap = brushTarget.GetComponent<Tilemap>();
			if (tilemap.name != TargetMapData.TilemapName)
			{
				return;
			}

			for (int i = 0; i < TargetMapData.TileInfoList.Count; i++)
			{
				if (TargetMapData.TileInfoList[i].Pos == pos)
				{
					TargetMapData.TileInfoList.RemoveAt(i);
				}
			}
		}

		public static void SaveData(TilemapData targetData,TilemapData sourceData)
		{
			if (targetData == null)
			{
				targetData = new TilemapData();
				targetData.TileInfoList = new List<TileInfo>();
			}

			targetData.OrderInLayer = sourceData.OrderInLayer;
			targetData.SortOrderIndex = sourceData.SortOrderIndex;
			targetData.TilemapName = sourceData.TilemapName;
			targetData.SortingLayerIndex = sourceData.SortingLayerIndex;
			foreach (var item in sourceData.TileInfoList)
			{
				targetData.TileInfoList.Add(item);
			}

			EditorUtility.SetDirty(targetData);
		}

		public static void SaveTilemapData2Json(string path)
		{
			TilemapData data;
			path = path + "/" + TilemapHelper.TargetMapData.TilemapName;
			if (File.Exists(path))
			{
				data = AssetDatabase.LoadAssetAtPath<TilemapData>(path);
				data.TilemapName = TilemapHelper.TargetMapData.TilemapName;
				data.SortingLayerIndex = TilemapHelper.TargetMapData.SortingLayerIndex;
				data.SortOrderIndex = TilemapHelper.TargetMapData.SortOrderIndex;
				data.OrderInLayer = TilemapHelper.TargetMapData.OrderInLayer;
				data.ChunkBeginPosX = TilemapHelper.TargetMapData.ChunkBeginPosX;
				data.ChunkBeginPosY = TilemapHelper.TargetMapData.ChunkBeginPosY;
				data.ChunkHeight = TilemapHelper.TargetMapData.ChunkHeight;
				data.ChunkWidth = TilemapHelper.TargetMapData.ChunkWidth;
				data.ScriptName = TilemapHelper.TargetMapData.ScriptName;
				data.TileInfoList.Clear();
				foreach (var item in TilemapHelper.TargetMapData.TileInfoList)
				{
					data.TileInfoList.Add(item);
				}
			}
			else
			{
				data = ScriptableObject.CreateInstance<TilemapData>();
				data.TilemapName = TilemapHelper.TargetMapData.TilemapName;
				data.SortingLayerIndex = TilemapHelper.TargetMapData.SortingLayerIndex;
				data.SortOrderIndex = TilemapHelper.TargetMapData.SortOrderIndex;
				data.OrderInLayer = TilemapHelper.TargetMapData.OrderInLayer;
				data.ChunkBeginPosX = TilemapHelper.TargetMapData.ChunkBeginPosX;
				data.ChunkBeginPosY = TilemapHelper.TargetMapData.ChunkBeginPosY;
				data.ChunkHeight = TilemapHelper.TargetMapData.ChunkHeight;
				data.ChunkWidth = TilemapHelper.TargetMapData.ChunkWidth;
				data.ScriptName = TilemapHelper.TargetMapData.ScriptName;
				data.TileInfoList.Clear();
				foreach (var item in TilemapHelper.TargetMapData.TileInfoList)
				{
					data.TileInfoList.Add(item);
				}
			}

			try
			{
				AssetDatabase.CreateAsset(data, path + ".asset");
			}
			catch (Exception e)
			{
				throw new Exception(string.Format("保存TilemapData为Json文件失败，路径为{0}，message：{1}", path, e));
			}

			Debug.Log("保存TilemapData文件成功，文件路径为：" + path);
		}

		public static void LoadTilemapDataFromJson(TilemapData data)
		{
			if (data == null)
			{
				GUILayout.Box("加载TilemapData文件失败");
				return;
			}

			TargetMapData.OrderInLayer = data.OrderInLayer;
			TargetMapData.SortOrderIndex = data.SortOrderIndex;
			TargetMapData.OrderInLayer = data.OrderInLayer;
			TargetMapData.TilemapName = data.TilemapName;
			TargetMapData.ChunkBeginPosX = data.ChunkBeginPosX;
			TargetMapData.ChunkBeginPosY = data.ChunkBeginPosY;
			TargetMapData.ChunkHeight = data.ChunkHeight;
			TargetMapData.ChunkWidth = data.ChunkWidth;
			TargetMapData.ScriptName = data.ScriptName;
			TargetMapData.TileInfoList.Clear();
			foreach (var item in data.TileInfoList)
			{
				TargetMapData.TileInfoList.Add(item);
			}

			Debug.Log("读取TilemapData文件成功");
		}
		#endregion

		/// <summary>
		/// 地图ID
		/// </summary>
		public static int MapID = 0;

		public static Vector2 tileOffset2 = new Vector2(0.5f, 0.5f);
		public static Vector3 tileOffset3 = new Vector3(0.5f, 0.5f, 0);

#if UNITY_EDITOR
		/// <summary>
		/// 载入地图数据
		/// </summary>
		/// <param name="path"></param>
		private static void LoadData(string path)
		{

		}

		/// <summary>
		/// 设置Tile
		/// </summary>
		/// <param name="map"></param>
		/// <param name="pos"></param>
		/// <param name="tilebase"></param>
		public static void SetTile(Tilemap map, Vector3Int pos, TileBase tilebase)
		{
			map.SetTile(pos, tilebase);
		}

		/// <summary>
		/// 设置TileMap
		/// </summary>
		/// <param name="map"></param>
		/// <param name="tileMapDataList"></param>
		public static void SetTileMap(Tilemap map, TilemapData tileMapData)
		{
			if (tileMapData.TileInfoList != null)
			{
				foreach (var tile in tileMapData.TileInfoList)
				{
					map.SetTile(tile.IntPos, tile.Tile);
				}
			}
		}

		///// <summary>
		///// 初始化地图,绑定寻路数据
		///// </summary>
		//public static void InitMap()
		//{
		//	//Debug.Log(XMMapData.mapSize);
		//	TilemapManager = new Dictionary<Vector2, Point>();
		//	List<TileMapData> tilemapData = TilemapManager.MapData.Data[TilemapManager.MapID].tileMapDataList;
		//	foreach (var item in tilemapData)
		//	{
		//		for (int i = 0; i < item.tileInfoList.Count; i++)
		//		{
		//			int x = item.tileInfoList[i].ipos.x;
		//			int y = item.tileInfoList[i].ipos.y;
		//			//Debug.Log(x + " " + y);
		//			XMMapData.map.Add(new Vector2(x, y), new Point(x, y));
		//			bool walkable = ((XMTile)item.tileInfoList[i].tile).walkable;
		//			if (walkable)
		//			{
		//				XMMapData.map[new Vector2(x, y)].Walkable = walkable;
		//			}
		//		}
		//	}
		//}




		public static void ClearDataForPos(Vector3 pos)
		{

		}

		/// <summary>
		/// 清空数据
		/// </summary>
		public static void ClearAllData()
		{

		}
#endif
	}
}
#endif