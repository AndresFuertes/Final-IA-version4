using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingLeaderState : BaseState<BoidStates, Boid>
{
    public override void OnEnter() { }

    public override void OnUpdate()
    {
        if (avatar.IsLowHealth())
        {
            stateMachine.ChangeState(BoidStates.BoidEscaping);
            return;
        }

        if (!avatar.CanSeeLeader())
        {
            stateMachine.ChangeState(BoidStates.SearchingLeader);
            return;
        }

        if (avatar.CanSeeEnemy())
        {
            stateMachine.ChangeState(BoidStates.AttackingEnemies);
            return;
        }

        avatar.FollowLeader();
    }

    public override void OnExit() { }
}

