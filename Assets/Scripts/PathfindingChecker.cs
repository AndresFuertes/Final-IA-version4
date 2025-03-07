using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingChecker : MonoBehaviour
{
    public Transform fromTransform;
    public Transform toTransform;

    public List<Node> path = new List<Node>();

    private void CallbackPath(List<Node> newPath)
    {
        path = newPath;
    }

    private void OnDrawGizmos()
    {
        if (path.Count <= 0) return;

        foreach (Node node in path)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(node.transform.position, 1.2f);
        }
    }
}
