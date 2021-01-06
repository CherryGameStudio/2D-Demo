using UnityEngine;
using Cherry.Tilemaps;
using GameFramework;
using GameFramework.ObjectPool;
using GameFramework.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using GameFramework.DataTable;

namespace Cherry
{
	/// <summary>
	/// 游戏场景地图管理器
	/// </summary>
	public partial class MapManager
	{
		private readonly Dictionary<string, ChunkGroup> m_ChunkGroups;
		private readonly Dictionary<int, string> m_ChunksBeingLoaded;
		private readonly HashSet<int> m_ChunksToReleaseOnLoad;
		private readonly Queue<IChunk> m_RecycleQueue;
		private readonly List<TilemapData> tilemapDatas;
		private readonly LoadAssetCallbacks m_LoadAssetCallbacks;
		private IObjectPoolManager m_ObjectPoolManager;
		private IResourceManager m_ResourceManager;
		private IObjectPool<ChunkInstanceObject> m_InstancePool;
		private IChunkHelper m_ChunkHelper;
		private int m_SerialId;
		[ThreadStatic]
		private List<string> m_DependentAssetNames;
		[ThreadStatic]
		private List<string> m_BeingLoadedAssetNames;
		[ThreadStatic]
		private List<string> m_CachedAssetNames;
		//todo 打开地图的一系列事件
		
		/// <summary>
		/// 初始化地图管理器新实例
		/// </summary>
		public MapManager()
		{
			m_ChunkGroups = new Dictionary<string, ChunkGroup>();
			m_ChunksBeingLoaded = new Dictionary<int, string>();
			m_ChunksToReleaseOnLoad = new HashSet<int>();
			m_RecycleQueue = new Queue<IChunk>();
			tilemapDatas = new List<TilemapData>();
			m_LoadAssetCallbacks = new LoadAssetCallbacks(LoadAssetSuccessCallback,LoadAssetFailureCallback);
			m_ObjectPoolManager = null;
			m_ResourceManager = null;
			m_InstancePool = null;
			m_ChunkHelper = null;
			m_SerialId = 0;
			m_DependentAssetNames = new List<string>();
			m_BeingLoadedAssetNames = new List<string>();
			m_CachedAssetNames = new List<string>();
		}

		public int ChunkGroupCount
		{
			get { return m_ChunkGroups.Count; }
		}

		/// <summary>
		/// 获取或设置地图块实例对象池自动释放可释放对象的间隔秒数。
		/// </summary>
		public float InstanceAutoReleaseInterval
		{
			get
			{
				return m_InstancePool.AutoReleaseInterval;
			}
			set
			{
				m_InstancePool.AutoReleaseInterval = value;
			}
		}

		/// <summary>
		/// 获取或设置地图块实例对象池的容量。
		/// </summary>
		public int InstanceCapacity
		{
			get
			{
				return m_InstancePool.Capacity;
			}
			set
			{
				m_InstancePool.Capacity = value;
			}
		}

		/// <summary>
		/// 获取或设置地图块对象池对象过期秒数。
		/// </summary>
		public float InstanceExpireTime
		{
			get
			{
				return m_InstancePool.ExpireTime;
			}
			set
			{
				m_InstancePool.ExpireTime = value;
			}
		}

		/// <summary>
		/// 获取或设置地图块实例对象池的优先级。
		/// </summary>
		public int InstancePriority
		{
			get
			{
				return m_InstancePool.Priority;
			}
			set
			{
				m_InstancePool.Priority = value;
			}
		}

		public void Update()
		{
			while (m_RecycleQueue.Count>0)
			{
				IChunk chunk = m_RecycleQueue.Dequeue();
				chunk.OnRecycle();
				m_InstancePool.Unspawn(chunk.Handle);
			}

			foreach (KeyValuePair<string,ChunkGroup> chunkGroup in m_ChunkGroups)
			{
				chunkGroup.Value.Update();
			}
		}

		public void Shutdown()
		{
			m_ChunkGroups.Clear();
			m_ChunksBeingLoaded.Clear();
			m_ChunksToReleaseOnLoad.Clear();
			m_RecycleQueue.Clear();
		}

		public void SetObjectPoolManager(IObjectPoolManager objectPoolManager)
		{
			if (objectPoolManager == null)
			{
				throw new GameFrameworkException("objectPoolManager is invaild.");
			}

			m_ObjectPoolManager = objectPoolManager;
			m_InstancePool = m_ObjectPoolManager.CreateSingleSpawnObjectPool<ChunkInstanceObject>("Chunk Instance Pool");
		}

