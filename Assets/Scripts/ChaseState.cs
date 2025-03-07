using UnityEngine;

public class ChaseState : BaseState<EnemyStates, NPCHunter>
{
    private float _attackCD = 2f;
    public override void OnEnter()
    {

    }

    public override void OnUpdate()
    {
        if (avatar.ReduceEnergy(fatiga: Time.deltaTime * 5))
        {
            stateMachine.ChangeState(EnemyStates.Idle);
            return;
        }

        Boid boid = avatar._player.GetComponent<Boid>();
        Vector3 futurePosition = boid.transform.position + boid.transform.forward * boid.Speed * Time.deltaTime * avatar._predictionTime;

        futurePosition.y = avatar.transform.position.y;

        var chaseVector = futurePosition - avatar.transform.position;

        if (chaseVector.magnitude > 0.3f)
        {
            avatar.transform.position += chaseVector.normalized * Time.deltaTime * avatar._chaseSpeed;
        }
        else
        {
            _attackCD -= Time.deltaTime;

            if (_attackCD <= 0)
            {
                _attackCD = 2f;
            }
        }
    }

    public override void OnExit()
    {
        avatar.SetPlayer(null);
    }
}
