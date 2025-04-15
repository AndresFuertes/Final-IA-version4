using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class LeaderController : MonoBehaviour
{
    public Transform target;
    [SerializeField] private Vector3 initialPos;
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float obstacleRange;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float detectionRadius;
    [SerializeField] private float lowHealthThreshold;
    public Transform safeZone;
    [SerializeField] private int mouseButton = 0;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float attackCooldown = 1f;
    public List<LeaderController> enemies = new List<LeaderController>();

    public List<Node> actualPath = new List<Node>();
    public bool isWaitingForPath;
    private Vector3 _obstacleDir;
    private int _obstacleCount = 0;
    public float stoppingDistance = 0.5f;
    private StateMachine<LeaderStates, LeaderController> stateMachine;
    private float currentHealth;
    private float lastAttackTime = 0f;

    private void Awake()
    {
        initialPos = transform.position;
        stateMachine = new StateMachine<LeaderStates, LeaderController>();

        stateMachine._posibleStates.Add(LeaderStates.Walking, new WalkingState().Setup(stateMachine).SetAvatar(this));
        stateMachine._posibleStates.Add(LeaderStates.Fighting, new FightingState().Setup(stateMachine).SetAvatar(this));
        stateMachine._posibleStates.Add(LeaderStates.Escaping, new EscapingState().Setup(stateMachine).SetAvatar(this));
        stateMachine._posibleStates.Add(LeaderStates.Idle, new IdleState().Setup(stateMachine).SetAvatar(this));

        stateMachine.ChangeState(LeaderStates.Idle);
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(mouseButton))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                target = Pathfinding.Instance.GetClosestNode(hit.point).transform;
                stateMachine.ChangeState(LeaderStates.Walking);
            }
        }

        stateMachine.OnUpdate();
    }

    public void FollowPath()
    {
        if (_obstacleCount == 0)
        {
            _obstacleDir = ObstacleAvoidance().normalized;
        }

        if (actualPath.Count > 0)
        {
            Vector3 dir = actualPath[0].transform.position - transform.position;
            dir.y = 0;

            if (dir.magnitude > stoppingDistance)
            {
                if (Pathfinding.LineOfSight(transform.position, actualPath[0].transform.position))
                {
                    transform.forward = Vector3.Lerp(transform.forward, dir.normalized + _obstacleDir, rotationSpeed * Time.deltaTime);
                    transform.position += transform.forward * speed * Time.deltaTime;
                }
                else
                {
                    Debug.Log("Requesting new path due to lack of line of sight.");
                    RequestNewPath();
                }
            }
            else
            {
                actualPath.RemoveAt(0);
                if (actualPath.Count == 0)
                {
                    stateMachine.ChangeState(LeaderStates.Idle);
                }
            }
        }
        else
        {
            isWaitingForPath = false;
        }

        _obstacleCount++;
        if (_obstacleCount > 2) _obstacleCount = 0;
    }

    public void RequestNewPath()
    {
        if (target != null)
        {
            isWaitingForPath = true;
            Pathfinding.Instance.RequestPath(transform.position, target.position, PathCallback, ErrorCallback);
        }
    }

    private Vector3 ObstacleAvoidance()
    {
        var obstacles = Physics.OverlapSphere(transform.position, obstacleRange, obstacleMask);
        if (obstacles.Length <= 0) return Vector3.zero;

        Vector3 obstacleDir = Vector3.zero;
        foreach (var obstacle in obstacles)
        {
            obstacleDir += transform.position - obstacle.transform.position;
        }

        obstacleDir.y = 0f;
        return obstacleDir;
    }

    private void PathCallback(List<Node> path)
    {
        actualPath = path;
        isWaitingForPath = false;
        Debug.Log("New path received with " + actualPath.Count + " nodes.");
    }

    private void ErrorCallback()
    {
        Debug.LogError("No se encontró ningún nodo");
        isWaitingForPath = false;
    }

    public bool IsLowHealth()
    {
        return currentHealth <= lowHealthThreshold;
    }

    public bool IsEnemyInSight()
    {
        foreach (var enemy in enemies)
        {
            if (enemy != null && Vector3.Distance(transform.position, enemy.transform.position) <= detectionRadius)
            {
                return true;
            }
        }
        return false;
    }

    public void Attack(LeaderController enemy)
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            enemy.TakeDamage(20f); // Ajusta el daño según sea necesario
            lastAttackTime = Time.time;
            Debug.Log("Attacking enemy: " + enemy.name);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            // Lógica para cuando el líder muere
            Debug.Log("Leader has been defeated!");
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    private void OnDrawGizmos()
    {
        if (actualPath != null && actualPath.Count > 0)
        {
            Gizmos.color = Color.blue;
            foreach (var node in actualPath)
            {
                Gizmos.DrawSphere(node.transform.position, 0.3f);
            }

            Gizmos.color = Color.green;
            for (int i = 0; i < actualPath.Count - 1; i++)
            {
                Gizmos.DrawLine(actualPath[i].transform.position, actualPath[i + 1].transform.position);
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, actualPath[0].transform.position);
        }
    }
}







