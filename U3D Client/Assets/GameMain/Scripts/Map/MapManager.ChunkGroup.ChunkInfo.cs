using GameFramework;

namespace Cherry
{
	public partial class MapManager
	{
		private sealed partial class ChunkGroup
		{
			/// <summary>
			/// 地图块信息。
			/// </summary>
			private sealed class ChunkInfo : IReference
			{
				private IChunk m_Chunk;
				private bool m_Pause;

				public ChunkInfo()
				{
					m_Chunk = null;
					m_Pause = false;
				}

				public IChunk Chunk
				{
					get { return m_Chunk; }
				}

				public bool Pause
				{
					get { return m_Pause; }
					set { m_Pause = value; }
				}

				public static ChunkInfo Create(IChunk chunk)
				{
					if (chunk==null)
					{
						throw new GameFrameworkException("Chunk is invaild.");
					}

					ChunkInfo chunkInfo = ReferencePool.Acquire<ChunkInfo>();
					chunkInfo.m_Chunk = chunk;
					chunkInfo.m_Pause = true;
					return chunkInfo;
				}

				public void Clear()
				{
					m_Chunk = null;
					m_Pause = false;
				}
			}
		}
	}
}
