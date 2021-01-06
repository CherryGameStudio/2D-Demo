using GameFramework.Event;

namespace Cherry
{
	/// <summary>
	/// 公共事件参数
	/// </summary>
	public class CommonEventArgs : GameEventArgs
	{
		public static readonly int EventId = typeof(CommonEventArgs).GetHashCode();
		public override int Id
		{
			get { return EventId; }
		}

		public int EventType
		{
			get;
			private set;
		}

		public object UserData1;
		public object UserData2;
		public object UserData3;


		public override void Clear()
		{

		}

		public void Fill(int eventType,object userdata)
		{
			EventType = eventType;
			UserData1 = userdata;
		}

		public void Fill(int eventType, object userdata1,object userdata2)
		{
			EventType = eventType;
			UserData1 = userdata1;
			UserData2 = userdata2;
		}

		public void Fill(int eventType, object userdata1, object userdata2,object userdata3)
		{
			EventType = eventType;
			UserData1 = userdata1;
			UserData2 = userdata2;
			UserData3 = userdata3;
		}
	}
}
