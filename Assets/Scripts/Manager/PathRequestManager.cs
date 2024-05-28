using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using UnityEngine;

public class PathRequestManager : MonoBaseSingleton<PathRequestManager>
{
    private PathFinding pathFinding;

    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>(); // ���� ������Ʈ���� ��ã�� ��û�� ������� ���� Queue
    PathRequest currentPathRequest; // ���� ó���� ��ã�� ��û

    private bool isProcessingPath;

    private void Awake()
    {
        pathFinding = GetComponent<PathFinding>();
    }

    // ������Ʈ���� ��û�ϴ� �Լ�.
    public void RequestPath(Vector3 _pathStart, Vector3 _pathEnd, UnityAction<Vector3[], bool> _callback)
    {
        PathRequest newRequest = new PathRequest(_pathStart, _pathEnd, _callback);
        pathRequestQueue.Enqueue(newRequest);
        TryProcessNext();
    }

    // ��ã�Ⱑ �Ϸ�� ��û�� ó���ϰ� ������Ʈ���� �̵����۸�� �ݹ��Լ��� �����ϴ� �Լ�.
    public void finishedProcessingPath(Vector3[] _path, bool _success)
    {
        currentPathRequest.callback(_path, _success);
        isProcessingPath = false;
        TryProcessNext();
    }

    // Queue������� ��ã�� ��û�� ���� PathFinding �˰��� �����ϴ� �Լ�.
    private void TryProcessNext()
    {
        if(!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathFinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }
    }    
}

public struct PathRequest
{
    public Vector3 pathStart;
    public Vector3 pathEnd;
    public UnityAction<Vector3[], bool> callback;

    public PathRequest(Vector3 _start, Vector3 _end, UnityAction<Vector3[], bool> _callback)
    {
        pathStart = _start;
        pathEnd = _end;
        callback = _callback;
    }
}
