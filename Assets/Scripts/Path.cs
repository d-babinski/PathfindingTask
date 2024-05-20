using System;
using UnityEngine;

public class Path : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer = null;

    private Vector3[] path = Array.Empty<Vector3>();
    
    public void AssignPath(Vector3[] _path)
    {
        lineRenderer.positionCount = _path.Length;
        lineRenderer.SetPositions(_path);
        path = _path;
    }
}
