using GameFramework;
using GameFramework.Fsm;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cherry
{
	public sealed partial class CharacterMotion
	{
		private class StateBase : FsmState<CharacterMotion>, IReference
		{
			protected override void OnEnter(IFsm<CharacterMotion> fsm)
			{
				base.OnEnter(fsm);
			}

			public virtual void Clear()
			{
				throw new System.NotImplementedException();
			}
		}
	}
}
