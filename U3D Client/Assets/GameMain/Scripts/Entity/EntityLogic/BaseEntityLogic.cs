﻿using UnityGameFramework.Runtime;
using UnityEngine;

namespace Cherry
{
	/// <summary>
	/// 游戏实体抽象类。
	/// </summary>
	public abstract class BaseEntityLogic : EntityLogic
	{
		public int Id
		{
			get { return base.Entity.Id; }
		}

		/// <summary>
		/// 实体初始化。
		/// </summary>
		/// <param name="userData">用户自定义数据。</param>
		protected override void OnInit(object userData)
		{
			base.OnInit(userData);
		}

		/// <summary>
		/// 实体回收。
		/// </summary>
		protected override void OnRecycle()
		{
			base.OnRecycle();
		}

		/// <summary>
		/// 实体显示。
		/// </summary>
		/// <param name="userData">用户自定义数据。</param>
		protected override void OnShow(object userData)
		{
			base.OnShow(userData);
		}

		/// <summary>
		/// 实体隐藏。
		/// </summary>
		/// <param name="isShutdown">是否是关闭实体管理器时触发。</param>
		/// <param name="userData">用户自定义数据。</param>
		protected override void OnHide(bool isShutdown, object userData)
		{
			base.OnHide(isShutdown,userData);
		}

		/// <summary>
		/// 实体附加子实体。
		/// </summary>
		/// <param name="childEntity">附加的子实体。</param>
		/// <param name="parentTransform">被附加父实体的位置。</param>
		/// <param name="userData">用户自定义数据。</param>
		protected override void OnAttached(EntityLogic childEntity, Transform parentTransform, object userData)
		{
			base.OnAttached(childEntity, parentTransform, userData);
		}

		/// <summary>
		/// 实体解除子实体。
		/// </summary>
		/// <param name="childEntity">解除的子实体。</param>
		/// <param name="userData">用户自定义数据。</param>
		protected override void OnDetached(EntityLogic childEntity, object userData)
		{
			base.OnDetached(childEntity, userData);
		}

		/// <summary>
		/// 实体附加子实体。
		/// </summary>
		/// <param name="parentEntity">被附加的父实体。</param>
		/// <param name="parentTransform">被附加父实体的位置。</param>
		/// <param name="userData">用户自定义数据。</param>
		protected override void OnAttachTo(EntityLogic parentEntity, Transform parentTransform, object userData)
		{
			OnAttached(parentEntity, parentTransform, userData);
		}

		/// <summary>
		/// 实体解除子实体。
		/// </summary>
		/// <param name="parentEntity">被解除的父实体。</param>
		/// <param name="userData">用户自定义数据。</param>
		protected override void OnDetachFrom(EntityLogic parentEntity, object userData)
		{
			base.OnDetachFrom(parentEntity, userData);
		}

		/// <summary>
		/// 实体轮询。
		/// </summary>
		/// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
		/// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
		protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
		{
			base.OnUpdate(elapseSeconds,realElapseSeconds);
		}

		/// <summary>
		/// 设置实体的可见性。
		/// </summary>
		/// <param name="visible">实体的可见性。</param>
		protected override void InternalSetVisible(bool visible)
		{
			base.InternalSetVisible(visible);
		}
	}
}

