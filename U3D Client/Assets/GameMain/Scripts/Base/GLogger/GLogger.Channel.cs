namespace Cherry
{
	public enum Log_Channel
	{
		None = 0,
		Log,
		Warning,
		Error,
		Resource,
		Bundle,
		Effect,
		UI,
		Animation,
		DataTable,
		Chunk,
	}

	public enum ELogType
	{
		Debug,
		Info,
		Warning,
		Error,
		Fatal,
	}

	public static partial class GLogger
	{
		private static string[] m_LogChannelStr =
		{
			"--[None]--",
			"--[Log]--",
			"--[Warning]--",
			"--[Error]--",
			"--[Resource]--",
			"--[Bundle]--",
			"--[Effect]--",
			"--[UI]--",
			"--[Animation]--",
			"--[DataTable]--",
			"--[Chunk]--",
		};
	}
}
