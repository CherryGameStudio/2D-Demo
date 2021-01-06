using UnityGameFramework.Runtime;
using GameFramework.Event;
using System;
using Cherry.Util;

namespace Cherry
{
	public static class DataTableExtension
	{
		public static void LoadAllDataTable(this DataTableComponent component)
		{
			GameEntry.Event.Subscribe(LoadDataTableSuccessEventArgs.EventId, OnLoadDataTableListSuccess);
			component.CreateDataTable(typeof(DRDataTableList)).ReadData("Assets/GameMain/DataTable/DataTableList.txt");
		}

		private const string DataTablePath = "Assets/GameMain/DataTable/";
		private static void OnLoadDataTableListSuccess(object sender, GameEventArgs e)
		{
			GLogger.Info(Log_Channel.DataTable, "数据表列表读取完成，开始读取游戏全局数据表");
			GameEntry.Event.Unsubscribe(LoadDataTableSuccessEventArgs.EventId, OnLoadDataTableListSuccess);

			int length = GameEntry.DataTable.GetDataTable<DRDataTableList>().Count;
			var dataTable = GameEntry.DataTable.GetDataTable<DRDataTableList>();
			for (int i = 0; i < length; i++)
			{
				string assetName = dataTable.GetDataRow(i).AssetName;
				string assetType = dataTable.GetDataRow(i).DataRowType;
				GameEntry.DataTable.CreateDataTable(Type.GetType(StringUtil.Concat("Cherry.",assetType))).ReadData(StringUtil.Concat(DataTablePath,assetName));
			}
		}
	}
}
