using System.Text;
using System.IO;
using GameFrameworkLog = UnityGameFramework.Runtime.Log;
using UnityEngine;

namespace Cherry
{
	/// <summary>
	/// 游戏全局日志配送者
	/// 
	/// TODO
	/// 目前有bug，在编辑器模式上对txt文件的输入有问题。
	/// 且线程安全上，没有使用队列+锁的形式去控制。
	/// </summary>
	public static partial class GLogger
	{
		public static bool IsEnable { get; set; } = true;
		public static bool PrintLogToConsole { get; set; } = true;
		public static bool PrintLogToUnityLog { get; set; } = true;
		public static bool EnableFileLog { get; set; } = true;

		const string LogFilePath = "Assets/GameMain/Config";
		const string LogFileName = "Log.txt";
		const string PreLogFileName = "PreLog.txt";
		const string FileLineEnding = "\r\n";

		private static char[] m_CharArray = new char[128];
		private static StringBuilder m_SB = null;
		private static StringBuilder m_FormatSB = null;
		private static StreamWriter m_Writer;

		static GLogger()
		{
#if UNITY_EDITOR
			EnableFileLog = true;
			PrintLogToConsole = true;
			PrintLogToUnityLog = true;
#else
			EnableFileLog = true;
			PrintLogToConsole = true;
			PrintLogToUnityLog = false;
#endif
			m_SB = new StringBuilder(100);
			m_FormatSB = new StringBuilder(100);
			string filePath = Path.Combine(LogFilePath, LogFileName);
			string preFilePath = Path.Combine(LogFilePath, PreLogFileName);
			try
			{
				if (File.Exists(preFilePath))
					File.Delete(preFilePath);
				if (File.Exists(filePath))
					File.Copy(filePath, preFilePath);
				m_Writer = File.CreateText(filePath);
			}
			catch (System.Exception)
			{

				throw;
			}
		}


		#region 外部接口
		public static void Debug(ELogType type, Log_Channel channel, string msg)
		{
			OutPutMsg(type, channel, msg, null);
		}

		public static void Debug(Log_Channel channel, string msg)
		{
			OutPutMsg(ELogType.Debug, channel, msg, null);
		}

		public static void DebugFormat(Log_Channel channel,string msg,params object[] formats)
		{
			m_FormatSB.Clear();
			m_FormatSB.AppendFormat(msg, formats);
			OutPutMsg(ELogType.Debug, channel, m_FormatSB.ToString(), null);
		}

		public static void Info(Log_Channel channel, string msg)
		{
			OutPutMsg(ELogType.Info, channel, msg, null);
		}

		public static void InfoFormat(Log_Channel channel, string msg, params object[] formats)
		{
			m_FormatSB.Clear();
			m_FormatSB.AppendFormat(msg, formats);
			OutPutMsg(ELogType.Info, channel, m_FormatSB.ToString(), null);
		}

		public static void Warning(Log_Channel channel, string msg)
		{
			OutPutMsg(ELogType.Warning, channel, msg, null);
		}

		public static void WarningFormat(Log_Channel channel, string msg, params object[] formats)
		{
			m_FormatSB.Clear();
			m_FormatSB.AppendFormat(msg, formats);
			OutPutMsg(ELogType.Warning, channel, m_FormatSB.ToString(), null);
		}

		public static void Error(Log_Channel channel, string msg)
		{
			OutPutMsg(ELogType.Error, channel, msg, null);
		}

		public static void ErrorFormat(Log_Channel channel, string msg, params object[] formats)
		{
			m_FormatSB.Clear();
			m_FormatSB.AppendFormat(msg, formats);
			OutPutMsg(ELogType.Error, channel, m_FormatSB.ToString(), null);
		}

		public static void Fatal(Log_Channel channel, string msg)
		{
			OutPutMsg(ELogType.Fatal, channel, msg, null);
		}

		public static void FatalFormat(Log_Channel channel, string msg, params object[] formats)
		{
			m_FormatSB.Clear();
			m_FormatSB.AppendFormat(msg, formats);
			OutPutMsg(ELogType.Fatal, channel, m_FormatSB.ToString(), null);
		}
		#endregion

		private static void OutPutMsg(ELogType type, Log_Channel channel, string msg, System.Exception exception)
		{
			if (!IsEnable)
				return;

			m_SB.Clear();
			string nowTime = System.DateTime.Now.ToString("_yyyy_MM_dd_hhmmss");
			m_SB.Append(nowTime);
			if (channel != Log_Channel.None)
			{
				m_SB.Append(m_LogChannelStr[(int)channel]);
			}
			m_SB.Append(msg);
			OutPut(type, m_SB.ToString(), exception);
		}

		private static void OutPut(ELogType type, string sb, System.Exception exception)
		{
			int len = sb.Length;
			if (m_CharArray.Length < len)
			{
				m_CharArray = new char[len];
			}
			sb.CopyTo(0, m_CharArray, 0, len);

			//写入Log文件中
			if (EnableFileLog)
			{
				try
				{
					if (m_Writer == null)
					{
						string filePath = Path.Combine(Application.dataPath, LogFileName);
						string preFilePath = Path.Combine(Application.dataPath, PreLogFileName);
						if (File.Exists(filePath))
							File.Move(filePath, preFilePath);
						m_Writer = File.CreateText(filePath);
					}
					if (m_Writer != null)
					{
						m_Writer.Write(m_CharArray, 0, len);
						m_Writer.Write(FileLineEnding);
						if (exception != null)
						{
							m_Writer.Write(exception.ToString());
							m_Writer.Write(FileLineEnding);
						}
						m_Writer.Flush();//实时写入，而不是等文件流关闭后写入
										 //实时写入，而不是等文件流关闭后写入
						m_Writer.Flush();
					}
				}
				catch (System.Exception)
				{
					if (m_Writer != null)
					{
						m_Writer.Close();
						m_Writer = null;
					}
				}
			}

			//编辑器模式下输出到控制台中
#if UNITY_EDITOR
			if (PrintLogToUnityLog)
			{
				string str = new string(m_CharArray, 0, len);
				switch (type)
				{
					case ELogType.Error:
					case ELogType.Fatal:
						UnityEngine.Debug.LogError(str);
						break;
					case ELogType.Warning:
						UnityEngine.Debug.LogWarning(str);
						break;
					case ELogType.Info:
						UnityEngine.Debug.Log(str);
						break;
					case ELogType.Debug:
						UnityEngine.Debug.Log(str);
						break;
					default:
						UnityEngine.Debug.Log(str);
						break;
				}
			}
#else
			if (PrintLogToUnityLog)
			{
				string str = new string(m_CharArray, 0, len);
				switch (type)
				{
					case ELogType.Error:
						GameFrameworkLog.Error(str);
						break;
					case ELogType.Fatal:
						GameFrameworkLog.Fatal(str);
						break;
					case ELogType.Warning:
						GameFrameworkLog.Warning(str);
						break;
					case ELogType.Info:
						GameFrameworkLog.Info(str);
						break;
					case ELogType.Debug:
						GameFrameworkLog.Debug(str);
						break;
					default:
						UnityEngine.Debug.Log(str);
						break;
				}
			}
#endif

		}
	}
}
