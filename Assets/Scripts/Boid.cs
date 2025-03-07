using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    [SerializeField] float _detectionRange;
    [SerializeField] float _separationRange;
    [SerializeField] float _speed;
    [SerializeField] float _maxSpeed;
    [SerializeField] float _desiredSpeed;
    [SerializeField] float _acceleration;

    [SerializeField, Range(0, 1)] float _separationWeight;
    [SerializeField, Range(0, 1)] float _cohesionWeight;
    [SerializeField, Range(0, 1)] float _alignmentWeight;

    [SerializeField] LayerMask _enemyMask;
    [SerializeField] LayerMask _boidMask;
    [SerializeField] LayerMask _leaderMask;

    [SerializeField] float _evadeDistance;
    [SerializeField] float _pursuitDistance;

    private Vector3 _desiredDirection;

    public float Speed
    {
        get { return _speed; }
    }

    private void Awake()
    {
        transform.forward = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
    }



    void Update()
    {
        //BoidLimit.Instance.CheckBounds(boid: this);
        _desiredDirection = Vector3.zero;

        Collider[] leaderInRange = Physics.OverlapSphere(transform.position, _detectionRange, _leaderMask);
        if (leaderInRange.Length > 0)
        {
            Vector3 leaderDir = leaderInRange[0].transform.position - transform.position;
            _desiredDirection += leaderDir;

            if (leaderDir.magnitude < 1)
            {
                _speed -= _acceleration * Time.deltaTime;
                //FindObjectOfType<FoodSpawner>().RemoveFood(foodInRange[0].gameObject);
            }
            else
            {
                _desiredDirection += leaderDir.normalized;
                _speed += _acceleration * Time.deltaTime;
            }
        }
        else
        {
            _speed += _acceleration * Time.deltaTime;
        }

        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, _detectionRange, _enemyMask);
        if (enemiesInRange.Length > 0)
        {
            Vector3 futureHunterPos = enemiesInRange[0].transform.position + enemiesInRange[0].transform.forward * _evadeDistance;
            _desiredDirection += (transform.position - futureHunterPos) + (transform.position - enemiesInRange[0].transform.position);

            _speed += _acceleration * Time.deltaTime;
        }
        else
        {
            _speed += _acceleration * Time.deltaTime;
        }

        _desiredDirection += Separation().normalized * _separationWeight + Cohesion().normalized * _cohesionWeight + Alignment().normalized * _alignmentWeight;

        _desiredDirection.y = 0;
        _desiredDirection.Normalize();

        Vector3 newDirection = Vector3.Lerp(transform.forward, _desiredDirection, _desiredSpeed);
        newDirection.y = 0;

        _speed = Mathf.Clamp(value: _speed, min: 0, _maxSpeed);

        transform.forward = newDirection.normalized;
        transform.position += transform.forward * _speed * Time.deltaTime;
    }

    public Vector3 Separation()
    {
        Collider[] boidsInRange = Physics.OverlapSphere(transform.position, _separationRange, _boidMask);

        Vector3 separation = Vector3.zero;
        if (boidsInRange.Length > 0)
        {

            foreach (Collider actualBoid in boidsInRange)
            {
                separation += transform.position - actualBoid.transform.position;
            }
        }
        return separation;
    }

    public Vector3 Alignment()
    {
        Collider[] boidsInRange = Physics.OverlapSphere(transform.position, _detectionRange, _boidMask);

        Vector3 alignment = Vector3.zero;
        if (boidsInRange.Length > 0)
        {

            foreach (Collider actualBoid in boidsInRange)
            {
                alignment += actualBoid.transform.forward;
            }
        }
        return alignment;
    }

    public Vector3 Cohesion()
    {
        Collider[] boidsInRange = Physics.OverlapSphere(transform.position, _detectionRange, _boidMask);

        Vector3 detection = Vector3.zero;
        if (boidsInRange.Length > 0)
        {

            foreach (Collider actualBoid in boidsInRange)
            {
                detection += actualBoid.transform.position - transform.position;
            }
        }
        return detection;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(from: transform.position, _desiredDirection);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(from: transform.position, direction: transform.forward);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _separationWeight);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _separationRange);
        Gizmos.color = Color.green;
    }
}