using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderStateMachine2 : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 initialPos;
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float obstacleRange;
    [SerializeField] private LayerMask obstacleMask;

    private bool isWaitingForPath;
    private List<Node> actualPath = new List<Node>();
    private Vector3 _obstacleDir;
    private int _obstacleCount = 0;
    private float stoppingDistance = 0.5f;

    private void Awake()
    {
        initialPos = transform.position;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                target = Pathfinding.Instance.GetClosestNode(hit.point).transform;
                RequestNewPath();
            }
        }

        if (!isWaitingForPath)
        {
            FollowPath();
        }
    }

    private void FollowPath()
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
                transform.forward = Vector3.Lerp(transform.forward, dir.normalized + _obstacleDir, rotationSpeed * Time.deltaTime);
                transform.position += transform.forward * speed * Time.deltaTime;
            }
            else
            {
                actualPath.RemoveAt(0);
            }
        }
        else
        {
            isWaitingForPath = false;
        }

        _obstacleCount++;
        if (_obstacleCount > 2) _obstacleCount = 0;
    }

    private void RequestNewPath()
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
    }

    private void ErrorCallback()
    {
        Debug.LogError("No se encontró ningún nodo");
        isWaitingForPath = false;
    }
}
