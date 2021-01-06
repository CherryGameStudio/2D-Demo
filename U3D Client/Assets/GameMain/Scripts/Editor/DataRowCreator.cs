///--------------------------------------
///自动导入DataRow文件代码工具流程
///
///读取txt文件目录，得到所有的txt文件信息
///
///对单个txt表数据进行分割获取
///	1.获得第一行数据，拿到每个列的数据类型
///	2.获得第二行数据，拿到所有注释信息
///	3.获得第三行数据，拿到所有变量名称
///	4.后续内容不进行读取，跳过
///
///对每个文件进行CS生成操作
///	1.生成命名空间，等基本代码
///	2.根据表名，生成对应类名
///	3.根据数据类型，注释信息，变量名称生成对应属性
///	4.根据数据类型，变量名称生成解析函数
///	5.后续待扩展
///--------------------------------------

using UnityEditor;
using System.IO;
using System.Text;
using Cherry.Util;
using GameFramework;

namespace Cherry.Editor
{


	/// <summary>
	/// DataRow代码文件生成工具
	/// </summary>
	public sealed class DataRowCreator
	{
		private const string TxtFolderPath = "Assets/GameMain/DataTable";
		private const string OutPutFilePath = "Assets/GameMain/Scripts/DataTable/DataRow/";
		private static StringBuilder m_StringBuilder = new StringBuilder(1024);

