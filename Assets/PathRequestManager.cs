using System; //Need the Action class
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Too many requests for paths from the pathfinder at once can cause freezing.
 * 
 * Therefore this script is going to spread things out.
 * 
 * It will store them in a queue.
 */

public class PathRequestManager : MonoBehaviour {

    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;
    Pathfinding pathfinding;
    bool isProcessingPath;

    static PathRequestManager instance;

    /*
     * Re-do to be more in line with how I usually do instancing later.
     * 
     */
    private void Awake()
    {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }


    /*
     * Action is used for spreading out the requests, so we can pass a method and then call that method once the path finding is done
     */
    public static void RequestPath(Vector2 pathStart, Vector2 pathEnd, Action<Vector2[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    private void TryProcessNext()
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }
    }

    public void FinishedProcessingPath(Vector2[] path, bool success)
    {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    struct PathRequest
    {
        public Vector2 pathStart;
        public Vector2 pathEnd;
        public Action<Vector2[], bool> callback;

        public PathRequest(Vector2 _start, Vector2 _end, Action<Vector2[], bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }
    }

}
