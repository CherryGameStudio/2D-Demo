using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using UnityGameFrameworkEntry = UnityGameFramework.Runtime.GameEntry;

namespace Cherry
{
	public sealed partial class GameEntry
	{
		public static MapComponent Map
		{
			get { return UnityGameFrameworkEntry.GetComponent<MapComponent>(); }
		}
	}
}
