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
		private readonly List<ChunkParams> m_BeingSetTilemapDatas;
		private readonly LoadAssetCallbacks m_LoadAssetCallbacks;
		private IObjectPoolManager m_ObjectPoolManager;
		private IResourceManager m_ResourceManager;
		private IObjectPool<ChunkInstanceObject> m_InstancePool;
		private IChunkHelper m_ChunkHelper;
		private int m_ChunkId;

		//上层缓存容器
		private Dictionary<string,int> m_ChunkName2IdDict;
		private List<int> m_BeingLoadedChunkIds;
		private List<string> m_DependentAssetNames;
		private List<string> m_BeingLoadedAssetNames;
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
			m_BeingSetTilemapDatas = new List<ChunkParams>();
			m_LoadAssetCallbacks = new LoadAssetCallbacks(LoadAssetSuccessCallback,LoadAssetFailureCallback);
			m_ObjectPoolManager = null;
			m_ResourceManager = null;
			m_InstancePool = null;
			m_ChunkHelper = null;
			m_ChunkId = 0;
			m_ChunkName2IdDict = new Dictionary<string, int>();
			m_BeingLoadedChunkIds = new List<int>();
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

		public IChunk GetChunk(int chunkId)
		{
			foreach (KeyValuePair<string,ChunkGroup> chunkGroup in m_ChunkGroups)
			{
				if (chunkGroup.Value.HasChunk(chunkId))
				{
					return chunkGroup.Value.GetChunk(chunkId);
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

		public int[] GetAllLoadingChunkId()
		{
			int index = 0;
			int[] results = new int[m_ChunksBeingLoaded.Count];
			foreach (KeyValuePair<int, string> uiFormBeingLoaded in m_ChunksBeingLoaded)
			{
				results[index++] = uiFormBeingLoaded.Key;
			}

			return results;
		}

		public void GetAllLoadingChunkId(List<int> results)
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
		/// <param name="chunkId">界面序列编号。</param>
		/// <returns>是否正在加载界面。</returns>
		public bool IsLoadingChunk(int chunkId)
		{
			return m_ChunksBeingLoaded.ContainsKey(chunkId);
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

			return HasChunk(chunk.ChunkId);
		}

		public void LoadChunk(ChunkType chunkType , object userData = null)
		{
			string chunkAssetName = GameEntry.DataTable.GetDataTable<DRChunk>().GetDataRow((int)chunkType).ChunkAssetName;
			if (string.IsNullOrEmpty(chunkAssetName))
			{
				throw new GameFrameworkException("chunkType name is invaild.");
			}

			m_BeingLoadedAssetNames.Clear();
			m_BeingLoadedChunkIds.Clear();
			m_ChunkHelper.GetDependentChunkIds((int)chunkType,m_BeingLoadedChunkIds);
			if (InternalGetBeingLoadedAssetNames(m_BeingLoadedChunkIds,m_BeingLoadedAssetNames))
			{
				foreach (string assetName in m_BeingLoadedAssetNames)
				{
					if (!m_ChunksBeingLoaded.ContainsValue(assetName) && !HasChunk(assetName))
					{
						ChunkParams param = ReferencePool.Acquire<ChunkParams>();
						if (m_ChunkName2IdDict.TryGetValue(assetName, out int dataRowId))
						{
							m_ChunksBeingLoaded.Add(dataRowId, assetName);
							param.ChunkData = GameEntry.DataTable.GetDataTable<DRChunk>().GetDataRow(dataRowId);
						}
						else//保险起见，基本不会else中的情况，else中读表的复杂度为O(N)，if中为O(log2N)
						{
							DRChunk chunkData = GameEntry.DataTable.GetDataTable<DRChunk>().GetDataRow((DRChunk chunkdata) => { return chunkdata.ChunkAssetName == assetName; });
							param.ChunkData = chunkData;
							dataRowId = chunkData.Id;
							m_ChunksBeingLoaded.Add(dataRowId, assetName);
							m_ChunkName2IdDict.Add(chunkData.ChunkAssetName, chunkData.Id);
						}
						//这边只是加载地图，且需区分加载地图本身和其依赖
						if (chunkAssetName == assetName)
						{
							param.UserData = userData;
							m_ResourceManager.LoadAsset(assetName, m_LoadAssetCallbacks, LoadingChunkInfo.Create(dataRowId, param, false, true));
						}
						else
						{
							m_ResourceManager.LoadAsset(assetName, m_LoadAssetCallbacks, LoadingChunkInfo.Create(dataRowId, param, false, true));
						}
					}
				}
			}

		}

		public void EnterChunk(ChunkType chunkType,int priority,object userData)
		{
			string chunkAssetName = GameEntry.DataTable.GetDataTable<DRChunk>().GetDataRow((int)chunkType).ChunkAssetName;
			if (string.IsNullOrEmpty(chunkAssetName))
			{
				throw new GameFrameworkException("chunkType name is invalid.");
			}

			if (m_ResourceManager == null)
			{
				throw new GameFrameworkException("ResourceManager is invalid.");
			}

			if (m_ChunkHelper == null)
			{
				throw new GameFrameworkException("chunkHelper is invalid.");
			}

			m_BeingLoadedAssetNames.Clear();
			m_BeingLoadedChunkIds.Clear();
			m_ChunkHelper.GetDependentChunkIds((int)chunkType, m_BeingLoadedChunkIds);
			if (InternalGetBeingLoadedAssetNames(m_BeingLoadedChunkIds, m_BeingLoadedAssetNames))
			{
				foreach (string assetName in m_BeingLoadedAssetNames)
				{
					if (!m_ChunksBeingLoaded.ContainsValue(assetName) && !HasChunk(assetName))
					{
						ChunkParams param = ReferencePool.Acquire<ChunkParams>();
						if (m_ChunkName2IdDict.TryGetValue(assetName, out int dataRowId))
						{
							m_ChunksBeingLoaded.Add(dataRowId, assetName);
							param.ChunkData = GameEntry.DataTable.GetDataTable<DRChunk>().GetDataRow(dataRowId);
						}
						else//保险起见，基本不会有else中的情况，else中读表的复杂度为O(N)，if中为O(log2N)
						{
							DRChunk chunkData = GameEntry.DataTable.GetDataTable<DRChunk>().GetDataRow((DRChunk chunkdata) => { return chunkdata.ChunkAssetName == assetName; });
							param.ChunkData = chunkData;
							dataRowId = chunkData.Id;
							m_ChunksBeingLoaded.Add(dataRowId, assetName);
							m_ChunkName2IdDict.Add(chunkData.ChunkAssetName, chunkData.Id);
						}
						if (chunkAssetName == assetName)
						{
							param.UserData = userData;
							m_ResourceManager.LoadAsset(assetName, m_LoadAssetCallbacks, LoadingChunkInfo.Create(dataRowId, param, false, true));
						}
						else
						{
							m_ResourceManager.LoadAsset(assetName, m_LoadAssetCallbacks, LoadingChunkInfo.Create(dataRowId, param, false, true));
						}
					}
				}
			}

			m_DependentAssetNames.Clear();
			m_ChunkHelper.GetDependentChunkAssetNames(chunkAssetName, m_DependentAssetNames);
			foreach (string assetName in m_DependentAssetNames)
			{
				if (m_InstancePool.CanSpawn(assetName))
				{
					ChunkInstanceObject chunkInstanceObject = m_InstancePool.Spawn(assetName);
					m_InstancePool.SetLocked(chunkInstanceObject, true);

					//保险起见，基本进入if中的情况，if中读表的复杂度为O(N)，不进入为O(log2N)
					if (!m_ChunkName2IdDict.TryGetValue(assetName, out int dataRowId))
					{
						DRChunk chunkData = GameEntry.DataTable.GetDataTable<DRChunk>().GetDataRow((DRChunk chunkdata) => { return chunkdata.ChunkAssetName == assetName; });
						dataRowId = chunkData.Id;
						m_ChunkName2IdDict.Add(chunkData.ChunkAssetName, chunkData.Id);
					}

					DRChunk dataRow = GameEntry.DataTable.GetDataTable<DRChunk>().GetDataRow(dataRowId);
					ChunkGroup tempChunkGroup = GetChunkGroup(dataRow.ChunkGroupName) as ChunkGroup;
					IChunk tempChunk = m_ChunkHelper.CreateChunk(chunkInstanceObject.Target, tempChunkGroup, userData);
					ChunkParams param = ReferencePool.Acquire<ChunkParams>();
					param.ChunkData = dataRow;
					param.UserData = userData;
					param.TilemapData = chunkInstanceObject.ChunkAsset as TilemapData;

					tempChunk.OnInit(dataRow.Id, assetName, m_DependentAssetNames, tempChunkGroup, false, param);
					tempChunkGroup.AddChunk(tempChunk);
					if (chunkAssetName == assetName)
					{
						tempChunk.OnEnter(param);
					}
				}
			}
		}

		/// <summary>
		/// 进入地图
		/// </summary>
		/// <param name="chunkAssetName"></param>
		/// <param name="chunkGroupName"></param>
		/// <param name="priority"></param>
		/// <param name="userData"></param>
		/// <returns></returns>
		[Obsolete]
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

			int serialId = m_ChunkId;
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
						serialId = ++m_ChunkId;
						m_ChunksBeingLoaded.Add(serialId, assetName);
						m_ResourceManager.LoadAsset(assetName, priority, m_LoadAssetCallbacks, LoadingChunkInfo.Create(serialId, userData, assetName == chunkAssetName,false));
					}
				}
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
					IChunk tempChunk = m_ChunkHelper.CreateChunk(chunkInstanceObject.Target, chunkGroup, userData);
					tempChunk.OnInit(dataRow.Id, assetName, m_DependentAssetNames, chunkGroup, false, null);
					chunkGroup.AddChunk(tempChunk);
				}
			}
			Chunk chunk = GetChunk(chunkAssetName) as Chunk;
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
				if (m_BeingSetTilemapDatas.Count > 0)
				{
					TilemapData data = m_BeingSetTilemapDatas[0].TilemapData;
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
					(GetChunk(m_BeingSetTilemapDatas[0].ChunkData.ChunkAssetName) as Chunk).IsReady = true;
					m_BeingSetTilemapDatas.RemoveAt(0);
				}
				if (DateTime.Now.Ticks - lastYieldTime > 200000)
				{
					yield return null;
					lastYieldTime = DateTime.Now.Ticks;
				}
			}
		}

		[Obsolete]
		private bool InternalGetBeingLoadedAssetNames(List<string> beingLoadedAssetNames)
		{
			m_CachedAssetNames.Clear();
			bool isNeededLoad = false;
			foreach (string assetName in beingLoadedAssetNames)
			{
				if (!m_InstancePool.CanSpawn(assetName))
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

		private bool InternalGetBeingLoadedAssetNames(List<int> beingLoadedChunkIds, List<string> beingLoadedAssetNames)
		{
			beingLoadedAssetNames.Clear();
			foreach (int id in beingLoadedChunkIds)
			{
				DRChunk datarow = GameEntry.DataTable.GetDataTable<DRChunk>().GetDataRow(id);
				beingLoadedAssetNames.Add(datarow.ChunkAssetName);
				if (!m_ChunkName2IdDict.ContainsValue(id))
				{
					m_ChunkName2IdDict.Add(datarow.ChunkAssetName,id);
				}
			}

			bool isNeededLoad = false;
			foreach (string assetName in beingLoadedAssetNames)
			{
				if (!m_InstancePool.CanSpawn(assetName))
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
			LoadingChunkInfo loadingChunkInfo = userData as LoadingChunkInfo;
			ChunkParams param = loadingChunkInfo.UserData as ChunkParams;
			param.TilemapData = chunkAsset as TilemapData;

			if (loadingChunkInfo == null)
			{
				throw new GameFrameworkException("Open UI form info is invalid.");
			}

			ChunkGroup group = (ChunkGroup)GetChunkGroup(param.ChunkData.ChunkGroupName);

			if (m_ChunksToReleaseOnLoad.Contains(loadingChunkInfo.ChunkId))
			{
				m_ChunksToReleaseOnLoad.Remove(loadingChunkInfo.ChunkId);
				ReferencePool.Release(loadingChunkInfo);
				m_ChunkHelper.ReleaseChunk(chunkAsset, null);
				return;
			}

			m_ChunksBeingLoaded.Remove(loadingChunkInfo.ChunkId);
			//加载地图成功后，不管如何都是注册进对象池且使用，直到卸载才会回收入对象池
			ChunkInstanceObject chunkInstanceObject = ChunkInstanceObject.Create(chunkAssetName, chunkAsset, m_ChunkHelper.InstantiateChunk(chunkAsset), m_ChunkHelper);
			m_InstancePool.Register(chunkInstanceObject, true);

			//加载地图成功后，初始化地图块相关信息，而不是在进入时初始化
			m_DependentAssetNames.Clear();
			m_ChunkHelper.GetDependentChunkAssetNames(chunkAssetName, m_DependentAssetNames);
			IChunk chunk = m_ChunkHelper.CreateChunk(chunkInstanceObject.Target, group, param);
			chunk.OnInit(loadingChunkInfo.ChunkId, chunkAssetName, m_DependentAssetNames, group, true, param);
			group.AddChunk(chunk);
			//loadingChunkInfo.ChunkGroup.Refresh();
			ReferencePool.Release(loadingChunkInfo);

			m_BeingSetTilemapDatas.Add(param);
		}

		private void LoadAssetFailureCallback(string chunkAssetName, LoadResourceStatus status, string errorMessage, object userData)
		{
			LoadingChunkInfo loadingChunkInfo = (LoadingChunkInfo)userData;
			if (loadingChunkInfo == null)
			{
				throw new GameFrameworkException("Open UI form info is invalid.");
			}

			if (m_ChunksToReleaseOnLoad.Contains(loadingChunkInfo.ChunkId))
			{
				m_ChunksToReleaseOnLoad.Remove(loadingChunkInfo.ChunkId);
				return;
			}

			m_ChunksBeingLoaded.Remove(loadingChunkInfo.ChunkId);
			string appendErrorMessage = Utility.Text.Format("Load Chunk failure, asset name '{0}', status '{1}', error message '{2}'.", chunkAssetName, status.ToString(), errorMessage);

			throw new GameFrameworkException(appendErrorMessage);
		}
	}
}
