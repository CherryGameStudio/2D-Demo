///--------------------------------------
///此文件由工具自动生成请不要改动
///--------------------------------------
using System;
using System.Collections.Generic;
using UnityGameFramework.Runtime;

namespace Cherry
{
	public class DRDataTableList : DataRowBase
	{
		private int m_Id = 0;

		/// <summary>
		/// 获取数据表编号
		/// </summary>
		public override int Id
		{
			get { return m_Id; }
		}

		/// <summary>
		/// 获取策划备注
		/// </summary>
		public string Comment
		{
			private set;
			get;
		}

		/// <summary>
		/// 获取数据表名称
		/// </summary>
		public string AssetName
		{
			private set;
			get;
		}

		/// <summary>
		/// 获取数据表行类型
		/// </summary>
		public string DataRowType
		{
			private set;
			get;
		}

		public override bool ParseDataRow(string dataRowString, object userData)
		{
			string[] columnStrings = dataRowString.Split(new char[] { '\t' });
			for (int i = 0; i < columnStrings.Length; i++)
			{
				columnStrings[i] = columnStrings[i].Trim(new char[] { '\"' });
			}
			int index = 0;
			m_Id = int.Parse(columnStrings[index++]);
			Comment = columnStrings[index++];
			AssetName = columnStrings[index++];
			DataRowType = columnStrings[index++];
			return true;
		}
	}
}
