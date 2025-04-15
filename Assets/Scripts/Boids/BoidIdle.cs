using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidIdle : BaseState<BoidStates, Boid>
{
    public override void OnEnter()
    {
        avatar.StopMovement();
    }

    public override void OnUpdate()
    {
        if (avatar.CanSeeLeader())
        {
            stateMachine.ChangeState(BoidStates.FollowingLeader);
        }
        else if (avatar.CanSeeEnemy())
        {
            stateMachine.ChangeState(BoidStates.AttackingEnemies);
        }
        else if (avatar.IsLowHealth())
        {
            stateMachine.ChangeState(BoidStates.BoidEscaping);
        }
    }

    public override void OnExit() { }
}
