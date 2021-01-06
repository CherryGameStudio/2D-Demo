using Cherry.Tilemaps;
using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cherry
{
	public class ChunkParams : IReference
	{
		public TilemapData TilemapData = null;

		public DRChunk ChunkData = null;

		public object UserData = null;

		public void Clear()
		{
			TilemapData = null;
			ChunkData = null;
			UserData = null;
		}
	}
}
