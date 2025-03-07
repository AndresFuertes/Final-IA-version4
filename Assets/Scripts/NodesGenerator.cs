using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Runtime.CompilerServices;

[ExecuteInEditMode]
public class NodesGenerator : MonoBehaviour
{
    [SerializeField] private Vector3 _area;
    [SerializeField] private float _nodeSize;
    private float _nodeDistance => _nodeSize * 2.5f;
    [SerializeField] private Node _prefab;
    [SerializeField] private List<Node> _nodeList = new List<Node>();
    private HashSet<Vector3> _spawnedNodes = new HashSet<Vector3>();
    private float maxX => transform.position.x + _area.x / 2;
    private float minY => transform.position.y - _area.y / 2;
    private float minZ => transform.position.z - _area.z / 2;
    private float maxZ => minZ + _area.z;
    private int multiplyDir = -1;
    [SerializeField] private Transform _targetParent;
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private Node _centerNode;

    public void Generate()
    {
        _spawnedNodes = new HashSet<Vector3>();
        var actualStartRay = transform.position;
        actualStartRay.x -= _area.x / 2;
        actualStartRay.y += _area.y / 2;
        actualStartRay.z += _area.z / 2;

        Node actualNode;
        var count = 0;
        while (actualStartRay.y >= minY)
        {
            actualStartRay.x = transform.position.x - (_area.x / 2);
            actualStartRay.z = transform.position.z + _area.z / 2;

            while (actualStartRay.x <= maxX)
            {
                var ray = new Ray(actualStartRay, Vector3.down);

                if (Physics.Raycast(ray, out var hit, Mathf.Infinity, LayerMask.GetMask("Floor")))
                {
                    if (!Physics.Raycast(ray, (hit.point - actualStartRay).magnitude, LayerMask.GetMask("Paredes"))
                        && !Physics.CheckSphere(hit.point + Vector3.up, _nodeSize, _obstacleMask)
                        && hit.point.y > minY)
                    {
                        if (!IsNodeInRange(_spawnedNodes, hit.point + Vector3.up))
                        {
                            actualNode = PrefabUtility.InstantiatePrefab(_prefab, _targetParent) as Node;
                            actualNode.gameObject.name += count;
                            actualNode.transform.position = hit.point + Vector3.up;
                            _spawnedNodes.Add(hit.point + Vector3.up);
                            _nodeList.Add(actualNode);
                        }
                    }
                }

                float distance = _nodeDistance;
                distance *= multiplyDir;
                actualStartRay.z += distance;

                if (actualStartRay.z > maxZ || actualStartRay.z < minZ)
                {
                    actualStartRay.z -= distance;
                    multiplyDir *= -1;
                    actualStartRay.x += _nodeDistance;
                }

                count++;
            }

            multiplyDir = -1;
            actualStartRay.y -= 1;
        }

        multiplyDir = -1;
    }

    public void DeleteNodes()
    {
        if (!_nodeList.Any()) return;

        foreach (var node in _nodeList)
        {
            DestroyImmediate(node.gameObject);
        }

        _nodeList = new List<Node>();
    }

    public void GenerateNeighbours()
    {
        foreach (var node in _nodeList.Select(actualNode => actualNode.GetComponent<Node>()))
        {
            Undo.RecordObject(node, "SetNeighbours");
            node.neighbours.Clear();
        }

        foreach (var actualNode in _nodeList)
        {
            Undo.RecordObject(actualNode, "SetNeighbours");
            var neighbours = Physics.OverlapSphere(actualNode.transform.position, _nodeDistance, LayerMask.GetMask("Node"));

            foreach (var neighbour in neighbours)
            {
                var checkingNeighbour = neighbour.gameObject.GetComponent<Node>();

                if (checkingNeighbour == actualNode) continue;

                if (actualNode.neighbours.Contains(checkingNeighbour)) continue;

                var dir = neighbour.transform.position - actualNode.transform.position;
                var rayWallChecker = new Ray(actualNode.transform.position, dir);

                if (Physics.Raycast(rayWallChecker, _nodeDistance, LayerMask.GetMask("Paredes", "Floor"))
                    || Vector3.Angle(dir, Vector3.up) < 25 || Vector3.Angle(dir, Vector3.up) > 160
                    || !Physics.Raycast(actualNode.transform.position + dir / 2, Vector3.down, 2, LayerMask.GetMask("Floor"))) continue;

                checkingNeighbour.neighbours.Add(actualNode);
                actualNode.neighbours.Add(checkingNeighbour);
            }
            PrefabUtility.RecordPrefabInstancePropertyModifications(actualNode);
        }
    }

    public void ClearIsolatedNodes()
    {
        if (_centerNode == null)
        {
            Debug.LogWarning("There is no center node assigned");
            return;
        }

        foreach (var node in _nodeList)
        {
            if (!Pathfinding.FastPF(_centerNode, node))
            {
                foreach (var neighbour in node.neighbours)
                    neighbour.neighbours.Remove(node);

                DestroyImmediate(node.gameObject);
            }
        }
        RemoveNulls();
    }

    public void RemoveNulls()
    {
        _nodeList = _nodeList.Where(x => x != null).ToList();
    }

    public void RemoveMissing()
    {
        var mNodes = _nodeList.Select(x => x.GetComponent<Node>());
        foreach (var node in mNodes)
        {
            node.neighbours = node.neighbours.Where(x => true).ToList();
        }
    }

    private bool IsNodeInRange(HashSet<Vector3> positions, Vector3 newPosition)
    {
        foreach (var nodePosition in positions)
        {
            if (Vector3.Distance(nodePosition, newPosition) < _nodeSize)
                return true;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, _area);
    }
}
