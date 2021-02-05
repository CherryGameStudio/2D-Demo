using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cherry
{
    public class MyPlayerCharacterController
    {
		private PlayerCharacter m_PlayerCharacter;

		private float m_HorizontalRaw;
        private float m_VerticalRaw;


		public PlayerCharacter PlayerCharacter
		{
			get { return m_PlayerCharacter; }
		}

        public float HorizontalRaw
		{
			get { return m_HorizontalRaw; }
			set { m_HorizontalRaw = value; }
		}

		public float VerticalRaw
		{
			get { return m_VerticalRaw; }
			set { m_VerticalRaw = value; }
		}

		public void UpdateCharacterController()
		{
			if (m_PlayerCharacter == null)
			{
				return;
			}
            //Debug.Log("玩家控制轮询");
		}

		public void SetPlayerCharacter(PlayerCharacter character)
		{
			m_PlayerCharacter = character;
		}
    }
}
