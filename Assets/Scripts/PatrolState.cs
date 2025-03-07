using UnityEngine;

public class PatrolState : BaseState<EnemyStates, NPCHunter>
{
    private int _actualWaypoint = 0;
    private bool _forwardDirection = true;

    public override void OnUpdate()
    {
        if (avatar.ReduceEnergy(fatiga: Time.deltaTime * 5))
        {
            stateMachine.ChangeState(EnemyStates.Idle);
            return;
        }

        Collider[] closePlayer = Physics.OverlapSphere(avatar.transform.position, avatar._detectionRadius, avatar._playerMask);
        if (closePlayer.Length > 0)
        {
            avatar.SetPlayer(closePlayer[0].gameObject);
            stateMachine.ChangeState(EnemyStates.Chase);
            return;
        }

        if (avatar.MoveTo(_actualWaypoint))
        {
            if (_forwardDirection)
            {
                _actualWaypoint++;

                if (_actualWaypoint >= avatar._maxWaypoints)
                {
                    _actualWaypoint = avatar._maxWaypoints - 1;
                    _forwardDirection = false;
                }
            }
            else
            {
                _actualWaypoint--;

                if (_actualWaypoint < 0)
                {
                    _actualWaypoint = 1;
                    _forwardDirection = true;
                }
            }

        }
    }
}
