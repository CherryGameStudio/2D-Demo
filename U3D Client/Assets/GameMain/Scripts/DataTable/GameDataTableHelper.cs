using GameFramework.DataTable;
using System;
using System.IO;
using System.Text;
using UnityGameFramework.Runtime;

namespace Cherry
{
	/// <summary>
	/// 游戏数据表辅助器。
	/// </summary>
	public class GameDataTableHelper : DefaultDataTableHelper
	{
		private static readonly string[] EmptyStringArray = new string[] { };

		/// <summary>
		/// 解析数据表。
		/// </summary>
		/// <param name="dataTable">数据表。</param>
		/// <param name="dataTableString">要解析的数据表字符串。</param>
		/// <param name="userData">用户自定义数据。</param>
		/// <returns>是否解析数据表成功。</returns>
		public override bool ParseData(DataTableBase dataTable, string dataTableString, object userData)
		{
			try
			{
				int line = 0;
				int position = 0;
				string dataRowString = null;
				while ((dataRowString = dataTableString.ReadLine(ref position)) != null)
				{
					line++;
					if (line <= 2)
					{
						continue;
					}

					if (dataRowString[0] == '#')
					{
						continue;
					}

					if (!dataTable.AddDataRow(dataRowString, userData))
					{
						Log.Error("Can not parse data row string '{0}'.", dataRowString);
						return false;
					}
				}

				return true;
			}
			catch (Exception exception)
			{
				Log.Error("Can not parse data table string with exception '{0}'.", exception.ToString());
				return false;
			}
		}

		/// <summary>
		/// 解析数据表。
		/// </summary>
		/// <param name="dataTable">数据表。</param>
		/// <param name="dataTableBytes">要解析的数据表二进制流。</param>
		/// <param name="startIndex">数据表二进制流的起始位置。</param>
		/// <param name="length">数据表二进制流的长度。</param>
		/// <param name="userData">用户自定义数据。</param>
		/// <returns>是否解析数据表成功。</returns>
		public override bool ParseData(DataTableBase dataTable, byte[] dataTableBytes, int startIndex, int length, object userData)
		{
			try
			{
				using (MemoryStream memoryStream = new MemoryStream(dataTableBytes, startIndex, length, false))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8))
					{
						int stringsOffset = binaryReader.ReadInt32();
						binaryReader.BaseStream.Position = stringsOffset;
						int stringCount = binaryReader.Read7BitEncodedInt32();
						string[] strings = stringCount > 0 ? new string[stringCount] : EmptyStringArray;
						for (int i = 0; i < stringCount; i++)
						{
							strings[i] = binaryReader.ReadString();
						}

						binaryReader.BaseStream.Position = sizeof(int);
						int dataRowCount = binaryReader.Read7BitEncodedInt32();
						for (int i = 0; i < dataRowCount; i++)
						{
							int dataRowBytesLength = binaryReader.Read7BitEncodedInt32();
							if (!dataTable.AddDataRow(dataTableBytes, (int)binaryReader.BaseStream.Position, dataRowBytesLength, strings))
							{
								Log.Error("Can not parse data row bytes.");
								return false;
							}

							binaryReader.BaseStream.Position += dataRowBytesLength;
						}

						if (binaryReader.BaseStream.Position != stringsOffset)
						{
							Log.Error("Strings offset verification failed.");
							return false;
						}
					}
				}

				return true;
			}
			catch (Exception exception)
			{
				Log.Error("Can not parse dictionary bytes with exception '{0}'.", exception.ToString());
				return false;
			}
		}
	}
}
