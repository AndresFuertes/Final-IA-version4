using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseState<LeaderStates, LeaderController>
{
    [SerializeField] private float healRate = 1f;
    [SerializeField] private float healInterval = 2f;
    private float lastHealTime = 0f;

    public override void OnEnter()
    {
        avatar.actualPath.Clear();
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

        // Recuperar salud gradualmente
        if (Time.time - lastHealTime >= healInterval)
        {
            avatar.Heal(healRate);
            lastHealTime = Time.time;
        }
    }

    public override void OnExit()
    {
        // Lógica al salir del estado Idle
    }
}
