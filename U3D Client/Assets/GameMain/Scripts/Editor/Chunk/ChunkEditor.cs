using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Cherry.Tilemaps;
using UnityEngine.Tilemaps;
using System.IO;
using System.Text;
using System.Reflection;
using System;

namespace Cherry.Editor.Tilemaps
{
	/// <summary>
	/// 地图编辑器。
	/// </summary>
	public class ChunkEditor : EditorWindow
	{
		#region new
		private readonly string MapFolderPath = "Assets/GameMain/AssetData/Map";

		public int toolbarOption = 0;
		public string[] toolbarTexts = { "编 辑", "设 置", "关 于" };

		DirectoryInfo[] directoryInfos;
		private int directoryToolbarIndex = 0;
		int[] directoryIntPopupSize;
		private string[] directoryToolbarTexts;

		private int fileToolbarIndex = 0;
		int[] fileIntPopupSize;
		private string[] fileToolbarTexts;

		//是否画线
		public static bool isDrawLine = false;

		//搜索栏
		private string searchText;
		private int searchCount;
		private StringBuilder m_StringBuilder = new StringBuilder(1024);

		int selectedIndex = 0;
		#endregion


		Vector2 scrollPos;
		Vector3 delVecPos;


		#region ShowTileMapEditor
		[MenuItem("CherryTool/地图编辑器")]
		public static void ShowTilemapEditor()
		{
			if (EditorSceneManager.GetActiveScene().name == "TilemapEditScene")
			{
				ChunkEditor window = (ChunkEditor)GetWindow(typeof(ChunkEditor));
				window.titleContent = new GUIContent("地图编辑器");
				window.Show();

				Type type = Assembly.Load("Unity.2D.Tilemap.Editor").GetType("UnityEditor.Tilemaps.GridPaintPaletteWindow");
				EditorWindow tileWindow = (EditorWindow)ScriptableObject.CreateInstance(type);
				tileWindow.titleContent = new GUIContent("Tile Palette");
				type.GetMethod("Show", Type.EmptyTypes).Invoke(tileWindow, null);
			}
			else
			{
				if (EditorUtility.DisplayDialog("提示","打开编辑器需要跳转到TilemapEditScene场景，点击确认将会自动保存当前场景，点击取消则继续提留在当前场景","确定残忍离开~","取消离开选项~"))
				{
					EditorSceneManager.SaveOpenScenes();
					EditorSceneManager.OpenScene("Assets/GameMain/Scenes/TilemapEditScene.unity");

					ChunkEditor window = (ChunkEditor)GetWindow(typeof(ChunkEditor));
					window.titleContent = new GUIContent("地图编辑器");
					window.Show();

					Type type = Assembly.Load("Unity.2D.Tilemap.Editor").GetType("UnityEditor.Tilemaps.GridPaintPaletteWindow");
					EditorWindow tileWindow = (EditorWindow)ScriptableObject.CreateInstance(type);
					tileWindow.titleContent = new GUIContent("嘿嘿");
					type.GetMethod("Show", Type.EmptyTypes).Invoke(tileWindow, null);
				}
			}
		}
		#endregion

		#region OnGUI
		private void OnGUI()
		{
			EditorGUILayout.Space();
			MainGUI();
			GUILayout.Space(5);
			GUI.backgroundColor = Color.gray;
			toolbarOption = GUILayout.Toolbar(toolbarOption, toolbarTexts, GUILayout.Height(30));
			switch (toolbarOption)
			{
				case 0:
					Title("Editor");
					TilemapContent();
					break;
				case 1:
					Title("Setting");
					SettingContent();
					break;
				case 2:
					Title("About");
					AboutContent();
					break;
				default:
					break;
			}
		}

		private void Title(string operationName)
		{
			GUILayout.Label(operationName, EditorStyles.boldLabel);
			EditorGUILayout.Space();
			GUI.backgroundColor = Color.white;
		}
		#endregion

