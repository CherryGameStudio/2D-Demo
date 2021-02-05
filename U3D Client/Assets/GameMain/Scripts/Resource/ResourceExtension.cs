using GameFramework;
using GameFramework.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Cherry
{
	public static class ResourceExtension
	{
		private static int m_LoadId = 0;

		private static LoadAssetCallbacks m_LoadAssetCallbacks;
		private static Dictionary<int, int> m_LoadingAssetsCount;

		static ResourceExtension()
		{
			m_LoadAssetCallbacks = new LoadAssetCallbacks(OnLoadAssetsSuccess, OnLoadAssetsFail);
			m_LoadingAssetsCount = new Dictionary<int, int>();
		}

		public static void LoadAssets(this ResourceComponent component,List<string> assetPaths , int priority = 0 , object userData = null)
		{
			int id = ++m_LoadId;
			foreach (string path in assetPaths)
			{
				GameEntry.Resource.LoadAsset(path, priority, m_LoadAssetCallbacks, LoadAssetsConfig.Create(id,assetPaths.Count,userData));
			}

			m_LoadingAssetsCount.Add(id, assetPaths.Count);
		}

		public static void TryResetLoadId(this ResourceComponent component)
		{
			if (m_LoadId > 1000000)
			{
				m_LoadId = 0;
				Debug.LogError("ResetLoadId!!!");
			}
		}

		private static void OnLoadAssetsSuccess(string assetName, object asset, float duration, object userData)
		{
			LoadAssetsConfig config = userData as LoadAssetsConfig;
			if (m_LoadingAssetsCount.ContainsKey(config.Id))
			{
				if (--m_LoadingAssetsCount[config.Id] == 0)
				{
					m_LoadingAssetsCount.Remove(config.Id);
					Debug.Log("加载一组资源成功！");
					//抛出加载成功事件
					GameEntry.Event.Fire(GameEntry.Resource, ResourceListLoadSuccessEventArgs.Create(config.Count));
				}
			}
			else
			{
				GLogger.Error(Log_Channel.Resource, "加载一组资源出现异常，引用计数出现错误！");
			}
		}

		private static void OnLoadAssetsFail(string assetName, LoadResourceStatus status, string errorMessage, object userData)
		{

		}

		private class LoadAssetsConfig : IReference
		{
			public int Id;
			public int Count;
			public object UserData;

			public static LoadAssetsConfig Create(int id,int count,object userData)
			{
				LoadAssetsConfig config = ReferencePool.Acquire<LoadAssetsConfig>();
				config.Id = id;
				config.Count = count;
				config.UserData = userData;
				return config;
			}

			public void Clear()
			{
				Id = default;
				Count = default;
				UserData = default;
			}
		}
	}
}
