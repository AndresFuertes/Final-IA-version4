using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightingState : BaseState<LeaderStates, LeaderController>
{
    private LeaderController currentTarget;

    public override void OnEnter()
    {
        currentTarget = FindClosestEnemy();
    }

    public override void OnUpdate()
    {
        if (avatar.IsLowHealth())
        {
            stateMachine.ChangeState(LeaderStates.Escaping);
            return;
        }

        if (currentTarget == null || !avatar.IsEnemyInSight())
        {
            stateMachine.ChangeState(LeaderStates.Idle);
            return;
        }

        // Moverse hacia el enemigo
        avatar.target = currentTarget.transform;
        avatar.FollowPath();

        // Atacar al enemigo si está en rango de contacto
        if (Vector3.Distance(avatar.transform.position, currentTarget.transform.position) < avatar.stoppingDistance)
        {
            avatar.Attack(currentTarget);
        }
    }

    private LeaderController FindClosestEnemy()
    {
        LeaderController closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (var enemy in avatar.enemies)
        {
            if (enemy != null)
            {
                float distance = Vector3.Distance(avatar.transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }

        return closestEnemy;
    }

    public override void OnExit()
    {
        currentTarget = null;
    }
}

