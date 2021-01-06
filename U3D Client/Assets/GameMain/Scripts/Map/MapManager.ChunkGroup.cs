using GameFramework;
using System.Collections.Generic;

namespace Cherry
{
    public partial class MapManager
    {
        /// <summary>
        /// 地图块组。
        /// </summary>
        private sealed partial class ChunkGroup : IChunkGroup
		{
            private readonly string m_Name;
            private bool m_Pause;
            private readonly IChunkGroupHelper m_ChunkGroupHelper;
            private readonly GameFrameworkLinkedList<ChunkInfo> m_ChunkInfos;
            private LinkedListNode<ChunkInfo> m_CachedNode;

			public ChunkGroup(string name, IChunkGroupHelper chunkGroupHelper)
			{
				if (string.IsNullOrEmpty(name))
				{
					throw new GameFrameworkException("ChunkGroup name is invaild.");
				}
				if (chunkGroupHelper == null)
				{
					throw new GameFrameworkException("ChunkGroup is invaild.");
				}
				m_Name = name;
				m_Pause = false;
				m_ChunkGroupHelper = chunkGroupHelper;
				m_ChunkInfos = new GameFrameworkLinkedList<ChunkInfo>();
				m_CachedNode = null;
			}

			public string ChunkGroupName
			{
				get { return m_Name; }
			}

			public bool Pause
			{
				get { return m_Pause; }
				set { m_Pause = value; }
			}

			public int ChunkCount
			{
				get { return m_ChunkInfos.Count; }
			}

			public IChunk CurrentChunk
			{
				get { return m_ChunkInfos.First != null ? m_ChunkInfos.First.Value.Chunk : null; }
			}

			public IChunkGroupHelper ChunkGroupHelper
			{
				get { return m_ChunkGroupHelper; }
			}

			public void Update()
			{
				LinkedListNode<ChunkInfo> current = m_ChunkInfos.First;
				while (current != null) 
				{
					if (current.Value.Pause)
					{
						break;
					}

					m_CachedNode = current.Next;
					current.Value.Chunk.OnUpdate();
					current = m_CachedNode;
					m_CachedNode = null;
				}
			}

			public bool HasChunk(int serialId)
			{
				foreach (ChunkInfo chunkInfo in m_ChunkInfos)
				{
					if (chunkInfo.Chunk.ChunkId == serialId)
					{
						return true;
					}
				}

				return false;
			}

			public bool HasChunk(string chunkAssetName)
			{
				if (string.IsNullOrEmpty(chunkAssetName))
				{
					throw new GameFrameworkException("chunkAssetName is invaild.");
				}

				foreach (ChunkInfo chunkInfo in m_ChunkInfos)
				{
					if (chunkInfo.Chunk.ChunkAssetName == chunkAssetName)
					{
						return true;
					}
				}

				return false;
			}

			public IChunk GetChunk(int serialId)
			{
				foreach (ChunkInfo chunkInfo in m_ChunkInfos)
				{
					if (chunkInfo.Chunk.ChunkId == serialId)
					{
						return chunkInfo.Chunk;
					}
				}

				return null;
			}

			public IChunk GetChunk(string chunkAssetName)
			{
				if (string.IsNullOrEmpty(chunkAssetName))
				{
					throw new GameFrameworkException("chunkAssetName is invaild.");
				}

				foreach (ChunkInfo chunkInfo in m_ChunkInfos)
				{
					if (chunkInfo.Chunk.ChunkAssetName == chunkAssetName)
					{
						return chunkInfo.Chunk;
					}
				}

				return null;
			}

			public IChunk[] GetChunks(string chunkAssetName)
			{
				if (string.IsNullOrEmpty(chunkAssetName))
				{
					throw new GameFrameworkException("chunkAssetName is invaild.");
				}

				List<IChunk> results = new List<IChunk>();
				foreach (ChunkInfo chunkInfo in m_ChunkInfos)
				{
					if (chunkInfo.Chunk.ChunkAssetName == chunkAssetName)
					{
						results.Add(chunkInfo.Chunk);
					}
				}

				return results.ToArray();
			}

			public void GetChunks(string chunkAssetName, List<IChunk> results)
			{
				if (string.IsNullOrEmpty(chunkAssetName))
				{
					throw new GameFrameworkException("chunkAssetName is invaild.");
				}

				if (results == null)
				{
					throw new GameFrameworkException("results is invaild.");
				}

				results.Clear();
				foreach (ChunkInfo chunkInfo in m_ChunkInfos)
				{
					if (chunkInfo.Chunk.ChunkAssetName == chunkAssetName)
					{
						results.Add(chunkInfo.Chunk);
					}
				}
			}

			public IChunk[] GetAllChunks()
			{
				List<IChunk> results = new List<IChunk>();
				foreach (ChunkInfo chunkInfo in m_ChunkInfos)
				{
					results.Add(chunkInfo.Chunk);
				}

				return results.ToArray();
			}

			public void GetAllChunks(List<IChunk> results)
			{
				if (results == null)
				{
					throw new GameFrameworkException("results is invaild.");
				}

				results.Clear();
				foreach (ChunkInfo chunkInfo in m_ChunkInfos)
				{
					results.Add(chunkInfo.Chunk);
				}
			}

			public void AddChunk(IChunk chunk)
			{
				m_ChunkInfos.AddFirst(ChunkInfo.Create(chunk));
			}

			public void RemoveChunk(IChunk chunk)
			{
				ChunkInfo chunkInfo = GetChunkInfo(chunk);
				if (chunkInfo == null)
				{
					throw new GameFrameworkException(Utility.Text.Format("Can not find Chunk form info for serial id '{0}', Chunk asset name is '{1}'.", chunk.ChunkId.ToString(), chunk.ChunkAssetName));
				}

				if (!chunkInfo.Pause)
				{
					chunkInfo.Pause = true;
					chunk.OnPause();
				}

				if (m_CachedNode != null && m_CachedNode.Value.Chunk == chunk)
				{
					m_CachedNode = m_CachedNode.Next;
				}

				if (!m_ChunkInfos.Remove(chunkInfo))
				{
					throw new GameFrameworkException(Utility.Text.Format("Chunk group '{0}' not exists specified chunk '[{1}]{2}'.", m_Name, chunk.ChunkId.ToString(), chunk.ChunkAssetName));
				}

				ReferencePool.Release(chunkInfo);
			}

			public void Refresh()
			{
				LinkedListNode<ChunkInfo> current = m_ChunkInfos.First;
				bool pause = m_Pause;
				while (current != null && current.Value != null) 
				{
					LinkedListNode<ChunkInfo> next = current.Next;
					if (pause)
					{
						if (!current.Value.Pause)
						{
							current.Value.Pause = true;
							current.Value.Chunk.OnPause();
						}
					}
					else
					{
						if (current.Value.Pause)
						{
							current.Value.Pause = false;
							current.Value.Chunk.OnResume();
						}
						//todo 考虑是否给Chunk添加PauseCoveredChunk这个参数
					}

					current = next;
				}
			}

			private ChunkInfo GetChunkInfo(IChunk chunk)
			{
				if (chunk == null)
				{
					throw new GameFrameworkException("Chunk is invaild.");
				}

				foreach (ChunkInfo chunkInfo in m_ChunkInfos)
				{
					if (chunkInfo.Chunk == chunk)
					{
						return chunkInfo;
					}
				}

				return null;
			}
		}
    }
}