		#region MainGUI
		private void MainGUI()
		{
			GUILayout.BeginVertical();

			DirectoryInfo directoryInfo = new DirectoryInfo(MapFolderPath);
			directoryInfos = directoryInfo.GetDirectories();
			//地图文件夹目录
			EditorGUIUtility.labelWidth = 70;
			directoryToolbarTexts = new string[directoryInfos.Length];
			directoryIntPopupSize = new int[directoryInfos.Length];
			for (int i = 0; i < directoryInfos.Length; i++)
			{
				directoryIntPopupSize[i] = i;
				directoryToolbarTexts[i] = directoryInfos[i].Name;
			}
			directoryToolbarIndex = EditorGUILayout.IntPopup("地图组目录：", directoryToolbarIndex, directoryToolbarTexts, directoryIntPopupSize, GUILayout.Width(directoryToolbarTexts[directoryToolbarIndex].Length * 20)/*,GUILayout.ExpandWidth(true)*/);

			//地图文件目录
			FileInfo[] fileInfos = directoryInfos[directoryToolbarIndex].GetFiles("*.asset");
			fileToolbarTexts = new string[fileInfos.Length];
			for (int i = 0; i < fileInfos.Length; i++)
			{
				fileToolbarTexts[i] = fileInfos[i].Name;
			}
			if (fileToolbarTexts.Length == 0)
			{
				GUILayout.Label("副本为空，请设置副本数据");
				return;
			}
			fileIntPopupSize = new int[fileToolbarTexts.Length];
			for (int i = 0; i < fileToolbarTexts.Length; i++)
			{
				fileIntPopupSize[i] = i;
			}
			fileToolbarIndex = EditorGUILayout.IntPopup("地图块目录：", fileToolbarIndex, fileToolbarTexts, fileIntPopupSize, GUILayout.Width(fileToolbarTexts[fileToolbarIndex].Length * 15));



			GUILayout.EndVertical();

			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button("打开编辑场景", GUILayout.Height(35)))
			{
				if (EditorSceneManager.GetActiveScene().name == "TilemapEditScene")
				{
					EditorUtility.DisplayDialog("提示", "已经在地图编辑场景。", "确认");
					return;
				}
				else
				{
					EditorSceneManager.OpenScene("Assets/GameMain/Scenes/TilemapEditScene.unity");
				}
			}
			if (GUILayout.Button("预览当前地图块目录地图（未开放）", GUILayout.Height(35)))
			{
				TilemapData data = AssetDatabase.LoadAssetAtPath<TilemapData>(MapFolderPath + "/" + directoryToolbarTexts[directoryToolbarIndex] + "/" + fileToolbarTexts[fileToolbarIndex]);
				CreatePreviewMap(data);
			}
			//if (GUILayout.Button("重置本地数据", GUILayout.Height(35)))
			//{
			//	if (EditorUtility.DisplayDialog("提示", "重置将会把TargetData用本地SourceData数据重置，所有游戏地图信息将会变成SourceData，确定要重置副本吗", "确定", "取消"))
			//	{
			//		TilemapHelper.SaveData(TilemapHelper.TargetMapData, TilemapHelper.SourceMapData);
			//	}
			//}
			//if (GUILayout.Button("保存本地数据", GUILayout.Height(35)))
			//{
			//	if (EditorUtility.DisplayDialog("提示", "保存将会把当前TargetData保存到Unity本地，确定要保存副本吗", "确定", "取消"))
			//	{
			//		EditorUtility.SetDirty(TilemapHelper.TargetMapData);
			//	}
			//}
			//if (GUILayout.Button("保存数据", GUILayout.Height(35)))
			//{
			//	if (EditorUtility.DisplayDialog("提示", "保存数据会将当前编辑的TargetData数据保存到SourceData当中，确定要保存数据吗", "确定", "取消"))
			//	{
			//		TilemapHelper.SaveData(TilemapHelper.SourceMapData, TilemapHelper.TargetMapData);
			//	}
			//}

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("保存序列化文件", GUILayout.Height(35)))
			{
				SaveJsonData(MapFolderPath + "/" +directoryToolbarTexts[directoryToolbarIndex]);
			}
			if (GUILayout.Button("读取序列化文件", GUILayout.Height(35)))
			{
				TilemapData data = AssetDatabase.LoadAssetAtPath<TilemapData>(MapFolderPath + "/" + directoryToolbarTexts[directoryToolbarIndex] + "/" + fileToolbarTexts[fileToolbarIndex]);
				LoadJsonDataAndCreateTilemap(data);
			}
			EditorGUILayout.EndHorizontal();
		}

		private void SaveJsonData(string path)
		{
			if (EditorUtility.DisplayDialog("提示", string.Format("当前Json文件将会保存到路径【{0}】，文件名为【{1}】", directoryInfos[directoryToolbarIndex], TilemapHelper.TargetMapData.TilemapName), "确定", "取消")) 
			{
				TilemapHelper.SaveTilemapData2Json(path);
			}
		}

		private void LoadJsonDataAndCreateTilemap(TilemapData data)
		{
			TilemapHelper.LoadTilemapDataFromJson(data);
		}
		#endregion

		private void TilemapContent()
		{
			EditorGUIUtility.labelWidth = 120;
			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
			string[] StrlayersPopupSize = new string[SortingLayer.layers.Length];
			int[] intlayersPopupSize = new int[SortingLayer.layers.Length];
			for (int i = 0; i < intlayersPopupSize.Length; i++)
			{
				intlayersPopupSize[i] = i;
				StrlayersPopupSize[i] = SortingLayer.layers[i].name;
			}
			string[] SortOrderPopupSize = { TilemapRenderer.SortOrder.BottomLeft.ToString(), TilemapRenderer.SortOrder.BottomRight.ToString(), TilemapRenderer.SortOrder.TopLeft.ToString(), TilemapRenderer.SortOrder.TopRight.ToString() };
			int[] intSortOrderPopupSize = { (int)TilemapRenderer.SortOrder.BottomLeft, (int)TilemapRenderer.SortOrder.BottomRight, (int)TilemapRenderer.SortOrder.TopLeft, (int)TilemapRenderer.SortOrder.TopRight };

			TilemapHelper.TargetMapData.TilemapName = EditorGUILayout.TextField("地图名称 : ", TilemapHelper.TargetMapData.TilemapName, GUILayout.Width(250));
			TilemapHelper.TargetMapData.SortOrderIndex = EditorGUILayout.IntPopup("Sort Order：", TilemapHelper.TargetMapData.SortOrderIndex, SortOrderPopupSize, intSortOrderPopupSize, GUILayout.ExpandWidth(true), GUILayout.Width(250));
			TilemapHelper.TargetMapData.SortingLayerIndex = EditorGUILayout.IntPopup("Sorting Layer：", TilemapHelper.TargetMapData.SortingLayerIndex, StrlayersPopupSize, intlayersPopupSize, GUILayout.ExpandWidth(true), GUILayout.Width(250));
			TilemapHelper.TargetMapData.OrderInLayer = EditorGUILayout.IntField("Order in Layer : ", TilemapHelper.TargetMapData.OrderInLayer, GUILayout.Width(250));
			TilemapHelper.TargetMapData.ChunkWidth = EditorGUILayout.IntField("地图X轴长度 : ", TilemapHelper.TargetMapData.ChunkWidth, GUILayout.Width(250));
			TilemapHelper.TargetMapData.ChunkHeight = EditorGUILayout.IntField("地图Y轴长度 : ", TilemapHelper.TargetMapData.ChunkHeight, GUILayout.Width(250));
			TilemapHelper.TargetMapData.ChunkBeginPosX = EditorGUILayout.IntField("起始Tile位置X坐标 : ", TilemapHelper.TargetMapData.ChunkBeginPosX, GUILayout.Width(250));
			TilemapHelper.TargetMapData.ChunkBeginPosY = EditorGUILayout.IntField("起始Tile位置Y坐标 : ", TilemapHelper.TargetMapData.ChunkBeginPosY, GUILayout.Width(250));

			//获得所有ChunkLogic脚本名，去除基类
			List<string> typeNames = new List<string>();
			Type[] types = Assembly.Load("Assembly-CSharp").GetTypes();
			foreach (Type type in types)
			{
				if (type.IsClass && !type.IsAbstract && typeof(ChunkLogic).IsAssignableFrom(type) && type.Name != typeof(ChunkLogic).Name)
				{
					typeNames.Add(type.FullName);
				}
			}
			typeNames.Sort();

			selectedIndex = EditorGUILayout.Popup("脚本名", selectedIndex, typeNames.ToArray(),GUILayout.Width(250));
			TilemapHelper.TargetMapData.ScriptName = typeNames[selectedIndex];

			GUILayout.EndVertical();

			GUILayout.BeginVertical();
			if (GUILayout.Button("编辑地图", GUILayout.Width(100)))
			{
				CreateCurTileMap();
			}
			GUILayout.Label("当前编辑的地图：" + TilemapHelper.TargetMapData.TilemapName, GUILayout.ExpandWidth(true));

			isDrawLine = GUILayout.Toggle(isDrawLine, "是否显示地图边界");

			GUILayout.EndVertical();
			GUILayout.EndHorizontal();

			if (TilemapHelper.TargetMapData.TileInfoList == null)
			{
				GUILayout.Label("Tile TotalCount : 0");
			}
			else
			{
				GUILayout.Label("Tile TotalCount : " + TilemapHelper.TargetMapData.TileInfoList.Count);
			}

			GUILayout.Label("Tile SearchCount : " + searchCount);

			if (searchText == null)
			{
				searchText = "";
			}
			GUILayout.BeginHorizontal();
			GUILayout.Label("请输入搜索关键字:", GUILayout.Width(120));
			searchText = GUILayout.TextField(searchText, GUILayout.ExpandWidth(true));
			if (GUILayout.Button("清空输入", GUILayout.Width(80)))
			{
				searchText = "";
				GUI.changed = true;
				GUIUtility.keyboardControl = 0;
			}
			GUILayout.EndHorizontal();

			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
			int tempCount = 0;
			if (TilemapHelper.TargetMapData.TileInfoList != null && TilemapHelper.TargetMapData.TileInfoList.Count != 0)
			{
				for (int i = 0; i < TilemapHelper.TargetMapData.TileInfoList.Count; i++)
				{
					CherryTile tile = (CherryTile)TilemapHelper.TargetMapData.TileInfoList[i].Tile;
					if (tile != null)
					{
						m_StringBuilder.Clear();
						string tempStr = m_StringBuilder.Append("【").Append(i.ToString("0000")).Append("】     Pos:").Append(TilemapHelper.TargetMapData.TileInfoList[i].Pos.ToString()).Append("      Tile:").Append(tile.ToString()).ToString();
						if (tempStr.ToLower().Contains(searchText.ToLower()))
						{
							tempCount++;
							GUILayout.Label(tempStr);
						}
					}			
				}
			}
			searchCount = tempCount;
			EditorGUILayout.EndScrollView();
		}

		private void OnInspectorUpdate()
		{
			this.Repaint();
		}

		/// <summary>
		/// 创建并编辑当前选择的Tilemap的地图信息。
		/// </summary>
		private void CreateCurTileMap()
		{
			GameObject grid = GameObject.Find("Grid");
			if (grid == null)
			{
				grid = new GameObject("Grid");
				grid.AddComponent<Grid>();
				grid.transform.position = new Vector3(-0.5f, -0.5f, 0);
			}
			//if (grid)
			//{
			//	DestroyImmediate(grid);
			//}
			//grid = new GameObject("Grid");
			//for (int i = 0; i < grid.transform.childCount; i++)
			//{
			//	DestroyImmediate(grid.transform.GetChild(i).gameObject);
			//}
			GameObject tilemap = new GameObject(TilemapHelper.TargetMapData.TilemapName);
			tilemap.transform.SetParent(grid.transform);
			tilemap.transform.localPosition = Vector3.zero;
			Tilemap map = tilemap.AddComponent<Tilemap>();
			TilemapRenderer render = tilemap.AddComponent<TilemapRenderer>();
			render.sortOrder = (TilemapRenderer.SortOrder)TilemapHelper.TargetMapData.SortOrderIndex;
			render.sortingOrder = TilemapHelper.TargetMapData.OrderInLayer;
			render.sortingLayerName = SortingLayer.layers[TilemapHelper.TargetMapData.SortingLayerIndex].name;
			TilemapHelper.SetTileMap(map, TilemapHelper.TargetMapData);
			//编辑Tilemap
			Selection.activeObject = tilemap;
		}

		private void CreatePreviewMap(TilemapData data)
		{
			GameObject grid = GameObject.Find("Grid");
			if (grid == null)
			{
				grid = new GameObject("Grid");
				grid.transform.position = new Vector3(-0.5f, -0.5f, 0);
				grid.AddComponent<Grid>();
			}
			GameObject tilemap = new GameObject(data.TilemapName);
			tilemap.transform.SetParent(grid.transform);
			tilemap.transform.localPosition = Vector3.zero;
			Tilemap map = tilemap.AddComponent<Tilemap>();
			TilemapRenderer render = tilemap.AddComponent<TilemapRenderer>();
			render.sortOrder = (TilemapRenderer.SortOrder)data.SortOrderIndex;
			render.sortingOrder = data.OrderInLayer;
			render.sortingLayerName = SortingLayer.layers[data.SortingLayerIndex].name;
			TilemapHelper.SetTileMap(map, data);
		}

		private void SettingContent()
		{

		}

		private void AboutContent()
		{

		}

		private void CreateTileMap()
		{

		}
	}
}