		public void SetResourceManager(IResourceManager resourceManager)
		{
			if (resourceManager == null)
			{
				throw new GameFrameworkException("resourceManager is invaild.");
			}

			m_ResourceManager = resourceManager;
		}

		public void SetChunkHelper(IChunkHelper chunkHelper)
		{
			if (chunkHelper == null)
			{
				throw new GameFrameworkException("chunkHelper is invaild.");
			}

			m_ChunkHelper = chunkHelper;
		}

		public bool HasChunkGroup(string chunkGroupName)
		{
			if (string.IsNullOrEmpty(chunkGroupName))
			{
				throw new GameFrameworkException("chunkGroupName is invaild.");
			}

			return m_ChunkGroups.ContainsKey(chunkGroupName);
		}

		public IChunkGroup GetChunkGroup(string chunkGroupName)
		{
			if (string.IsNullOrEmpty(chunkGroupName))
			{
				throw new GameFrameworkException("chunkGroupName is invaild");
			}

			ChunkGroup chunkGroup;
			if (m_ChunkGroups.TryGetValue(chunkGroupName,out chunkGroup))
			{
				return chunkGroup;
			}

			return null;
		}

		public IChunkGroup[] GetAllChunkGroups()
		{
			int index = 0;
			IChunkGroup[] results = new IChunkGroup[m_ChunkGroups.Count];
			foreach (KeyValuePair<string,ChunkGroup> chunkGroup in m_ChunkGroups)
			{
				results[index++] = chunkGroup.Value;
			}

			return results;
		}

		public void GetAllChunkGroups(List<IChunkGroup> results)
		{
			if (results == null)
			{
				throw new GameFrameworkException("results is invaild.");
			}

			results.Clear();
			foreach (KeyValuePair<string, ChunkGroup> chunkGroup in m_ChunkGroups)
			{
				results.Add(chunkGroup.Value);
			}
		}

		public bool AddChunkGroup(string chunkGroupName, IChunkGroupHelper chunkGroupHelper)
		{
			if (string.IsNullOrEmpty(chunkGroupName))
			{
				throw new GameFrameworkException("Chunk group name is invalid.");
			}

			if (chunkGroupHelper == null)
			{
				throw new GameFrameworkException("chunk group helper is invalid.");
			}

			if (HasChunkGroup(chunkGroupName))
			{
				return false;
			}

			m_ChunkGroups.Add(chunkGroupName, new ChunkGroup(chunkGroupName, chunkGroupHelper));
			return true;
		}

