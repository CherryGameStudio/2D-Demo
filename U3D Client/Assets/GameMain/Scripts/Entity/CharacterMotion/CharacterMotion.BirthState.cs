using GameFramework.Fsm;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cherry
{
	public sealed partial class CharacterMotion
	{
		private sealed class BirthState : StateBase
		{
			protected override void OnEnter(IFsm<CharacterMotion> fsm)
			{
				base.OnEnter(fsm);

				ChangeState<IdleState>(fsm);
			}
		}
	}
}
