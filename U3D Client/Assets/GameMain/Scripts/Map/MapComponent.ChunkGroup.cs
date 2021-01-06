using System;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Cherry
{
	public sealed partial class MapComponent : GameFrameworkComponent
	{
		[Serializable]
		private sealed class ChunkGroup
		{
			[SerializeField]
			private string m_Name = null;

			public string ChunkGroupName
			{
				get
				{
					return m_Name;
				}
			}
		}
	}
}
