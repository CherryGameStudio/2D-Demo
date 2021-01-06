using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cherry
{
	public class GMManager
	{
		public static void OpenUI(int type)
		{
			GameEntry.UI.OpenUIFormByUIFormType((UIFormType)type);
		}
	}
}
