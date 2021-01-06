using UnityEngine;
using UnityGameFramework.Runtime;

namespace Cherry
{
	/// <summary>
	/// UGUI界面逻辑基类
	/// </summary>
	[DisallowMultipleComponent]
	public class UGUIFormBase : UIFormLogic
	{
		protected override void OnInit(object userData)
		{
			base.OnInit(userData);
		}

		protected override void OnRecycle()
		{
			base.OnRecycle();
		}

		protected override void OnOpen(object userData)
		{
			base.OnOpen(userData);
		}

		protected override void OnClose(bool isShutdown, object userData)
		{
			base.OnClose(isShutdown, userData);
		}

		protected override void OnPause()
		{
			base.OnPause();
		}

		protected override void OnResume()
		{
			base.OnResume();
		}

		protected override void OnCover()
		{
			base.OnCover();
		}

		protected override void OnReveal()
		{
			base.OnReveal();
		}

		protected override void OnRefocus(object userData)
		{
			base.OnRefocus(userData);
		}

		protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
		{
			base.OnUpdate(elapseSeconds, realElapseSeconds);
		}

		protected override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
		{
			base.OnDepthChanged(uiGroupDepth, depthInUIGroup);
		}

		protected override void InternalSetVisible(bool visible)
		{
			base.InternalSetVisible(visible);
		}

		#region 事件注册
		public virtual void OnClick(GameObject gameObject)
		{

		}
		#endregion
	}
}
