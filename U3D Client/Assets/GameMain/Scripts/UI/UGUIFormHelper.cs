using UnityGameFramework.Runtime;
using UnityEngine;
using GameFramework.UI;

namespace Cherry
{
    public class UGUIFormHelper : UIFormHelperBase
    {
		private ResourceComponent m_ResourceComponent = null;

		/// <summary>
		/// 实例化界面。
		/// </summary>
		/// <param name="uiFormAsset">要实例化的界面资源。</param>
		/// <returns>实例化后的界面。</returns>
		public override object InstantiateUIForm(object uiFormAsset)
		{
			return Instantiate((Object)uiFormAsset);
		}

		/// <summary>
		/// 创建界面。
		/// </summary>
		/// <param name="uiFormInstance">界面实例。</param>
		/// <param name="uiGroup">界面所属的界面组。</param>
		/// <param name="userData">用户自定义数据。</param>
		/// <returns>界面。</returns>
		public override IUIForm CreateUIForm(object uiFormInstance, IUIGroup uiGroup, object userData)
		{
			GameObject gameObject = uiFormInstance as GameObject;
			if (gameObject == null)
			{
				Log.Error("UI form instance is invalid.");
				return null;
			}

			RectTransform m_RectTransform = gameObject.GetOrAddComponent<RectTransform>();
			m_RectTransform.SetParent(((MonoBehaviour)uiGroup.Helper).transform);
			m_RectTransform.localScale = Vector3.one;
			m_RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
			m_RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, 0);
			//m_RectTransform.sizeDelta = new Vector2(1024, 768);
			//锚框
			m_RectTransform.anchorMin = Vector2.zero;
			m_RectTransform.anchorMax = Vector2.one;
			return gameObject.GetOrAddComponent<UIForm>();
		}

		/// <summary>
		/// 释放界面。
		/// </summary>
		/// <param name="uiFormAsset">要释放的界面资源。</param>
		/// <param name="uiFormInstance">要释放的界面实例。</param>
		public override void ReleaseUIForm(object uiFormAsset, object uiFormInstance)
		{
			m_ResourceComponent.UnloadAsset(uiFormAsset);
			Destroy((Object)uiFormInstance);
		}

		private void Start()
		{
			m_ResourceComponent = GameEntry.Resource;
			if (m_ResourceComponent == null)
			{
				Log.Fatal("Resource component is invalid.");
				return;
			}
		}
	}
}
