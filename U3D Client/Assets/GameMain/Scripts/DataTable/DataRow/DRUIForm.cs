///--------------------------------------
///此文件由工具自动生成请不要改动
///--------------------------------------
using System;
using System.Collections.Generic;
using UnityGameFramework.Runtime;

namespace Cherry
{
	public class DRUIForm : DataRowBase
	{
		private int m_Id = 0;

		/// <summary>
		/// 获取界面编号
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
		/// 获取资源名称
		/// </summary>
		public string AssetName
		{
			private set;
			get;
		}

		/// <summary>
		/// 获取界面组
		/// </summary>
		public string UIGroupName
		{
			private set;
			get;
		}

		/// <summary>
		/// 获取是否暂停被其覆盖的同组其它界面
		/// </summary>
		public int IsPauseOtherUIForm
		{
			private set;
			get;
		}

		/// <summary>
		/// 获取是否跳场景关闭界面
		/// </summary>
		public int IsChangeSceneClose
		{
			private set;
			get;
		}

		/// <summary>
		/// 获取是否隐藏Main层界面
		/// </summary>
		public int IsHideMainGroup
		{
			private set;
			get;
		}

		/// <summary>
		/// 获取是否隐藏Main2层界面
		/// </summary>
		public int IsHideMain2Group
		{
			private set;
			get;
		}

		/// <summary>
		/// 获取是否隐藏Dialog层界面
		/// </summary>
		public int IsHideDialogGroup
		{
			private set;
			get;
		}

		/// <summary>
		/// 获取是否隐藏Top层界面
		/// </summary>
		public int IsHideTopGroup
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
			UIGroupName = columnStrings[index++];
			IsPauseOtherUIForm = int.Parse(columnStrings[index++]);
			IsChangeSceneClose = int.Parse(columnStrings[index++]);
			IsHideMainGroup = int.Parse(columnStrings[index++]);
			IsHideMain2Group = int.Parse(columnStrings[index++]);
			IsHideDialogGroup = int.Parse(columnStrings[index++]);
			IsHideTopGroup = int.Parse(columnStrings[index++]);
			return true;
		}
	}
}
