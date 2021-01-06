using GameFramework.Event;
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityGameFramework.Runtime;

namespace Cherry
{
	public class ProcedurePreload : ProcedureBase
	{
		protected override void OnInit(IFsm<IProcedureManager> procedureOwner)
		{
			base.OnInit(procedureOwner);
		}

		protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
		{
			base.OnEnter(procedureOwner);
			UnityGameFramework.Runtime.Log.Info("进入游戏预加载资源流程");
			GameEntry.Event.Subscribe(LoadDataTableSuccessEventArgs.EventId, OnLoadDataTableSuccess);
			GameEntry.Event.Subscribe(OpenUIFormSuccessEventArgs.EventId, OnLoadPreloadUIAssetSuccess);

			//加载数据表
			GameEntry.DataTable.LoadAllDataTable();

			//加载预加载资源文件
			//预加载loadingUI
			GameEntry.UI.OpenUIForm("Assets/GameMain/AssetData/UI/LoadingUIForm.prefab", "Top", false, "init");
		}

		
		protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
		{
			base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
			int dataTableCount = GameEntry.DataTable.GetDataTable<DRDataTableList>().Count + 1;
			if (dataTableCount == loadedDataTableCount && loadedUIAsset == true)
			{
				loadedDataTableCount = default;
				ChangeState<ProcedureMain>(procedureOwner);
			}
		}

		protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
		{
			base.OnLeave(procedureOwner, isShutdown);
			GameEntry.Event.Unsubscribe(LoadDataTableSuccessEventArgs.EventId, OnLoadDataTableSuccess);
			GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnLoadPreloadUIAssetSuccess);
		}

		protected override void OnDestroy(IFsm<IProcedureManager> procedureOwner)
		{
			base.OnDestroy(procedureOwner);
		}

		private int loadedDataTableCount = 0;
		//数据表加载完成回调
		private void OnLoadDataTableSuccess(object sender, GameEventArgs e)
		{
			loadedDataTableCount++;
		}

		private bool loadedUIAsset = false; 
		//预加载UI加载完成回调
		private void OnLoadPreloadUIAssetSuccess(object sender,GameEventArgs e)
		{
			loadedUIAsset = true;
		}
	}
}
