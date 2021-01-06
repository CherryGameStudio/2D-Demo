using System;
using UnityGameFramework.Runtime;
using UnityGameFrameworkEntry = UnityGameFramework.Runtime.GameEntry;

namespace Cherry
{
    /// <summary>
    /// 游戏入口
    /// </summary>
    public sealed partial class GameEntry
    {
		public static BaseComponent Base
		{
			get { return UnityGameFrameworkEntry.GetComponent<BaseComponent>(); }
		}

		public static ConfigComponent Config
		{
			get { return UnityGameFrameworkEntry.GetComponent<ConfigComponent>(); }
		}

		public static DataNodeComponent DataNode
		{
			get { return UnityGameFrameworkEntry.GetComponent<DataNodeComponent>(); }
		}

		public static DataTableComponent DataTable
		{
			get { return UnityGameFrameworkEntry.GetComponent<DataTableComponent>(); }
		}

		public static DebuggerComponent Debugger
		{
			get { return UnityGameFrameworkEntry.GetComponent<DebuggerComponent>(); }
		}

		public static DownloadComponent Download
		{
			get { return UnityGameFrameworkEntry.GetComponent<DownloadComponent>(); }
		}

		public static EntityComponent Entity
		{
			get { return UnityGameFrameworkEntry.GetComponent<EntityComponent>(); }
		}

		public static EventComponent Event
		{
			get { return UnityGameFrameworkEntry.GetComponent<EventComponent>(); }
		}

		public static FileSystemComponent FileSystem
		{
			get { return UnityGameFrameworkEntry.GetComponent<FileSystemComponent>(); }
		}

		public static FsmComponent Fsm
		{
			get { return UnityGameFrameworkEntry.GetComponent<FsmComponent>(); }
		}

		public static LocalizationComponent Localization
		{
			get { return UnityGameFrameworkEntry.GetComponent<LocalizationComponent>(); }
		}

		public static NetworkComponent Network
		{
			get { return UnityGameFrameworkEntry.GetComponent<NetworkComponent>(); }
		}

		public static ObjectPoolComponent ObjectPool
		{
			get { return UnityGameFrameworkEntry.GetComponent<ObjectPoolComponent>(); }
		}

		public static ProcedureComponent Procedure
		{
			get { return UnityGameFrameworkEntry.GetComponent<ProcedureComponent>(); }
		}

		public static ResourceComponent Resource
		{
			get { return UnityGameFrameworkEntry.GetComponent<ResourceComponent>(); }
		}

		public static SceneComponent Scene
		{
			get { return UnityGameFrameworkEntry.GetComponent<SceneComponent>(); }
		}

		public static SettingComponent Setting
		{
			get { return UnityGameFrameworkEntry.GetComponent<SettingComponent>(); }
		}

		public static SoundComponent Sound
		{
			get { return UnityGameFrameworkEntry.GetComponent<SoundComponent>(); }
		}

		public static UIComponent UI
		{
			get { return UnityGameFrameworkEntry.GetComponent<UIComponent>(); }
		}

		internal static T GetComponent<T>()
		{
			throw new NotImplementedException();
		}
	}
}
