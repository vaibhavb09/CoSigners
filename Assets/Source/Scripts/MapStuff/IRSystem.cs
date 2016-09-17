using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IRSystem : MonoBehaviour {
	
	
	private static IRSystem m_instance;
	
	public float irRadius;
	public bool IsIRActive;
    private GameObject[] nodes;
	/*
	List<Node> IRNodeList = new List<Node>();
	private Object node;
	
	// Use this for initialization
	void Start () {
		
		IsIRActive = false;
 		//nodes = GameObject.FindGameObjectsWithTag("Node");
		
	}
	
	public void AddIRNode(Node i_node)
	{
		//Debug.Log("ADD ONE IR");
		IRNodeList.Add(i_node);
		IsIRActive = true;
		irRadius = i_node.IRRadius;
	}
	
	public void RemoveIRNode(Node i_node)
	{
		//Debug.Log("REMOVE ONE IR");
		IRNodeList.Remove(i_node);
		if(IRNodeList.Count == 0)
		{
			IsIRActive = false;
		}
	}
	
	public bool IsThisIRActive(Node i_node)
	{
		foreach ( Node tempNode in IRNodeList)
		{
			if(tempNode.Index == i_node.Index)
			{
				return true;
			}			
		}		
		return false;
	}
	
	
	public bool HasActiveIRNearBy(Vector3 i_position){
		bool inRange = false;
		foreach ( Node tempNode in IRNodeList)
		{
			//Debug.Log("NodeList Not Empty");
			//Debug.Log(irRadius);
			Vector3 modifiedNode = new Vector3(tempNode.transform.position.x,0.0f,tempNode.transform.position.z);
			Vector3 modifiedGuardPos = new Vector3(i_position.x,0.0f,i_position.z);
		    if(((modifiedNode - modifiedGuardPos).sqrMagnitude) < (tempNode.IRRadius * tempNode.IRRadius))
			{
				inRange = true;
				break;
			}
		}
		return inRange;
	}
	*/
	public void Update()
	{	

	}

	
	public IRSystem () 
    {
        m_instance = this;
    }

	
	public static IRSystem ISystem
	{
		get
		{
			if(m_instance == null)
			{
				m_instance = new IRSystem();			
			}
			return m_instance;
		}
	}
	
}
