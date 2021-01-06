using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cherry
{
	public sealed partial class MapManager
	{
		/// <summary>
		/// 加载地图时传递给加载回调的信息。
		/// </summary>
		private sealed class LoadingChunkInfo : IReference
		{
			private int m_SerialId;
			private ChunkGroup m_ChunkGroup;
			private bool m_IsEnterChunk = false;
			private bool m_IsOnlyLoadChunk = false;
			private List<string> m_LoadingAssetNames = new List<string>();
			private object m_UserData;

			public LoadingChunkInfo()
			{
				m_SerialId = 0;
				m_ChunkGroup = null;
				m_IsEnterChunk = false;
				m_IsOnlyLoadChunk = false;
				m_LoadingAssetNames.Clear();
				m_UserData = null;
			}

			public int SerialId
			{
				get { return m_SerialId; }
			}

			//读表获得，因为没办法知道依赖地图块所属地图组
			//public ChunkGroup ChunkGroup
			//{
			//	get { return m_ChunkGroup; }
			//}

			public bool IsEnterChunk
			{
				get { return m_IsEnterChunk; }
			}

			public bool IsOnlyLoadChunk
			{
				get { return m_IsOnlyLoadChunk; }
			}

			public List<string> LoadingAssetNames
			{
				get { return m_LoadingAssetNames; }
			}

			public object UserData
			{
				get { return m_UserData; }
			}

			public static LoadingChunkInfo Create(int serialId,object UserData,bool isEnterChunk,bool isOnlyLoadChunk)
			{
				LoadingChunkInfo loadingChunkInfo = ReferencePool.Acquire<LoadingChunkInfo>();
				loadingChunkInfo.m_SerialId = serialId;
				loadingChunkInfo.m_UserData = UserData;
				loadingChunkInfo.m_IsEnterChunk = isEnterChunk;
				loadingChunkInfo.m_IsOnlyLoadChunk = isOnlyLoadChunk;
				return loadingChunkInfo;
			}

			public void Clear()
			{
				m_SerialId = 0;
				m_ChunkGroup = null;
				m_IsEnterChunk = false;
				m_IsOnlyLoadChunk = false;
				m_LoadingAssetNames.Clear();
				m_UserData = null;
			}
		}
	}
}
