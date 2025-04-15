using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingEnemiesState : BaseState<BoidStates, Boid>
{
    public override void OnEnter() { }

    public override void OnUpdate()
    {
        if (avatar.IsLowHealth())
        {
            stateMachine.ChangeState(BoidStates.BoidEscaping);
            return;
        }

        if (!avatar.CanSeeEnemy())
        {
            stateMachine.ChangeState(BoidStates.FollowingLeader);
            return;
        }

        avatar.AttackEnemies();
    }

    public override void OnExit() { }
}