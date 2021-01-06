using GameFramework.Event;
using UnityGameFramework.Runtime;
using Cherry.Util;

namespace Cherry
{
	public static class SceneExtension
	{
		private const string SceneAssetPath = "Assets/GameMain/Scenes/";
		public static void LoadSceneBySceneType(this SceneComponent component ,SceneType type, object userdata = null)
		{
			DRScene datarow = GameEntry.DataTable.GetDataTable<DRScene>().GetDataRow((int)type);
			GameEntry.UI.OpenUIFormByUIFormType(UIFormType.LoadingUIForm);
			component.LoadScene(StringUtil.Concat(SceneAssetPath, datarow.SceneAssetName));
		}

		private static void OnLoadingUILoadSuccess(object sender,GameEventArgs e)
		{

		}
	}
}
