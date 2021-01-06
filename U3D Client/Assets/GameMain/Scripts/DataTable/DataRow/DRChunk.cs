///--------------------------------------
///此文件由工具自动生成请不要改动
///--------------------------------------
using System;
using System.Collections.Generic;
using UnityGameFramework.Runtime;

namespace Cherry
{
	public class DRChunk : DataRowBase
	{
		private int m_Id = 0;

		/// <summary>
		/// 获取地图块编号
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
		/// 获取地图块资源名称
		/// </summary>
		public string ChunkAssetName
		{
			private set;
			get;
		}

		/// <summary>
		/// 获取地图块连接的地图块资源名称列表（包含自身）
		/// </summary>
		public List<string> DependentChunkAssetNames
		{
			private set;
			get;
		}

		/// <summary>
		/// 获取地图组
		/// </summary>
		public string ChunkGroupName
		{
			private set;
			get;
		}

		/// <summary>
		/// 获取地图块连接的地图块编号(包含自身)
		/// </summary>
		public List<int> DependentChunkId
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
			ChunkAssetName = columnStrings[index++];
			DependentChunkAssetNames = new List<string>(columnStrings[index++].Split(new char[] { ',', '，' }));
			ChunkGroupName = columnStrings[index++];
			DependentChunkId = new List<int>(Array.ConvertAll<string, int>(columnStrings[index++].Split(new char[] { ',', '，' }), m_str => int.Parse(m_str)));
			return true;
		}
	}
}
