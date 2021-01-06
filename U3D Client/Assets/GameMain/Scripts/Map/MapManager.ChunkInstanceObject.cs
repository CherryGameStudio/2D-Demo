using GameFramework;
using GameFramework.ObjectPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cherry
{
	public partial class MapManager
	{
		/// <summary>
		/// 地图块实例对象。
		/// </summary>
		private sealed class ChunkInstanceObject : ObjectBase
		{
			private object m_ChunkAsset;
			private IChunkHelper m_ChunkHelper;

			public object ChunkAsset
			{
				get { return m_ChunkAsset; }
			}

			public ChunkInstanceObject()
			{
				m_ChunkAsset = null;
				m_ChunkHelper = null;
			}

			public static ChunkInstanceObject Create(string name,object chunkAsset,object chunkInstance, IChunkHelper chunkHelper)
			{
				if (chunkAsset == null) 
				{
					throw new GameFrameworkException("chunkAsset is invaild.");
				}

				if (chunkInstance == null) 
				{
					throw new GameFrameworkException("chunkInstance is invaild.");
				}

				ChunkInstanceObject chunkInstanceObject = ReferencePool.Acquire<ChunkInstanceObject>();
				chunkInstanceObject.Initialize(name, chunkInstance);
				chunkInstanceObject.m_ChunkAsset = chunkAsset;
				chunkInstanceObject.m_ChunkHelper = chunkHelper;
				return chunkInstanceObject;
			}

			public override void Clear()
			{
				base.Clear();
				m_ChunkAsset = null;
				m_ChunkHelper = null;
			}

			protected override void Release(bool isShutdown)
			{
				m_ChunkHelper.ReleaseChunk(m_ChunkAsset, Target);
			}
		}
	}
}
