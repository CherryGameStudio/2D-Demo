using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using GameFramework.DataTable;
using Cherry.Tilemaps;
using UnityEngine.Tilemaps;
using UnityGameFramework.Runtime;
using GameFramework.Event;

namespace Cherry
{
	public class TestUIForm : UGUIFormBase
	{
		protected override void OnOpen(object userData)
		{
			base.OnOpen(userData);
			UnityGameFramework.Runtime.Log.Info("打开主界面");
			GameEntry.Event.Subscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
		}

		public override void OnClick(GameObject gameObject)
		{
			//load scene
			if (gameObject.name == "Button")
			{
				//GameEntry.Resource.LoadAsset("Assets/GameMain/AssetData/Map/DarkForest/Forest.asset", new GameFramework.Resource.LoadAssetCallbacks(LoadSuccess));
				//GameEntry.Map.LoadChunk("Assets/GameMain/AssetData/Map/DarkForest/Forest.asset");
				GameEntry.Map.LoadChunk(ChunkType.Forest);
				GameEntry.Entity.ShowEntity<PlayerCharacter>(1, "Assets/GameMain/AssetData/Character/Player/Player.prefab", "Player");
				Entity entity = GameEntry.Entity.GetEntity(1);
				//GameEntry.Scene.LoadSceneBySceneType(SceneType.TestScene);
				//GameEntry.UI.CloseUIForm(this.UIForm);

				/*
				//测试读表
				IDataTable<DRtest> dataTable = GameEntry.DataTable.GetDataTable<DRtest>();
				var row1 = dataTable.GetDataRow(1);
				var row2 = dataTable.GetDataRow(2);
				string name1 = row1.name;
				string name2 = row2.name;
				int score1 = row1.score;
				int score2 = row2.score;
				GLogger.ErrorFormat(Log_Channel.DataTable, "测试读表：name1是{0},name2是{1},score1是{2},score2是{3}", name1, name2, score1, score2);

				//测试GLogger
				int a = 2, b = 3, c = 1;
				GLogger.DebugFormat(Log_Channel.UI, "测试GLogger：a是{0}，b是{1}，c是{2}", a, b, c);
				*/
			}
		}


		//测试加载地图，不论是否真正加载了内存，加载成功回调都会调用
		private void LoadSuccess(string chunkAssetName, object chunkAsset, float duration, object userData)
		{
			Debug.LogError(1);
			this.StartCoroutine(LoadTilemap((TilemapData)chunkAsset));
		}

		private IEnumerator LoadTilemap(TilemapData chunkAsset)
		{
			GameObject grid = GameObject.Find("Grid");
			if (grid == null)
			{
				grid = new GameObject("Grid");
			}
			grid.AddComponent<Grid>();
			grid.transform.position = new Vector3(-0.5f, -0.5f, 0);
			GameObject map = new GameObject(chunkAsset.TilemapName);
			map.transform.SetParent(grid.transform);
			Tilemap tilemap = map.AddComponent<Tilemap>();
			tilemap.transform.localPosition = Vector3.zero;
			map.AddComponent<TilemapRenderer>();
			foreach (var item in chunkAsset.TileInfoList)
			{
				tilemap.SetTile(item.IntPos, (CherryTile)item.Tile);
				yield return new WaitForEndOfFrame();
			}
			yield return null;
		}

		private void OnShowEntitySuccess(object sender, GameEventArgs e)
		{
			Debug.Log("实体加载成功");
		}
	}
}
