﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class EnemyController : MonoBehaviour {
    
    [SerializeField]
    private LayerMask walkableMask;
    [SerializeField]
    public float speed;
    [SerializeField, Range(0.1f,5f)]
    private float delayBeforeNewPath;

    public event Action OnDamageDone;
    WaitForSeconds waitForNewPath;
    Coroutine movement;
    Coroutine moveTo;   
    Transform playerTransform;
    Transform thisTransform;
    PathManager pathManager;
    PathRequestData pathRequest;  
    Collider[] closeToPlayerNodes;
    Coroutine followPlayer;
    float inaccuracy;

    private void Awake()
    {
        Init();
        if (followPlayer != null)
            StopCoroutine(followPlayer);
        followPlayer = StartCoroutine(FollowPlayer());
    }
    
    private void Init()
    {
        waitForNewPath = new WaitForSeconds(delayBeforeNewPath);
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        pathManager = GameObject.FindGameObjectWithTag("AStar").GetComponent<PathManager>();
        thisTransform = transform;
        inaccuracy = GameController.instance.GetInaccuracyValue;
    }
 
    private IEnumerator FollowPlayer()
    {
        while (true)
        {
            yield return waitForNewPath;
            if (playerTransform == null)
            {
                playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            }
            closeToPlayerNodes = Physics.OverlapSphere(playerTransform.position, inaccuracy, walkableMask);
            int randomNode = UnityEngine.Random.Range(1, closeToPlayerNodes.Length-1);
            try
            {
                pathRequest = new PathRequestData(thisTransform.position, closeToPlayerNodes[randomNode].transform.position, PathSubmitted);
            }
            catch
            {
                print("<color=blue>Path request data can't be set up!</color>");
            }
            try
            {
                pathManager.RequestPath(pathRequest);
            }   
            catch
            {
                print("<color=blue>Path request failed!</color>");
            }
        }
    }

    public void PathSubmitted(List<Vector3> waypoints)
    {
        if(movement != null)
            StopCoroutine(movement);
        movement = StartCoroutine(Movement(waypoints));
    }

    private IEnumerator Movement(List<Vector3> worldPoints)
    {
        foreach (Vector3 point in worldPoints)
        {
            if(moveTo != null)            
                StopCoroutine(moveTo);           
            yield return moveTo = StartCoroutine(MoveTo(point + Vector3.up));
        }
    }

    private IEnumerator MoveTo(Vector3 movePoint)
    {
        while (thisTransform.position != movePoint)
        {
            float speedCurrent = speed * Time.deltaTime;
            thisTransform.position = Vector3.MoveTowards(thisTransform.position, movePoint, speedCurrent);
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (OnDamageDone != null)
                OnDamageDone();
        }
    }

    private void OnDisable()
    {
        OnDamageDone -= GameController.instance.OnPlayerDamaged;
    }

    private void OnDrawGizmos()
    {
        if (closeToPlayerNodes == null)
            return;
            foreach(Collider n in closeToPlayerNodes)
            {
                if (n == null)
                    return;
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(n.transform.position, new Vector3(1,1,1));
            }
    }

    public void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
