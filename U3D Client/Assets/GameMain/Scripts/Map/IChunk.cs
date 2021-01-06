using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cherry
{
	/// <summary>
	/// 地图块接口
	/// </summary>
	public interface IChunk
	{
		/// <summary>
		/// 获取地图块序列编号。
		/// </summary>
		int SerialId
		{
			get;
		}

		/// <summary>
		/// 获取地图块资源名称。
		/// </summary>
		string ChunkAssetName
		{
			get;
		}

		List<string> ChunkDependentAssetNames
		{
			get;
		}

		/// <summary>
		/// 获取地图块实例
		/// </summary>
		object Handle
		{
			get;
		}

		/// <summary>
		/// 获取地图块组
		/// </summary>
		IChunkGroup ChunkGroup
		{
			get;
		}

		/// <summary>
		/// 初始化地图块。
		/// </summary>
		/// <param name="serialId">地图块序列化编号。</param>
		/// <param name="chunkAssetName">地图块资源名称。</param>
		/// <param name="chunkGroup">地图块组。</param>
		/// <param name="isNewInstance">是否是新实例。</param>
		/// <param name="userData">用户自定义数据。</param>
		void OnInit(int serialId, string chunkAssetName, List<string> chunkDependentAssetNames, IChunkGroup chunkGroup, bool isNewInstance, object userData);

		/// <summary>
		/// 地图块回收。
		/// </summary>
		void OnRecycle();

		/// <summary>
		/// 玩家进入地图块。
		/// </summary>
		/// <param name="userData">用户自定义数据。</param>
		void OnEnter(object userData);

		/// <summary>
		/// 玩家离开地图块。
		/// </summary>
		/// <param name="userData"></param>
		void OnLeave(object userData);

		/// <summary>
		/// 地图块逻辑暂停。
		/// </summary>
		void OnPause();

		/// <summary>
		/// 地图块逻辑恢复。
		/// </summary>
		void OnResume();

		/// <summary>
		/// 地图块轮询。
		/// </summary>
		void OnUpdate();
	}
}
