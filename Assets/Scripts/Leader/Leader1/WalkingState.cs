using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingState : BaseState<LeaderStates, LeaderController>
{
    public override void OnEnter()
    {
        avatar.RequestNewPath();
    }

    public override void OnUpdate()
    {
        if (avatar.IsLowHealth())
        {
            stateMachine.ChangeState(LeaderStates.Escaping);
            return;
        }

        if (avatar.IsEnemyInSight())
        {
            stateMachine.ChangeState(LeaderStates.Fighting);
            return;
        }

        avatar.FollowPath();

        if (avatar.actualPath.Count == 0 && !avatar.isWaitingForPath)
        {
            stateMachine.ChangeState(LeaderStates.Idle);
        }
    }

    public override void OnExit()
    {
        // Lógica al salir del estado Walking
    }
}

