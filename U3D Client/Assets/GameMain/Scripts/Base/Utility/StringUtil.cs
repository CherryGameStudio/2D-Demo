using System.Text;

namespace Cherry.Util
{
	/// <summary>
	/// 字符串操作工具类
	/// </summary>
	public static class StringUtil
	{
		private static StringBuilder m_StringBuilder = new StringBuilder(1024);

		public static string Concat(string str1,string str2)
		{
			m_StringBuilder.Length = 0;
			m_StringBuilder.Append(str1);
			m_StringBuilder.Append(str2);
			return m_StringBuilder.ToString();
		}

		public static string Concat(string str1, string str2,string str3)
		{
			m_StringBuilder.Length = 0;
			m_StringBuilder.Append(str1);
			m_StringBuilder.Append(str2);
			m_StringBuilder.Append(str3);
			return m_StringBuilder.ToString();
		}

		public static string Concat(string str1, string str2,string str3,string str4)
		{
			m_StringBuilder.Length = 0;
			m_StringBuilder.Append(str1);
			m_StringBuilder.Append(str2);
			m_StringBuilder.Append(str3);
			m_StringBuilder.Append(str4);
			return m_StringBuilder.ToString();
		}

		/// <summary>
		/// 生成一行字符串
		/// </summary>
		/// <param name="text">内容</param>
		/// <param name="tabCount">制表符数量</param>
		/// <returns></returns>
		public static string LineText(string text,int tabCount = 0)
		{
			m_StringBuilder.Length = 0;
			for (int i = 0; i < tabCount; i++)
			{
				m_StringBuilder.Append("\t");
			}
			m_StringBuilder.Append(text);
			m_StringBuilder.Append("\n");
			return m_StringBuilder.ToString();
		}
	}
}
