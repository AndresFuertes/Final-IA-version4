using UnityEngine;

public class IdleState : BaseState<EnemyStates, NPCHunter>
{
    private float _actualCounter;
    public override void OnEnter()
    {
        _actualCounter = 5;
    }

    public override void OnUpdate()
    {
        _actualCounter -= Time.deltaTime;

        if (_actualCounter <= 0)
        {
            avatar.RecoverEnergy();
            stateMachine.ChangeState(EnemyStates.Patrol);
        }
    }
}

