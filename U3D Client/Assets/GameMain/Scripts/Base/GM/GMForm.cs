using System;
using System.Reflection;
using UnityEngine;

namespace Cherry
{
	public class GMForm : MonoBehaviour
	{
		private string strInput = string.Empty;
		private bool isopen = false;

		private void OnGUI()
		{
			GUILayout.BeginArea(new Rect(800, 0, 50, 50));

			if (GUILayout.Button("G"))
			{
				if (isopen)
					isopen = false;
				else
					isopen = true;
			}
			if (isopen)
			{
				windowRect = GUILayout.Window(1, windowRect, DoMyWindow, "GMManager");
			}
			GUILayout.EndArea();
		}


		private Rect windowRect = new Rect(100, 100, 600, 400);
		void DoMyWindow(int windowID)
		{
			GUILayout.BeginHorizontal();
			strInput = GUILayout.TextField(strInput, 50, GUILayout.Width(500), GUILayout.Height(20));
			if (GUILayout.Button("Send"))
			{
				string[] strs = strInput.Split(new char[] { ' ' });
				Type type = Type.GetType("Cherry.GMManager");
				MethodInfo method = type.GetMethod(strs[0]);
				ParameterInfo[] parameterInfos = method.GetParameters();
				if (parameterInfos.Length == 0)
				{
					method?.Invoke(this, null);
				}
				else
				{
					object[] Args = new object[parameterInfos.Length];
					//后续反射解析类型可扩展
					for (int i = 0; i < parameterInfos.Length; i++)
					{
						if (parameterInfos[i].ParameterType == typeof(int))
						{
							Args[i] = int.Parse(strs[i + 1]);
						}
						else if (parameterInfos[i].ParameterType == typeof(string))
						{
							Args[i] = strs[i + 1];
						}
					}
					method?.Invoke(this, Args);
				}
			}
			GUILayout.EndHorizontal();
		}
	}
}
