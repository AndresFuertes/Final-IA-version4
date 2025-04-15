using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapingState : BaseState<LeaderStates, LeaderController>
{
    public override void OnEnter()
    {
        avatar.target = avatar.safeZone;
        avatar.RequestNewPath();
    }

    public override void OnUpdate()
    {
        avatar.FollowPath();

        if (!avatar.IsLowHealth())
        {
            stateMachine.ChangeState(LeaderStates.Idle);
        }
    }

    public override void OnExit()
    {
        // Lógica al salir del estado Escaping
    }
}
