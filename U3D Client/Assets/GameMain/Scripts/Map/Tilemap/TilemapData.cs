using System.Collections.Generic;
using UnityEngine;

namespace Cherry.Tilemaps
{
	/// <summary>
	/// 瓦片地图信息。
	/// </summary>
	[System.Serializable]
	[CreateAssetMenu(order = -100)]
	public sealed class TilemapData : ScriptableObject
	{
		[SerializeField]
		public string TilemapName = "DefaultTilemapName";
		[SerializeField]
		public int SortOrderIndex = 0;
		[SerializeField]
		public int SortingLayerIndex = 0;
		[SerializeField]
		public int OrderInLayer = 0;
		[SerializeField]
		public int ChunkWidth = 0;
		[SerializeField]
		public int ChunkHeight = 0;
		[SerializeField]
		public int ChunkBeginPosX = 0;
		[SerializeField]
		public int ChunkBeginPosY = 0;
		[SerializeField]
		public string ScriptName;
		[SerializeField]
		public List<TileInfo> TileInfoList = new List<TileInfo>();
	}
}
