using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cherry 
{
	/// <summary>
	/// 地图块逻辑基类。
	/// </summary>
	public class ChunkLogic : MonoBehaviour
	{
		private Chunk m_Chunk;
		private Transform m_CachedTransform = null;

		/// <summary>
		/// 获取地图块。
		/// </summary>
		public Chunk Chunk
		{
			get { return m_Chunk; }
		}

		/// <summary>
		/// 获取已缓存的Transform。
		/// </summary>
		public Transform Transform
		{
			get { return m_CachedTransform; }
		}

		/// <summary>
		/// 获取地图块名称。
		/// </summary>
		public string Name
		{
			get { return gameObject.name; }
		}

		public virtual void OnInit(object userData)
		{
			if (m_CachedTransform == null)
			{
				m_CachedTransform = transform;
			}

			m_Chunk = GetComponent<Chunk>();
		}

		public virtual void OnRecycle()
		{

		}

		public virtual void OnEnter(object userData)
		{
			Debug.Log("enter" + this.Chunk.ChunkAssetName);
		}

		public virtual void OnLeave(object userData)
		{
			Debug.Log("leave" + this.Chunk.ChunkAssetName);
		}

		public virtual void OnPause()
		{

		}

		public virtual void OnResume()
		{

		}

		public virtual void OnUpdate()
		{

		}

		private bool isEnter = false;
		public virtual void OnTriggerEnter2D(Collider2D collision)
		{

		}

		public virtual void OnTriggerExit2D(Collider2D collision)
		{

		}

	} 
}
