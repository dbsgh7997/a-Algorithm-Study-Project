using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private Transform target;

    private float speed = 20;
    private Vector3[] path;
    private int targetIndex;

    private void Awake()
    {
        PathRequestManager.getInstance.RequestPath(transform.position, target.position, OnPathFound);
    }

    public void OnPathFound(Vector3[] _newPath, bool _pathSuccessful)
    {
        if(_pathSuccessful)
        {
            path = _newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    private IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];

        while (true)
        {
            if(transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }
}
