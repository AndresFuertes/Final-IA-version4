using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidEscapingState : BaseState<BoidStates, Boid>
{
    public override void OnEnter() { }

    public override void OnUpdate()
    {
        if (!avatar.IsLowHealth())
        {
            stateMachine.ChangeState(BoidStates.FollowingLeader);
            return;
        }

        avatar.Escape();
    }

    public override void OnExit() { }
}