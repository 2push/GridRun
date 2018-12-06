using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PathManager : MonoBehaviour {

    private ASPathfinding pathfinding;
    private static PathManager instance = null;
    private void Awake()
    {     
        Init();
    }

    private void Init()
    {
        #region Singleton
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        #endregion
        pathfinding = GetComponent<ASPathfinding>();
    }

    public void RequestPath(PathRequestData pathRequest)
    {
        pathfinding.FindPathFromAtoB(pathRequest);
    }
}

public struct PathRequestData
{
    public Vector3 startPos;
    public Vector3 finishPos;
    public Action<List<Vector3>> pathGetter;

    public PathRequestData(Vector3 _startPos, Vector3 _finishPos, Action<List<Vector3>> _pathGetter)
    {
        startPos = _startPos;
        finishPos = _finishPos;
        pathGetter = _pathGetter;
    }
}