using GameFramework.DataTable;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using UnityEngine.Tilemaps;
using Cherry.Util;
using Cherry.Tilemaps;

namespace Cherry
{
	public class DefaultChunkHelper : ChunkHelperBase
	{
		private ResourceComponent m_ResourceComponent = null; 

		public override IChunk CreateChunk(object chunkInstance, IChunkGroup chunkGroup, object userData)
		{
			GameObject gameObject = chunkInstance as GameObject;
			if (gameObject == null)
			{
				Log.Error("Chunk instance is invalid.");
				return null;
			}

			gameObject.transform.SetParent(((MonoBehaviour)chunkGroup.ChunkGroupHelper).transform);
			return gameObject.GetOrAddComponent<Chunk>();
		}

		public override void GetDependentChunkAssetNames(string chunkAssetName, List<string> chunkAssetNames)
		{
			IDataTable<DRChunk> dataTable = GameEntry.DataTable.GetDataTable<DRChunk>();
			DRChunk dataRow = dataTable.GetDataRow((DRChunk m_DataRow) => { return m_DataRow.ChunkAssetName == chunkAssetName; });
			chunkAssetNames.Clear();
			foreach (string assetName in dataRow.DependentChunkAssetNames)
			{
				chunkAssetNames.Add(assetName);
			}
		}

		public override object InstantiateChunk(object chunkFormAsset)
		{
			GameObject go = new GameObject((chunkFormAsset as TilemapData).TilemapName);
			go.AddComponent<Tilemap>();
			go.AddComponent<TilemapRenderer>();
			go.AddComponent<TilemapCollider2D>().usedByComposite = true;
			go.AddComponent<Rigidbody2D>().isKinematic = true;
			go.AddComponent<CompositeCollider2D>();

			//try catch
			go.AddComponent(System.Type.GetType((chunkFormAsset as TilemapData).ScriptName));
			return go;
			//如果地图是资源文件则直接实例化Asset
			//return Instantiate((Object)chunkFormAsset);
		}

		public override void ReleaseChunk(object chunkAsset, object chunkInstance)
		{
			m_ResourceComponent.UnloadAsset(chunkAsset);
			Destroy((Object)chunkInstance);
		}

		private void Start()
		{
			m_ResourceComponent = GameEntry.Resource;
			if (m_ResourceComponent == null)
			{
				Log.Fatal("Resource component is invalid.");
				return;
			}
		}
	}
}
