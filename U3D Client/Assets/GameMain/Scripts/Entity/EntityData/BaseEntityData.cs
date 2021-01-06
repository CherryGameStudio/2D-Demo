using GameFramework;
using UnityEngine;
using System;

namespace Cherry
{
	/// <summary>
	/// 实体数据抽象类
	/// </summary>
	[Serializable]
    public abstract class BaseEntityData : IReference
    {
		[SerializeField]
		private int m_Id = 0;

		[SerializeField]
		protected int m_Type = 0;

		[SerializeField]
		protected bool m_Ready = false;

		[SerializeField]
		private Vector3 m_Position = Vector3.zero;

		[SerializeField]
		private Quaternion m_Rotation = Quaternion.identity;

		/// <summary>
		/// 实体编号。
		/// </summary>
		public int Id
		{
			get
			{
				return m_Id;
			}
			set
			{
				m_Id = value;
			}
		}

		/// <summary>
		/// 实体类型。
		/// </summary>
		public virtual int Type
		{
			get
			{
				return m_Type;
			}
			set
			{
				m_Type = value;

				if (!m_Ready)
				{
					m_Ready = true;
					OnDataReady();
				}
			}
		}

		/// <summary>
		/// 数据是否准备好。
		/// </summary>
		public bool Ready
		{
			get
			{
				return m_Ready;
			}
		}

		/// <summary>
		/// 实体位置。
		/// </summary>
		public Vector3 Position
		{
			get
			{
				return m_Position;
			}
			set
			{
				m_Position = value;
			}
		}

		/// <summary>
		/// 实体朝向。
		/// </summary>
		public Quaternion Rotation
		{
			get
			{
				return m_Rotation;
			}
			set
			{
				m_Rotation = value;
			}
		}

		public virtual EntityLogicType LogicType
		{
			get
			{
				return EntityLogicType.Default;
			}
		}

		public virtual void Clear()
		{
			m_Id = -1;
			m_Type = 0;
			m_Ready = false;
			m_Position = Vector3.zero;
			m_Rotation = Quaternion.identity;
		}

		public virtual void OnDataReady()
		{
		}
	}
}
