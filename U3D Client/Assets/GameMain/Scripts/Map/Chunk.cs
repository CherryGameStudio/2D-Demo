using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cherry
{
	/// <summary>
	/// 地图块。
	/// </summary>
	public class Chunk : MonoBehaviour, IChunk
	{
		[SerializeField]
		private int m_ChunkId;
		[SerializeField]
		private bool m_IsReady;
		[SerializeField]
		private string m_ChunkAssetName;
		[SerializeField]
		private List<string> m_DependentChunkAssetNames = new List<string>();
		private IChunkGroup m_ChunkGroup;
		private ChunkLogic m_ChunkLogic;

		public int ChunkId
		{
			get { return m_ChunkId; }
		}

		public bool IsReady
		{
			get { return m_IsReady; }
			set { m_IsReady = value; }
		}

		public string ChunkAssetName
		{
			get { return m_ChunkAssetName; }
		}

		public object Handle
		{
			get { return gameObject; }
		}

		public IChunkGroup ChunkGroup
		{
			get { return m_ChunkGroup; }
		}

		public ChunkLogic ChunkLogic
		{
			get { return m_ChunkLogic; }
		}

		public List<string> ChunkDependentAssetNames
		{
			get { return m_DependentChunkAssetNames; }
		}

		public int ChunkDependencyAssetCount
		{
			get { return m_DependentChunkAssetNames.Count; }
		}

		public void OnInit(int serialId, string chunkAssetName,List<string> chunkDependentAssetNames, IChunkGroup chunkGroup, bool isNewInstance, object userData)
		{
			m_ChunkId = serialId;
			m_ChunkAssetName = chunkAssetName;
			m_ChunkGroup = chunkGroup;
			foreach (string assetName in chunkDependentAssetNames)
			{
				m_DependentChunkAssetNames.Add(assetName);
			}

			if (!isNewInstance)
			{
				return;
			}

			m_ChunkLogic = GetComponent<ChunkLogic>();
			if (m_ChunkLogic == null)
			{
				GLogger.ErrorFormat(Log_Channel.Chunk, "Chunk '{0}' can not get chunk logic.", chunkAssetName);
			}

			try
			{
				m_ChunkLogic.OnInit(userData);
			}
			catch (Exception exception)
			{
				GLogger.ErrorFormat(Log_Channel.Chunk,"Chunk '[{0}]{1}' OnInit with exception '{2}'.", m_ChunkId.ToString(), m_ChunkAssetName, exception.ToString());
			}
		}

		public void OnRecycle()
		{
			try
			{
				m_ChunkLogic.OnRecycle();
			}
			catch (Exception exception)
			{
				GLogger.ErrorFormat(Log_Channel.Chunk, "Chunk '[{0}]{1}' OnRecycle with exception '{2}'.", m_ChunkId.ToString(), m_ChunkAssetName, exception.ToString());
			}

			m_ChunkId = 0;
		}

		public void OnEnter(object userData)
		{
			try
			{
				m_ChunkLogic.OnEnter(userData);
			}
			catch (Exception exception)
			{
				GLogger.ErrorFormat(Log_Channel.Chunk, "Chunk '[{0}]{1}' OnEnter with exception '{2}'.", m_ChunkId.ToString(), m_ChunkAssetName, exception.ToString());
			}
		}

		public void OnLeave(object userData)
		{
			try
			{
				m_ChunkLogic.OnLeave(userData);
			}
			catch (Exception exception)
			{
				GLogger.ErrorFormat(Log_Channel.Chunk, "Chunk '[{0}]{1}' OnLeave with exception '{2}'.", m_ChunkId.ToString(), m_ChunkAssetName, exception.ToString());
			}
		}

		public void OnPause()
		{
			try
			{
				m_ChunkLogic.OnPause();
			}
			catch (Exception exception)
			{
				GLogger.ErrorFormat(Log_Channel.Chunk, "Chunk '[{0}]{1}' OnPause with exception '{2}'.", m_ChunkId.ToString(), m_ChunkAssetName, exception.ToString());
			}
		}

		public void OnResume()
		{
			try
			{
				m_ChunkLogic.OnResume();
			}
			catch (Exception exception)
			{
				GLogger.ErrorFormat(Log_Channel.Chunk, "Chunk '[{0}]{1}' OnResume with exception '{2}'.", m_ChunkId.ToString(), m_ChunkAssetName, exception.ToString());
			}
		}

		public void OnUpdate()
		{
			try
			{
				m_ChunkLogic.OnUpdate();
			}
			catch (Exception exception)
			{
				GLogger.ErrorFormat(Log_Channel.Chunk, "Chunk '[{0}]{1}' OnUpdate with exception '{2}'.", m_ChunkId.ToString(), m_ChunkAssetName, exception.ToString());
			}
		}
	}
}
