using GameFramework;

namespace Cherry
{
	public class UIFormParams : IReference
	{
		/// <summary>
		/// UIForm表数据
		/// </summary>
		public DRUIForm UIData;

		/// <summary>
		/// 玩家自定义数据
		/// </summary>
		public object UserData;

		public void Clear()
		{
			UserData = default;
			UIData = default;
		}
	}
}
