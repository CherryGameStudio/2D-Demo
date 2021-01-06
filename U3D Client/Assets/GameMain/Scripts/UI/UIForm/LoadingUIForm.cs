using GameFramework.Event;
using UnityGameFramework.Runtime;

namespace Cherry
{
	public class LoadingUIForm : UGUIFormBase
	{
		protected override void OnInit(object userData)
		{
			base.OnInit(userData);
		}

		protected override void OnOpen(object userData)
		{
			base.OnOpen(userData);
			if (userData.ToString() == "init")
			{
				//初始化加载界面对象不回收
				InternalSetVisible(false);
				GameEntry.UI.SetUIFormInstanceLocked(this.UIForm, true);
				GameEntry.UI.CloseUIForm(this.UIForm);
			}
			else
			{
				GameEntry.Event.Subscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
			}
			GLogger.Debug(Log_Channel.UI, "打开loading界面");
		}

		protected override void OnClose(bool isShutdown, object userData)
		{
			base.OnClose(isShutdown, userData);
			GLogger.Debug(Log_Channel.UI, "关闭loading界面");
		}

		private void OnLoadSceneSuccess(object sender, GameEventArgs e)
		{
			GameEntry.UI.CloseUIFormByUIForm(this);
			GameEntry.Event.Unsubscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
		}
	}
}
