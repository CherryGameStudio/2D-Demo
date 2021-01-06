using GameFramework;
using GameFramework.ObjectPool;
using GameFramework.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Cherry
{
	/// <summary>
	/// 游戏场景地图组件
	/// </summary>
	[DisallowMultipleComponent]
	public sealed partial class MapComponent : GameFrameworkComponent
	{
		private const int DefaultPriority = 0;

		private MapManager m_MapManager = null;
		private EventComponent m_EventComponent = null;

		private readonly List<IChunk> m_InternalChunkResults = new List<IChunk>();

		[SerializeField]
		private float m_InstanceAutoReleaseInterval = 60f;

		[SerializeField]
		private int m_InstanceCapacity = 16;

		[SerializeField]
		private float m_InstanceExpireTime = 60f;

		[SerializeField]
		private int m_InstancePriority = 0;

		[SerializeField]
		private Transform m_InstanceRoot = null;

		[SerializeField]
		private string m_ChunkHelperTypeName = "Cherry.DefaultChunkHelper";

		[SerializeField]
		private ChunkHelperBase m_CustomChunkHelper = null;

		[SerializeField]
		private string m_ChunkGroupHelperTypeName = "Cherry.DefaultChunkGroupHelper";

		[SerializeField]
		private ChunkGroupHelperBase m_CustomChunkGroupHelper = null;

		[SerializeField]
		private ChunkGroup[] m_ChunkGroups = null;

		/// <summary>
		/// 获取地图块组地图块数量。
		/// </summary>
		public int ChunkGroupCount
		{
			get
			{
				return m_MapManager.ChunkGroupCount;
			}
		}

		/// <summary>
		/// 获取或设置地图块实例对象池自动释放可释放对象的间隔秒数。
		/// </summary>
		public float InstanceAutoReleaseInterval
		{
			get
			{
				return m_MapManager.InstanceAutoReleaseInterval;
			}
			set
			{
				m_MapManager.InstanceAutoReleaseInterval = m_InstanceAutoReleaseInterval = value;
			}
		}

		/// <summary>
		/// 获取或设置地图块实例对象池的容量。
		/// </summary>
		public int InstanceCapacity
		{
			get
			{
				return m_MapManager.InstanceCapacity;
			}
			set
			{
				m_MapManager.InstanceCapacity = m_InstanceCapacity = value;
			}
		}

		/// <summary>
		/// 获取或设置地图块实例对象池对象过期秒数。
		/// </summary>
		public float InstanceExpireTime
		{
			get
			{
				return m_MapManager.InstanceExpireTime;
			}
			set
			{
				m_MapManager.InstanceExpireTime = m_InstanceExpireTime = value;
			}
		}

		/// <summary>
		/// 获取或设置地图块实例对象池的优先级。
		/// </summary>
		public int InstancePriority
		{
			get
			{
				return m_MapManager.InstancePriority;
			}
			set
			{
				m_MapManager.InstancePriority = m_InstancePriority = value;
			}
		}


		protected override void Awake()
		{
			base.Awake();

			m_MapManager = new MapManager();

		}

		private void Start()
		{
			BaseComponent baseComponent = GameEntry.Base;
            if (baseComponent == null)
            {
                Log.Fatal("Base component is invalid.");
                return;
            }

            m_EventComponent = GameEntry.Event;
            if (m_EventComponent == null)
            {
                Log.Fatal("Event component is invalid.");
                return;
            }

            if (baseComponent.EditorResourceMode)
            {
				m_MapManager.SetResourceManager(baseComponent.EditorResourceHelper);
            }
            else
            {
				m_MapManager.SetResourceManager(GameFrameworkEntry.GetModule<IResourceManager>());
            }

			m_MapManager.SetObjectPoolManager(GameFrameworkEntry.GetModule<IObjectPoolManager>());
			m_MapManager.InstanceAutoReleaseInterval = m_InstanceAutoReleaseInterval;
			m_MapManager.InstanceCapacity = m_InstanceCapacity;
			m_MapManager.InstanceExpireTime = m_InstanceExpireTime;
			m_MapManager.InstancePriority = m_InstancePriority;

			ChunkHelperBase chunkHelper = Helper.CreateHelper(m_ChunkHelperTypeName, m_CustomChunkHelper);
			if (chunkHelper == null)
			{
				Log.Error("Can not create chunk helper.");
				return;
			}

			chunkHelper.name = "Chunk Helper";
			Transform transform = chunkHelper.transform;
			transform.SetParent(this.transform);
			transform.localScale = Vector3.one;

			m_MapManager.SetChunkHelper(chunkHelper);

			if (m_InstanceRoot == null)
			{
				m_InstanceRoot = new GameObject("Chunk Instances").transform;
				m_InstanceRoot.SetParent(gameObject.transform);
				m_InstanceRoot.localScale = Vector3.one;
			}

			for (int i = 0; i < m_ChunkGroups.Length; i++)
			{
				if (!AddChunkGroup(m_ChunkGroups[i].ChunkGroupName))
				{
					Log.Warning("Add Chunk group '{0}' failure.", m_ChunkGroups[i].ChunkGroupName);
					continue;
				}
			}

			StartCoroutine(m_MapManager.SetTile());
		}

		private void Update()
		{
			m_MapManager.Update();
		}

		public bool AddChunkGroup(string chunkGroupName)
		{
			if (m_MapManager.HasChunkGroup(chunkGroupName))
			{
				return false;
			}

			ChunkGroupHelperBase chunkGroupHelper = Helper.CreateHelper(m_ChunkGroupHelperTypeName, m_CustomChunkGroupHelper, ChunkGroupCount);
			if (chunkGroupHelper == null)
			{
				Log.Error("Can not create Chunk group helper.");
				return false;
			}

			chunkGroupHelper.name = Utility.Text.Format("Chunk Group - {0}", chunkGroupName);
			Transform transform = chunkGroupHelper.transform;
			transform.SetParent(m_InstanceRoot);
			transform.localScale = Vector3.one;
			transform.gameObject.AddComponent<Grid>();

			return m_MapManager.AddChunkGroup(chunkGroupName,chunkGroupHelper);
		}

		public void LoadChunk(ChunkType chunkType)
		{
			m_MapManager.LoadChunk(chunkType);
		}

		public void LoadChunk(string chunkAssetName)
		{
			m_MapManager.LoadChunk(chunkAssetName);
		}

		public void EnterChunk(string chunkAssetName)
		{
			m_MapManager.EnterChunk(chunkAssetName, 0, null);
		}

		public void LeaveChunk(IChunk chunk)
		{
			m_MapManager.LeaveChunk(chunk, null);
		}
	}
}