		[MenuItem("CherryTool/导出txt数据表对应的CS文件")]
		public static void DataRowCS()
		{
			DirectoryInfo allTxtInfo = new DirectoryInfo(TxtFolderPath);
			FileInfo[] files = allTxtInfo.GetFiles();
			for (int i = 0; i < files.Length; i++)
			{
				if (files[i].Extension != ".txt")
					continue;
				EditorUtility.DisplayProgressBar("生成数据表CS文件", Utility.Text.Format("正在生成第{0}个文件，文件名为{1}，共有{2}个文件",i,files[i].Name,files.Length), i / files.Length);
				string[] dataType = default;
				string[] comments = default;
				string[] variateName = default;
				StreamReader stream = files[i].OpenText();
				try
				{
					dataType = stream.ReadLine().Split(new char[] { '\t' });
					comments = stream.ReadLine().Split(new char[] { '\t' });
					variateName = stream.ReadLine().Split(new char[] { '\t' });
				}
				catch (System.Exception)
				{

					throw;
				}
				finally
				{
					stream.Close();					
				}

				m_StringBuilder.Length = 0;
				m_StringBuilder.Append(StringUtil.LineText("///--------------------------------------"));
				m_StringBuilder.Append(StringUtil.LineText("///此文件由工具自动生成请不要改动"));
				m_StringBuilder.Append(StringUtil.LineText("///--------------------------------------"));
				m_StringBuilder.Append(StringUtil.LineText("using System;"));
				m_StringBuilder.Append(StringUtil.LineText("using System.Collections.Generic;"));
				m_StringBuilder.Append(StringUtil.LineText("using UnityGameFramework.Runtime;\n"));
				m_StringBuilder.Append(StringUtil.LineText("namespace Cherry"));
				m_StringBuilder.Append(StringUtil.LineText("{"));
				m_StringBuilder.Append(StringUtil.LineText(StringUtil.Concat("public class DR", files[i].Name.Replace(".txt",""), " : DataRowBase"), 1));
				m_StringBuilder.Append(StringUtil.LineText("{",1));
				m_StringBuilder.Append(StringUtil.LineText("private int m_Id = 0;\n",2));
				for (int j = 0; j < dataType.Length; j++)
				{
					if (j == 0)
					{
						m_StringBuilder.Append(StringUtil.LineText("/// <summary>",2));
						m_StringBuilder.Append(StringUtil.LineText(StringUtil.Concat("/// 获取",comments[j]),2));
						m_StringBuilder.Append(StringUtil.LineText("/// </summary>", 2));
						m_StringBuilder.Append(StringUtil.LineText("public override int Id", 2));
						m_StringBuilder.Append(StringUtil.LineText("{",2));
						m_StringBuilder.Append(StringUtil.LineText("get { return m_Id; }",3));
						m_StringBuilder.Append(StringUtil.LineText("}\n",2));
					}
					else
					{
						m_StringBuilder.Append(StringUtil.LineText("/// <summary>", 2));
						m_StringBuilder.Append(StringUtil.LineText(StringUtil.Concat("/// 获取", comments[j]), 2));
						m_StringBuilder.Append(StringUtil.LineText("/// </summary>", 2));
						m_StringBuilder.Append(StringUtil.LineText(StringUtil.Concat("public ", dataType[j], " ", variateName[j]), 2));
						m_StringBuilder.Append(StringUtil.LineText("{",2));
						m_StringBuilder.Append(StringUtil.LineText("private set;",3));
						m_StringBuilder.Append(StringUtil.LineText("get;",3));
						m_StringBuilder.Append(StringUtil.LineText("}\n",2));
					}
				}

				m_StringBuilder.Append(StringUtil.LineText("public override bool ParseDataRow(string dataRowString, object userData)", 2));
				m_StringBuilder.Append(StringUtil.LineText("{", 2));
				m_StringBuilder.Append(StringUtil.LineText("string[] columnStrings = dataRowString.Split(new char[] { '\\t' });", 3));
				m_StringBuilder.Append(StringUtil.LineText("for (int i = 0; i < columnStrings.Length; i++)", 3));
				m_StringBuilder.Append(StringUtil.LineText("{", 3));
				m_StringBuilder.Append(StringUtil.LineText(@"columnStrings[i] = columnStrings[i].Trim(new char[] { '\""' });", 4));
				m_StringBuilder.Append(StringUtil.LineText("}", 3));
				m_StringBuilder.Append(StringUtil.LineText("int index = 0;", 3));
				m_StringBuilder.Append(StringUtil.LineText("m_Id = int.Parse(columnStrings[index++]);", 3));
				for (int j = 1; j < dataType.Length; j++)
				{
					//后续可扩展读取类型
					switch (dataType[j])
					{
						case "int":
							m_StringBuilder.Append(StringUtil.LineText(StringUtil.Concat(variateName[j], " = int.Parse", "(columnStrings[index++]);"), 3));
							break;
						case "string":
							m_StringBuilder.Append(StringUtil.LineText(StringUtil.Concat(variateName[j], " = columnStrings[index++];"), 3));
							break;
						case "List<int>":
							m_StringBuilder.Append(StringUtil.LineText(StringUtil.Concat(variateName[j], " = new List<int>(Array.ConvertAll<string, int>(columnStrings[index++].Split(new char[] { ',', '，' }), m_str => int.Parse(m_str)));"), 3));
							break;
						case "List<string>":
							m_StringBuilder.Append(StringUtil.LineText(StringUtil.Concat(variateName[j], " = new List<string>(columnStrings[index++].Split(new char[] { ',', '，' }));"),3));
							break;
						default:
							break;
					}
				}
				m_StringBuilder.Append(StringUtil.LineText("return true;", 3));
				m_StringBuilder.Append(StringUtil.LineText("}", 2));
				m_StringBuilder.Append(StringUtil.LineText("}", 1));
				m_StringBuilder.Append(StringUtil.LineText("}"));

				try
				{
					using (StreamWriter writer = new StreamWriter(StringUtil.Concat(OutPutFilePath, "DR", files[i].Name.Replace(".txt", ".cs"))))
					{
						writer.Write(m_StringBuilder.ToString());
					}
				}
				catch (System.Exception e)
				{
					GLogger.ErrorFormat(Log_Channel.DataTable, "写入脚本文件时出现了意外的错误，msg：{0}", e);
					EditorUtility.ClearProgressBar();
					throw;
				}
			}
			EditorUtility.ClearProgressBar();
			AssetDatabase.Refresh();
		}
	}
}
