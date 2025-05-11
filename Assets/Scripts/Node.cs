using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] float lineWidth;
    [SerializeField] Material movableMaterial;
    [SerializeField] Material checkingMaterial;

    public Unit currentUnit;
    public Coord pos;
    LineRenderer lineRenderer;

    public NodeStatus status = NodeStatus.Movable;
    private void Awake()
    {
        lineRenderer = transform.GetChild(0).GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.startWidth = lineRenderer.endWidth = lineWidth;
    }
    /// <summary>
    /// 현재 노드부터 도착노드끼지의 LineRenderer연결
    /// </summary>
    /// <param name="endPosition"></param>
    public void ShowLine(Vector3 endPosition)
    {
        lineRenderer.enabled = true;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, new Vector3(transform.position.x, transform.position.y + 0.26f, transform.position.z));
        lineRenderer.SetPosition(1, new Vector3(endPosition.x, endPosition.y + 0.26f, endPosition.z));
    }

    public void SetNodeStatus(NodeStatus status)
    {
        switch (status)
        {
            case NodeStatus.Movable:
                gameObject.layer = 7;
                GetComponent<Renderer>().material = movableMaterial;
                break;
            case NodeStatus.CheckingNode:
                this.status = status;
                GetComponent<Renderer>().material = checkingMaterial;
                break;
        }
    }
    public void RevertNode()
    {
        if(status == NodeStatus.Movable)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.layer = 9;
            GetComponent<Renderer>().material = checkingMaterial;
        }
    }
    public void ResetNode()
    {
        if(gameObject.activeSelf)
        {
            gameObject.layer = 7;
            status = NodeStatus.Movable;
            lineRenderer.enabled = false;
            GetComponent<Renderer>().material = movableMaterial;
            gameObject.SetActive(false);
        }
    }
}
public enum NodeStatus
{
    None, Movable, CheckingNode
}
