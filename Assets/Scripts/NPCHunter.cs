using UnityEngine;

public class NPCHunter : MonoBehaviour
{
    private StateMachine<EnemyStates, NPCHunter> stateMachine;
    [SerializeField] private float _energy;
    [SerializeField] private Transform[] wayPoints;
    [SerializeField] private float speed;
    public float _detectionRadius;
    public LayerMask _playerMask;
    public float _predictionTime;
    public float _chaseSpeed;
    public GameObject _player
    {
        set;
        get;
    }

    public int _maxWaypoints
    {
        get
        {
            return wayPoints.Length;
        }
    }

    private void Awake()
    {
        stateMachine = new StateMachine<EnemyStates, NPCHunter>();

        stateMachine._posibleStates.Add(EnemyStates.Idle, new IdleState().Setup(stateMachine).SetAvatar(this));
        stateMachine._posibleStates.Add(EnemyStates.Patrol, new PatrolState().Setup(stateMachine).SetAvatar(this));
        stateMachine._posibleStates.Add(EnemyStates.Chase, new ChaseState().Setup(stateMachine).SetAvatar(this));

        stateMachine.ChangeState(EnemyStates.Idle);
    }

    private void Update()
    {
        stateMachine.OnUpdate();

        if (Input.GetKeyDown(KeyCode.C))
        {
            stateMachine.ChangeState(EnemyStates.Chase);
        }
    }

    public bool ReduceEnergy(float fatiga)
    {
        _energy -= fatiga;

        if (_energy < 0)
        {
            _energy = 0;
            return true;
        }
        return false;
    }

    public void RecoverEnergy()
    {
        _energy = 100;
    }

    public bool MoveTo(int index)
    {
        Vector3 direction = wayPoints[index].position - transform.position;
        transform.position += direction.normalized * speed * Time.deltaTime;

        if (direction.magnitude < .2f)
        {
            return true;
        }

        return false;
    }

    public void SetPlayer(GameObject player)
    {
        _player = player;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }
}
