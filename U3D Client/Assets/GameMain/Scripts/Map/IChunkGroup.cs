using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cherry
{
	/// <summary>
	/// 地图块组接口。
	/// </summary>
	public interface IChunkGroup
	{
		/// <summary>
		/// 获取地图块组名称。
		/// </summary>
		string ChunkGroupName
		{
			get;
		}

		/// <summary>
		/// 获取或设置地图块组是否暂停。
		/// </summary>
		bool Pause
		{
			get;
			set;
		}

		/// <summary>
		/// 获取地图块组中地图块数量。
		/// </summary>
		int ChunkCount
		{
			get;
		}

		/// <summary>
		/// 获取当前地图块。
		/// </summary>
		IChunk CurrentChunk
		{
			get;
		}

		/// <summary>
		/// 获取地图块组辅助器。
		/// </summary>
		IChunkGroupHelper ChunkGroupHelper
		{
			get;
		}

		/// <summary>
		/// 地图块组中是否存在地图块。
		/// </summary>
		/// <param name="serialId">地图块序列编号。</param>
		/// <returns>地图块组是否存在地图块。</returns>
		bool HasChunk(int serialId);

		/// <summary>
		/// 地图块组中是否存在地图块。
		/// </summary>
		/// <param name="chunkAssetName">地图块资源名称。</param>
		/// <returns>地图块组是否存在地图块。</returns>
		bool HasChunk(string chunkAssetName);

		/// <summary>
		/// 从地图块组中获取地图块。
		/// </summary>
		/// <param name="serialId">地图块序列编号。</param>
		/// <returns>要获取的地图块。</returns>
		IChunk GetChunk(int serialId);

		/// <summary>
		/// 从地图块组中获取地图块。
		/// </summary>
		/// <param name="chunkAssetName">地图块资源名称。</param>
		/// <returns>要获取的地图块。</returns>
		IChunk GetChunk(string chunkAssetName);

		/// <summary>
		/// 从地图块组中获取地图块。
		/// </summary>
		/// <param name="chunkAssetName">地图块资源名称。</param>
		/// <returns>要获取的地图块。</returns>
		IChunk[] GetChunks(string chunkAssetName);

		/// <summary>
		/// 从地图块组中获取地图块。
		/// </summary>
		/// <param name="chunkAssetName">地图块资源名称。</param>
		/// <param name="results">要获取的地图块。</param>
		void GetChunks(string chunkAssetName, List<IChunk> results);

		/// <summary>
		/// 获取地图块组中所有地图块。
		/// </summary>
		/// <returns>地图块组中所有的地图块。</returns>
		IChunk[] GetAllChunks();

		/// <summary>
		/// 获取地图块组中所有的地图块。
		/// </summary>
		/// <param name="results">地图块组中所有的地图块。</param>
		void GetAllChunks(List<IChunk> results);
	}
}
