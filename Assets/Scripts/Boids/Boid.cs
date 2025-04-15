using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _desiredSpeed;
    [SerializeField] private float _acceleration;

    [SerializeField, Range(0, 1)] private float _separationWeight;
    [SerializeField, Range(0, 1)] private float _cohesionWeight;
    [SerializeField, Range(0, 1)] private float _alignmentWeight;

    [SerializeField] private LayerMask _enemyMask;
    [SerializeField] private LayerMask _boidMask;
    [SerializeField] private LayerMask _leaderMask;
    [SerializeField] private LayerMask _obstacleMask;

    [SerializeField] private float _obstacleAvoidanceDistance = 2f;
    [SerializeField] private float _separationRange = 2f;

    [SerializeField] private float _health = 100f;
    [SerializeField] private float _lowHealthThreshold = 10f;

    private Vector3 _desiredDirection;
    private BoidStateMachine _stateMachine;
    private Pathfinding _pathfinding;
    private List<Node> _currentPath = new List<Node>();
    private int _currentPathIndex = 0;

    private Transform _leader;
    private Vector3 _lastLeaderPosition;

    public LayerMask LeaderMask => _leaderMask;
    public LayerMask EnemyMask => _enemyMask;

    private void Awake()
    {
        transform.forward = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        _stateMachine = new BoidStateMachine(this);

        if (_pathfinding == null)
        {
            _pathfinding = FindObjectOfType<Pathfinding>();
        }

        _leader = FindLeader();
        if (_leader == null)
        {
            Debug.LogError("No se encontró al líder. Asegúrate de que el líder tenga un Collider y esté en la capa correcta.");
        }
        else
        {
            Debug.Log("Líder encontrado: " + _leader.name);
            _lastLeaderPosition = _leader.position;
        }
    }

    private void Update()
    {
        _stateMachine.OnUpdate();


        AvoidObstacles();


        if (_currentPath != null && _currentPathIndex < _currentPath.Count)
        {
            FollowPath();
        }


        Vector3 leaderDirection = Vector3.zero;
        if (_leader != null)
        {
            if (CanSeeLeader())
            {

                leaderDirection = (_leader.position - transform.position).normalized;
                _lastLeaderPosition = _leader.position;
            }
            else
            {

                if (Vector3.Distance(transform.position, _lastLeaderPosition) > 0.5f)
                {
                    RequestPath(_lastLeaderPosition);
                }
            }
        }


        Vector3 separation = Separation().normalized * _separationWeight;
        Vector3 cohesion = Cohesion().normalized * _cohesionWeight;
        Vector3 alignment = Alignment().normalized * _alignmentWeight;


        _desiredDirection = leaderDirection + separation + cohesion + alignment;
        _desiredDirection.y = 0;


        if (_desiredDirection.sqrMagnitude > 0.001f)
        {
            _desiredDirection.Normalize();
        }
        else
        {

            _desiredDirection = transform.forward;
        }


        Vector3 newDirection = Vector3.Lerp(transform.forward, _desiredDirection, _desiredSpeed * Time.deltaTime);
        newDirection.y = 0;


        if (newDirection.sqrMagnitude > 0.001f)
        {
            transform.forward = newDirection.normalized;
        }
        transform.position += transform.forward * _speed * Time.deltaTime;


        _desiredDirection = Vector3.zero;
    }

    private void AvoidObstacles()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, _obstacleAvoidanceDistance, _obstacleMask))
        {
            Vector3 avoidanceDirection = Vector3.Reflect(transform.forward, hit.normal);
            _desiredDirection += avoidanceDirection.normalized;
        }
    }

    private void FollowPath()
    {
        if (_currentPathIndex >= _currentPath.Count) return;

        Vector3 targetPosition = _currentPath[_currentPathIndex].transform.position;
        Vector3 direction = (targetPosition - transform.position).normalized;

        _desiredDirection += direction;

        if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
        {
            _currentPathIndex++;
        }
    }

    public void RequestPath(Vector3 targetPosition)
    {
        _pathfinding.RequestPath(transform.position, targetPosition, OnPathFound, OnPathError);
    }

    private void OnPathFound(List<Node> path)
    {
        _currentPath = path;
        _currentPathIndex = 0;
    }

    private void OnPathError()
    {
        Debug.Log("No se pudo encontrar un camino.");
    }

    public bool IsLowHealth()
    {
        return _health <= _lowHealthThreshold;
    }

    public bool CanSeeLeader()
    {
        if (_leader == null) return false;


        return Pathfinding.LineOfSight(transform.position, _leader.position);
    }

    public bool CanSeeEnemy()
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, 50f, _enemyMask);
        foreach (var enemy in enemiesInRange)
        {
            if (Pathfinding.LineOfSight(transform.position, enemy.transform.position))
            {
                return true;
            }
        }
        return false;
    }

    public void FollowLeader()
    {
        if (_leader == null) return;

        if (CanSeeLeader())
        {
            Vector3 leaderDir = _leader.position - transform.position;
            if (leaderDir.sqrMagnitude > 0.001f)
            {
                _desiredDirection += leaderDir.normalized;
                _lastLeaderPosition = _leader.position;
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, _lastLeaderPosition) > 0.5f)
            {
                RequestPath(_lastLeaderPosition);
            }
        }
    }

    public void SearchLeader()
    {
        // Moverse en una dirección aleatoria para buscar al líder
        _desiredDirection += new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    }

    public void AttackEnemies()
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, 50f, _enemyMask);
        foreach (var enemy in enemiesInRange)
        {
            if (Pathfinding.LineOfSight(transform.position, enemy.transform.position))
            {
                Vector3 enemyDir = enemy.transform.position - transform.position;
                _desiredDirection += enemyDir.normalized;
                break;
            }
        }
    }

    public void Escape()
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, 50f, _enemyMask);
        foreach (var enemy in enemiesInRange)
        {
            if (Pathfinding.LineOfSight(transform.position, enemy.transform.position))
            {
                Vector3 enemyDir = transform.position - enemy.transform.position;
                _desiredDirection += enemyDir.normalized;
                break;
            }
        }
    }

    private Transform FindLeader()
    {

        Collider[] leaderInRange = Physics.OverlapSphere(transform.position, float.MaxValue, _leaderMask);
        if (leaderInRange.Length > 0)
        {
            return leaderInRange[0].transform;
        }
        return null;
    }


    public Vector3 Separation()
    {
        Collider[] boidsInRange = Physics.OverlapSphere(transform.position, _separationRange, _boidMask);
        Vector3 separation = Vector3.zero;

        if (boidsInRange.Length > 0)
        {
            foreach (Collider actualBoid in boidsInRange)
            {
                Vector3 dirToBoid = transform.position - actualBoid.transform.position;
                float distance = dirToBoid.magnitude;
                if (distance > 0)
                {
                    separation += dirToBoid.normalized / distance;
                }
            }

            separation *= 3f;
        }

        return separation;
    }

    public Vector3 Cohesion()
    {
        Collider[] boidsInRange = Physics.OverlapSphere(transform.position, _separationRange, _boidMask);
        Vector3 cohesion = Vector3.zero;

        if (boidsInRange.Length > 0)
        {
            foreach (Collider actualBoid in boidsInRange)
            {
                cohesion += actualBoid.transform.position;
            }
            cohesion /= boidsInRange.Length;
            cohesion -= transform.position;
        }

        return cohesion;
    }

    public Vector3 Alignment()
    {
        Collider[] boidsInRange = Physics.OverlapSphere(transform.position, _separationRange, _boidMask);
        Vector3 alignment = Vector3.zero;

        if (boidsInRange.Length > 0)
        {
            foreach (Collider actualBoid in boidsInRange)
            {
                alignment += actualBoid.transform.forward;
            }
            alignment /= boidsInRange.Length;
        }

        return alignment;
    }

    public void StopMovement()
    {
        _desiredDirection = Vector3.zero;
        _currentPath = null;
        _currentPathIndex = 0;
    }

    public Vector3 GetLeaderPosition()
    {
        if (_leader != null)
        {
            return _leader.position;
        }
        return Vector3.zero;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(from: transform.position, _desiredDirection);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(from: transform.position, direction: transform.forward);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _obstacleAvoidanceDistance);

        if (_leader != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, _leader.position);
        }

        // Dibuja la línea de visión hacia el líder
        if (_leader != null && !Pathfinding.LineOfSight(transform.position, _leader.position))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _leader.position);
        }
    }
}