using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cherry
{
	/// <summary>
	/// 地图块辅助器接口。
	/// </summary>
	public interface IChunkHelper
	{
		/// <summary>
		/// 实例化地图块。
		/// </summary>
		/// <param name="chunkFormAsset">要实例化的地图块资源。</param>
		/// <returns>实例化后的地图块。</returns>
		object InstantiateChunk(object chunkFormAsset);

		/// <summary>
		/// 创建地图块。
		/// </summary>
		/// <param name="chunkInstance">地图块实例。</param>
		/// <param name="chunkGroup">地图块所属的地图块组。</param>
		/// <param name="userData">用户自定义的数据。</param>
		/// <returns>地图块。</returns>
		IChunk CreateChunk(object chunkInstance, IChunkGroup chunkGroup, object userData);

		/// <summary>
		/// 回收地图块。
		/// </summary>
		/// <param name="chunkAsset">要回收的地图块资源。</param>
		/// <param name="chunkInstance">要回收的地图块实例。</param>
		void ReleaseChunk(object chunkAsset, object chunkInstance);

		/// <summary>
		/// 获取地图块所有依赖的地图块资源名称，包括自己。
		/// </summary>
		/// <param name="chunkAssetName">地图块资源名称。</param>
		/// <param name="chunkAssetNames">依赖的地图块资源名称，包括自己。</param>
		void GetDependentChunkAssetNames(string chunkAssetName,List<string> chunkAssetNames);

		/// <summary>
		/// 获取地图块所有依赖的地图块Id，包括自己。
		/// </summary>
		/// <param name="chunkId">地图块Id。</param>
		/// <param name="chunkIds">依赖的地图块Id，包括自己。</param>
		void GetDependentChunkIds(int chunkId, List<int> chunkIds);
	}
}
