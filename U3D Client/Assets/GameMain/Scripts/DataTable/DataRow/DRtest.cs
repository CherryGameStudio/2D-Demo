///--------------------------------------
///此文件由工具自动生成请不要改动
///--------------------------------------
using System;
using System.Collections.Generic;
using UnityGameFramework.Runtime;

namespace Cherry
{
	public class DRtest : DataRowBase
	{
		private int m_Id = 0;

		/// <summary>
		/// 获取备注
		/// </summary>
		public override int Id
		{
			get { return m_Id; }
		}

		/// <summary>
		/// 获取姓名
		/// </summary>
		public string name
		{
			private set;
			get;
		}

		/// <summary>
		/// 获取分数
		/// </summary>
		public int score
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
			name = columnStrings[index++];
			score = int.Parse(columnStrings[index++]);
			return true;
		}
	}
}
