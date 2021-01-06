using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cherry
{
	public abstract class ChunkHelperBase : MonoBehaviour, IChunkHelper
	{
		public abstract IChunk CreateChunk(object chunkInstance, IChunkGroup chunkGroup, object userData);
		public abstract void GetDependentChunkAssetNames(string chunkAssetName, List<string> chunkAssetNames);
		public abstract object InstantiateChunk(object chunkFormAsset);
		public abstract void ReleaseChunk(object chunkAsset, object chunkInstance);
	}
}
