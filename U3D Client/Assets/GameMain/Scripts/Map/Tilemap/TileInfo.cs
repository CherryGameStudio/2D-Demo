using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Cherry.Tilemaps
{
	/// <summary>
	/// 瓦片信息。
	/// </summary>
	[System.Serializable]
	public sealed class TileInfo/* : IReference*/
	{
		[SerializeField]
		public Tile Tile;
		[SerializeField]
		public Vector3 Pos;
		[SerializeField]
		public Vector3Int IntPos;
	}
}
