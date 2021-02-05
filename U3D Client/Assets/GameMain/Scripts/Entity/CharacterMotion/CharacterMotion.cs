using GameFramework;
using GameFramework.Fsm;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cherry
{
	/// <summary>
	/// 游戏角色状态机持有者。
	/// </summary>
	public sealed partial class CharacterMotion : MonoBehaviour
	{
		//游戏角色有限状态机。
		private IFsm<CharacterMotion> m_Fsm;

		public Character Owner
		{
			get;
			private set;
		}


		public void Init(Character owner)
		{
			Owner = owner;

			string fsmName = Utility.Text.Format("CharacterMotion[{0}]", Owner.Id.ToString());
			m_Fsm = GameEntry.Fsm.CreateFsm(fsmName, this,
				ReferencePool.Acquire<IdleState>(),
				ReferencePool.Acquire<MoveState>(),
				ReferencePool.Acquire<JumpState>(),
				ReferencePool.Acquire<BirthState>(),
				ReferencePool.Acquire<DeadState>(),
				ReferencePool.Acquire<AttackState>()
				);

			m_Fsm.Start<BirthState>();
		}
	}
}
