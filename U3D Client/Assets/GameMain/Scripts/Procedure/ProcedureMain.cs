using GameFramework.Fsm;
using GameFramework.Procedure;

namespace Cherry
{
	public class ProcedureMain : ProcedureBase
	{
		protected override void OnInit(IFsm<IProcedureManager> procedureOwner)
		{
			base.OnInit(procedureOwner);
		}

		protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
		{
			base.OnEnter(procedureOwner);
			UnityGameFramework.Runtime.Log.Info("进入游戏主流程");

			//打开主界面
			GameEntry.UI.OpenUIFormByUIFormType(UIFormType.TestUIForm, null);
			//测试用弹出界面
			//GameEntry.UI.OpenUIForm("Assets/GameMain/AssetData/UI/Image.prefab", "Top");
		}

		protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
		{
			base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
		}

		protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
		{
			base.OnLeave(procedureOwner, isShutdown);
		}

		protected override void OnDestroy(IFsm<IProcedureManager> procedureOwner)
		{
			base.OnDestroy(procedureOwner);
		}
	}
}
