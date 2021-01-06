using GameFramework.Fsm;
using GameFramework.Procedure;

namespace Cherry
{
    public class ProcedureLaunch : ProcedureBase
    {
		protected override void OnInit(IFsm<IProcedureManager> procedureOwner)
		{
			base.OnInit(procedureOwner);
		}

		protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
		{
			base.OnEnter(procedureOwner);
			UnityGameFramework.Runtime.Log.Info("进入游戏启动流程");

			//全局配置文件初始化

			//读取玩家本地数据

			ChangeState<ProcedureInit>(procedureOwner);
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