		/// <summary>
		/// 是否存在界面。
		/// </summary>
		/// <param name="serialId">界面序列编号。</param>
		/// <returns>是否存在界面。</returns>
		public bool HasChunk(int serialId)
		{
			foreach (KeyValuePair<string, ChunkGroup> chunkGroup in m_ChunkGroups)
			{
				if (chunkGroup.Value.HasChunk(serialId))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// 是否存在界面。
		/// </summary>
		/// <param name="uiFormAssetName">界面资源名称。</param>
		/// <returns>是否存在界面。</returns>
		public bool HasChunk(string chunkAssetName)
		{
			if (string.IsNullOrEmpty(chunkAssetName))
			{
				throw new GameFrameworkException("chunkAssetName is invalid.");
			}

			foreach (KeyValuePair<string, ChunkGroup> chunkGroup in m_ChunkGroups)
			{
				if (chunkGroup.Value.HasChunk(chunkAssetName))
				{
					return true;
				}
			}

			return false;
		}

		public IChunk GetChunk(int serialId)
		{
			foreach (KeyValuePair<string,ChunkGroup> chunkGroup in m_ChunkGroups)
			{
				if (chunkGroup.Value.HasChunk(serialId))
				{
					return chunkGroup.Value.GetChunk(serialId);
				}
			}

			return null;
		}

		public IChunk GetChunk(string chunkAssetName)
		{
			foreach (KeyValuePair<string, ChunkGroup> chunkGroup in m_ChunkGroups)
			{
				if (chunkGroup.Value.HasChunk(chunkAssetName))
				{
					return chunkGroup.Value.GetChunk(chunkAssetName);
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
			foreach (KeyValuePair<string,ChunkGroup> chunkGroup in m_ChunkGroups)
			{
				results.AddRange(chunkGroup.Value.GetChunks(chunkAssetName));
			}

			return results.ToArray();
		}

		public void GetChunks(string chunkAssetName,List<IChunk> results)
		{
			if (string.IsNullOrWhiteSpace(chunkAssetName))
			{
				throw new GameFrameworkException("chunkAssetName is invaild.");
			}

			if (results == null)
			{
				throw new GameFrameworkException("results is invaild.");
			}

			results.Clear();
			foreach (KeyValuePair<string, ChunkGroup> chunkGroup in m_ChunkGroups)
			{
				results.AddRange(chunkGroup.Value.GetChunks(chunkAssetName));
			}
		}

		public IChunk[] GetAllLoadedChunks()
		{
			List<IChunk> results = new List<IChunk>();
			foreach (KeyValuePair<string, ChunkGroup> chunkGroup in m_ChunkGroups)
			{
				results.AddRange(chunkGroup.Value.GetAllChunks());
			}

			return results.ToArray();
		}

		public void GetAllLoadedChunks(List<IChunk> results)
		{
			if (results == null)
			{
				throw new GameFrameworkException("results is invaild.");
			}

			results.Clear();
			foreach (KeyValuePair<string, ChunkGroup> chunkGroup in m_ChunkGroups)
			{
				results.AddRange(chunkGroup.Value.GetAllChunks());
			}
		}

		public int[] GetAllLoadingChunkSerialId()
		{
			int index = 0;
			int[] results = new int[m_ChunksBeingLoaded.Count];
			foreach (KeyValuePair<int, string> uiFormBeingLoaded in m_ChunksBeingLoaded)
			{
				results[index++] = uiFormBeingLoaded.Key;
			}

			return results;
		}

		public void GetAllLoadingChunkSerialId(List<int> results)
		{
			if (results == null)
			{
				throw new GameFrameworkException("results is invaild.");
			}

			results.Clear();
			foreach (KeyValuePair<int,string> chunkBeingLoaded in m_ChunksBeingLoaded)
			{
				results.Add(chunkBeingLoaded.Key);
			}
		}

		/// <summary>
		/// 是否正在加载界面。
		/// </summary>
		/// <param name="serialId">界面序列编号。</param>
		/// <returns>是否正在加载界面。</returns>
		public bool IsLoadingChunk(int serialId)
		{
			return m_ChunksBeingLoaded.ContainsKey(serialId);
		}

		/// <summary>
		/// 是否正在加载界面。
		/// </summary>
		/// <param name="uiFormAssetName">界面资源名称。</param>
		/// <returns>是否正在加载界面。</returns>
		public bool IsLoadingChunk(string chunkAssetName)
		{
			if (string.IsNullOrEmpty(chunkAssetName))
			{
				throw new GameFrameworkException("chunkAssetName name is invalid.");
			}

			return m_ChunksBeingLoaded.ContainsValue(chunkAssetName);
		}

		public bool IsValidChunk(IChunk chunk)
		{
			if (chunk == null)
			{
				return false;
			}

			return HasChunk(chunk.SerialId);
		}

		public void LoadChunk(ChunkType chunkType)
		{
			string chunkAssetName = GameEntry.DataTable.GetDataTable<DRChunk>().GetDataRow((int)chunkType).ChunkAssetName;
			LoadChunk(chunkAssetName);
		}

		public void LoadChunk(string chunkAssetName)
		{
			if (string.IsNullOrEmpty(chunkAssetName))
			{
				throw new GameFrameworkException("chunk asset name is invaild.");
			}

			int serialId = m_SerialId;
			m_BeingLoadedAssetNames.Clear();
			m_ChunkHelper.GetDependentChunkAssetNames(chunkAssetName, m_BeingLoadedAssetNames);
			//m_DependentAssetNames.Clear();
			//m_ChunkHelper.GetDependentChunkAssetNames(chunkAssetName, m_DependentAssetNames);
			if (InternalGetBeingLoadedAssetNames(m_BeingLoadedAssetNames))
			{
				foreach (string assetName in m_BeingLoadedAssetNames)
				{
					if (!m_ChunksBeingLoaded.ContainsValue(assetName) && !HasChunk(assetName))
					{
						serialId = ++m_SerialId;
						m_ChunksBeingLoaded.Add(serialId, assetName);
						//这边只是加载地图
						m_ResourceManager.LoadAsset(assetName, m_LoadAssetCallbacks, LoadingChunkInfo.Create(serialId, null, false, true));
					}
				}
			}
		}

		public int EnterChunk(ChunkType chunkType,int priority,object userData)
		{
			string chunkAssetName = GameEntry.DataTable.GetDataTable<DRChunk>().GetDataRow((int)chunkType).ChunkAssetName;
			return EnterChunk(chunkAssetName, priority, userData);
		}

		/// <summary>
		/// 进入地图
		/// </summary>
		/// <param name="chunkAssetName"></param>
		/// <param name="chunkGroupName"></param>
		/// <param name="priority"></param>
		/// <param name="userData"></param>
		/// <returns></returns>
		public int EnterChunk(string chunkAssetName, int priority, object userData)
		{
			if (string.IsNullOrEmpty(chunkAssetName))
			{
				throw new GameFrameworkException("chunkAssetName name is invalid.");
			}

			if (m_ResourceManager == null)
			{
				throw new GameFrameworkException("ResourceManager is invalid.");
			}

			if (m_ChunkHelper == null)
			{
				throw new GameFrameworkException("chunkHelper is invalid.");
			}

			int serialId = m_SerialId;
			bool loadedEnterChunk = false;
			//配表地图块资源包括自身，所以添加进筛选队列。
			m_BeingLoadedAssetNames.Clear();
			m_ChunkHelper.GetDependentChunkAssetNames(chunkAssetName, m_BeingLoadedAssetNames);
			m_DependentAssetNames.Clear();
			m_ChunkHelper.GetDependentChunkAssetNames(chunkAssetName, m_DependentAssetNames);
			if (InternalGetBeingLoadedAssetNames(m_BeingLoadedAssetNames))
			{
				foreach (string assetName in m_BeingLoadedAssetNames)
				{
					if (!m_ChunksBeingLoaded.ContainsValue(assetName) && !HasChunk(assetName))
					{
						if (assetName == chunkAssetName)
							loadedEnterChunk = true;
						serialId = ++m_SerialId;
						m_ChunksBeingLoaded.Add(serialId, assetName);
						m_ResourceManager.LoadAsset(assetName, priority, m_LoadAssetCallbacks, LoadingChunkInfo.Create(serialId, userData, assetName == chunkAssetName,false));
					}
				}
			}
			else//无需加载
			{

			}

			foreach (string assetName in m_DependentAssetNames)
			{
				if (m_InstancePool.CanSpawn(assetName))
				{
					ChunkInstanceObject chunkInstanceObject = m_InstancePool.Spawn(assetName);
					m_InstancePool.SetLocked(chunkInstanceObject, true);

					IDataTable<DRChunk> dataTable = GameEntry.DataTable.GetDataTable<DRChunk>();
					DRChunk dataRow = dataTable.GetDataRow((DRChunk m_DataRow) => { return m_DataRow.ChunkAssetName == assetName; });
					ChunkGroup chunkGroup = (ChunkGroup)GetChunkGroup(dataRow.ChunkGroupName);

					IChunk chunk = m_ChunkHelper.CreateChunk(chunkInstanceObject.Target, chunkGroup, userData);
					chunkGroup.AddChunk(chunk);
				}
			}			
			GetChunk(chunkAssetName).OnEnter(userData);
			return serialId;
		}

		/// <summary>
		/// 离开地图
		/// </summary>
		/// <param name="chunk"></param>
		/// <param name="userData"></param>
		public void LeaveChunk(IChunk chunk,object userData)
		{
			if (chunk == null)
			{
				throw new GameFrameworkException("chunk is invalid.");
			}

			foreach (string assetName in chunk.ChunkDependentAssetNames)
			{
				if (assetName == chunk.ChunkAssetName)
				{
					chunk.OnLeave(userData);
				}

				IDataTable<DRChunk> dataTable = GameEntry.DataTable.GetDataTable<DRChunk>();
				DRChunk dataRow = dataTable.GetDataRow((DRChunk m_DataRow) => { return m_DataRow.ChunkAssetName == assetName; });
				ChunkGroup chunkGroup = (ChunkGroup)GetChunkGroup(dataRow.ChunkGroupName);

				IChunk tempChunk = GetChunk(assetName);
				m_InstancePool.SetLocked(tempChunk.Handle, false);
				chunkGroup.RemoveChunk(tempChunk);
				m_RecycleQueue.Enqueue(tempChunk);
			}
			//chunkGroup.Refresh();
		}

		//加载Tile
		public IEnumerator SetTile()
		{
			long lastYieldTime = DateTime.Now.Ticks;
			while (true)
			{
				if (tilemapDatas.Count > 0)
				{
					TilemapData data = tilemapDatas[0];
					Debug.LogWarning(data.TilemapName);
					Tilemap tilemap = GameObject.Find(data.TilemapName).GetComponent<Tilemap>();
					foreach (var item in data.TileInfoList)
					{
						tilemap.SetTile(item.IntPos, item.Tile);
						if (DateTime.Now.Ticks - lastYieldTime > 200000)
						{
							yield return null;
							lastYieldTime = DateTime.Now.Ticks;
						}
					}
					tilemapDatas.RemoveAt(0);
				}
				if (DateTime.Now.Ticks - lastYieldTime > 200000)
				{
					yield return null;
					lastYieldTime = DateTime.Now.Ticks;
				}
			}
		}

		private bool InternalGetBeingLoadedAssetNames(List<string> beingLoadedAssetNames)
		{
			m_CachedAssetNames.Clear();
			bool isNeededLoad = false;
			foreach (string assetName in beingLoadedAssetNames)
			{
				if (m_InstancePool.CanSpawn(assetName))
				{

				}
				else
				{
					m_CachedAssetNames.Add(assetName);
					isNeededLoad = true;
				}
			}
			beingLoadedAssetNames.Clear();
			foreach (string assetName in m_CachedAssetNames)
			{
				beingLoadedAssetNames.Add(assetName);
			}

			return isNeededLoad;
		}

		private void LoadAssetSuccessCallback(string chunkAssetName, object chunkAsset, float duration, object userData)
		{
			LoadingChunkInfo loadingChunkInfo = (LoadingChunkInfo)userData;
			if (loadingChunkInfo == null)
			{
				throw new GameFrameworkException("Open UI form info is invalid.");
			}

			IDataTable<DRChunk> dataTable = GameEntry.DataTable.GetDataTable<DRChunk>();
			DRChunk dataRow = dataTable.GetDataRow((DRChunk m_DataRow) => { return m_DataRow.ChunkAssetName == chunkAssetName; });
			ChunkGroup group = (ChunkGroup)GetChunkGroup(dataRow.ChunkGroupName);

			if (m_ChunksToReleaseOnLoad.Contains(loadingChunkInfo.SerialId))
			{
				m_ChunksToReleaseOnLoad.Remove(loadingChunkInfo.SerialId);
				ReferencePool.Release(loadingChunkInfo);
				m_ChunkHelper.ReleaseChunk(chunkAsset, null);
				return;
			}

			m_ChunksBeingLoaded.Remove(loadingChunkInfo.SerialId);
			//加载地图成功后，不管如何都是注册进对象池且使用，直到卸载才会回收入对象池
			ChunkInstanceObject chunkInstanceObject = ChunkInstanceObject.Create(chunkAssetName, chunkAsset, m_ChunkHelper.InstantiateChunk(chunkAsset), m_ChunkHelper);
			m_InstancePool.Register(chunkInstanceObject, true);

			//加载地图成功后，初始化地图块相关信息，而不是在进入时初始化
			m_DependentAssetNames.Clear();
			m_ChunkHelper.GetDependentChunkAssetNames(chunkAssetName, m_DependentAssetNames);
			IChunk chunk = m_ChunkHelper.CreateChunk(chunkInstanceObject.Target, group, userData);
			chunk.OnInit(loadingChunkInfo.SerialId, chunkAssetName, m_DependentAssetNames, group, true, userData);
			group.AddChunk(chunk);
			//loadingChunkInfo.ChunkGroup.Refresh();
			ReferencePool.Release(loadingChunkInfo);

			tilemapDatas.Add(chunkAsset as TilemapData);
		}

		private void LoadAssetFailureCallback(string chunkAssetName, LoadResourceStatus status, string errorMessage, object userData)
		{
			LoadingChunkInfo loadingChunkInfo = (LoadingChunkInfo)userData;
			if (loadingChunkInfo == null)
			{
				throw new GameFrameworkException("Open UI form info is invalid.");
			}

			if (m_ChunksToReleaseOnLoad.Contains(loadingChunkInfo.SerialId))
			{
				m_ChunksToReleaseOnLoad.Remove(loadingChunkInfo.SerialId);
				return;
			}

			m_ChunksBeingLoaded.Remove(loadingChunkInfo.SerialId);
			string appendErrorMessage = Utility.Text.Format("Load Chunk failure, asset name '{0}', status '{1}', error message '{2}'.", chunkAssetName, status.ToString(), errorMessage);

			throw new GameFrameworkException(appendErrorMessage);
		}
	}
}
