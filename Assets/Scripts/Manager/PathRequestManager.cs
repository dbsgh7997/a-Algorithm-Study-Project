using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using UnityEngine;

public class PathRequestManager : MonoBaseSingleton<PathRequestManager>
{
    private PathFinding pathFinding;

    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>(); // 여러 오브젝트들의 길찾기 요청을 순서대로 담은 Queue
    PathRequest currentPathRequest; // 현재 처리할 길찾기 요청

    private bool isProcessingPath;

    private void Awake()
    {
        pathFinding = GetComponent<PathFinding>();
    }

    // 오브젝트들이 요청하는 함수.
    public void RequestPath(Vector3 _pathStart, Vector3 _pathEnd, UnityAction<Vector3[], bool> _callback)
    {
        PathRequest newRequest = new PathRequest(_pathStart, _pathEnd, _callback);
        pathRequestQueue.Enqueue(newRequest);
        TryProcessNext();
    }

    // 길찾기가 완료된 요청을 처리하고 오브젝트에게 이동시작명령 콜백함수를 실행하는 함수.
    public void finishedProcessingPath(Vector3[] _path, bool _success)
    {
        currentPathRequest.callback(_path, _success);
        isProcessingPath = false;
        TryProcessNext();
    }

    // Queue순서대로 길찾기 요청을 꺼내 PathFinding 알고리즘 시작하는 함수.
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
