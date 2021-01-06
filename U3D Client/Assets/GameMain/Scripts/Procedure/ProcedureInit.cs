using GameFramework.Fsm;
using GameFramework.Procedure;

namespace Cherry
{
	public class ProcedureInit : ProcedureBase
	{
		protected override void OnInit(IFsm<IProcedureManager> procedureOwner)
		{
			base.OnInit(procedureOwner);
		}

		protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
		{
			base.OnEnter(procedureOwner);
			UnityGameFramework.Runtime.Log.Info("进入游戏初始化流程");

			//资源初始化
			if (GameEntry.Base.EditorResourceMode)
			{
				m_ResourcesInit = true;
			}
			else
			{
				GameEntry.Resource.InitResources(OnInitResourcesComplete);
			}

			//全局配置文件初始化

			//读取玩家本地数据



			
		}

		protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
		{
			base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
			if (m_ResourcesInit == true)
			{
				ChangeState<ProcedurePreload>(procedureOwner);
			}
		}

		protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
		{
			base.OnLeave(procedureOwner, isShutdown);
		}

		protected override void OnDestroy(IFsm<IProcedureManager> procedureOwner)
		{
			base.OnDestroy(procedureOwner);
		}

		private bool m_ResourcesInit = false;
		private void OnInitResourcesComplete()
		{
			m_ResourcesInit = true;
			UnityGameFramework.Runtime.Log.Info("资源加载完成");
		}
	}
}
