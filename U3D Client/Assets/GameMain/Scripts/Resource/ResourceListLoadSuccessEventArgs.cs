using GameFramework;
using GameFramework.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cherry
{
	public sealed class ResourceListLoadSuccessEventArgs : GameEventArgs
	{
		private static readonly int EventId = typeof(ResourceListLoadSuccessEventArgs).GetHashCode();

		public ResourceListLoadSuccessEventArgs()
		{
			Count = default;
		}

		public override int Id
		{
			get { return EventId; }
		}

		/// <summary>
		/// 资源个数。
		/// </summary>
		public int Count
		{
			get;
			private set;
		}

		public static ResourceListLoadSuccessEventArgs Create(int count)
		{
			ResourceListLoadSuccessEventArgs e = ReferencePool.Acquire<ResourceListLoadSuccessEventArgs>();
			e.Count = count;
			return e;
		}

		public override void Clear()
		{
			Count = default;
		}
	}
}
