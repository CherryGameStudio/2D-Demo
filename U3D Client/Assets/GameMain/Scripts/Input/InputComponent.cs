using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Cherry
{
    public class InputComponent : GameFrameworkComponent
    {
        private MyPlayerCharacterController m_CharacterController;

        public MyPlayerCharacterController PlayerCharacterController
		{
            get { return m_CharacterController; }
		}

        void Start()
        {
            m_CharacterController = new MyPlayerCharacterController();
        }

        void Update()
        {
            m_CharacterController.UpdateCharacterController();

            //InputProxy
            m_CharacterController.HorizontalRaw = Input.GetAxisRaw("Horizontal");
            m_CharacterController.VerticalRaw = Input.GetAxisRaw("Vertical");
		}
    }
}
