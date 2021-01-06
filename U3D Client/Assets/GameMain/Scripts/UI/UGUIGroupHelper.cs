using UnityGameFramework.Runtime;
using UnityEngine.UI;
using UnityEngine;

namespace Cherry
{
	/// <summary>
	/// UGUI界面组辅助器
	/// </summary>
	public class UGUIGroupHelper : UIGroupHelperBase
	{
		private RectTransform m_RectTransform;
		private Canvas m_Canvas;

		private void Awake()
		{
			m_RectTransform = gameObject.GetOrAddComponent<RectTransform>();
			m_Canvas = gameObject.GetOrAddComponent<Canvas>();
			gameObject.GetOrAddComponent<GraphicRaycaster>();
			Debug.Log(m_RectTransform.rect.ToString());

			
			

			m_RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, Screen.width);
			m_RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, Screen.height);
			//m_RectTransform.sizeDelta = new Vector2(1024, 768);
			//锚框
			m_RectTransform.anchorMin = Vector2.zero;
			m_RectTransform.anchorMax = Vector2.one;
			Debug.Log(m_RectTransform.rect.ToString());
		}

		private void Start()
		{
			
		}

		public override void SetDepth(int depth)
		{
			//对Canvas进行初始化操作
			//此步必须在这个函数中进行，因为新生成的GameObject是在世界结点下的，在Awake函数中不能设置overrideSorting
			//到SetDepth时UIGroup的Go才设置在Canvas下，才能使用。
			m_Canvas.overrideSorting = true;
			m_Canvas.sortingOrder = depth;
		}
	}
}
