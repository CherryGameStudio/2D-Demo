using UnityGameFramework.Runtime;
using GameFramework;

namespace Cherry
{
	/// <summary>
	/// UIComponent扩展类 
	/// </summary>
	public static class UIExtension
	{
		public static void OpenUIFormByUIFormType(this UIComponent component,UIFormType type, object userdata = null) 
		{
			DRUIForm formData = GameEntry.DataTable.GetDataTable<DRUIForm>().GetDataRow((int)type);
			if (formData == null)
			{
				GLogger.ErrorFormat(Log_Channel.UI, "{0}类型UIForm没有配置对应数据表！", type.ToString());
				return;
			}

			UIFormParams formParams = ReferencePool.Acquire<UIFormParams>();
			formParams.UIData = formData;
			formParams.UserData = userdata;

			//暂不考虑UI资源加载的优先级,资源暂时这么存放
			component.OpenUIForm("Assets/GameMain/AssetData/UI/" + formData.AssetName, formData.UIGroupName, formData.IsPauseOtherUIForm, formParams);
		}

		public static void CloseUIFormByUIForm(this UIComponent component,UGUIFormBase form,object userdata = null)
		{
			component.CloseUIForm(form.UIForm, userdata);
		}
	}
}
