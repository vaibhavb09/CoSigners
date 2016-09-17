using UnityEngine;
using System.Collections;

public class DrawLine : MonoBehaviour {
	//#MARK FOR DESTROY
	/* Not used anymore
	public Node[] ObjectList = new Node[2];
	public float LineWidth = 1;
	public Material playerCaptureLineMat;
	public Material canCaptureLineMat;
	public Material SecurityCaptureLineMat;
	public Material possibleMat;
	//draw mesh part
	private Mesh _mesh;
	private Vector3[] _vectices;
	private int[] _triangles;
	private Vector2[] _UV;
	private Vector3[] _normals;
	
	
	void Start()
	{
		
	}
	
	public void InitLine()
	{
		_mesh  = new Mesh();
		_triangles = new int[6];
		_vectices = new Vector3[4];
		_UV = new Vector2[4];
		_normals = new Vector3[4];
		_triangles[0] = 0;
		_triangles[1] = 1;
		_triangles[2] = 2;
		_triangles[3] = 2;
		_triangles[4] = 1;
		_triangles[5] = 3;
		_UV[0] = new Vector2(1,0);
		_UV[1] = new Vector2(1,1);
		_UV[2] = new Vector2(0,0);
		_UV[3] = new Vector2(0,1);
		_normals[0] = Vector3.up;
		_normals[1] = Vector3.up;
		_normals[2] = Vector3.up;
		_normals[3] = Vector3.up;
		
		Vector3 side = Vector3.Cross(Vector3.up, ObjectList[1].transform.position - ObjectList[0].transform.position);
		side.Normalize();
		_vectices[0] = ObjectList[0].transform.position + 0.1f * Vector3.down + side * (LineWidth/2);
		_vectices[1] = ObjectList[0].transform.position + 0.1f * Vector3.down + side * (LineWidth/-2);
		_vectices[2] = ObjectList[1].transform.position + 0.1f * Vector3.down + side * (LineWidth/2);
		_vectices[3] = ObjectList[1].transform.position + 0.1f * Vector3.down + side * (LineWidth/-2);
				
        GetComponent<MeshFilter>().mesh = _mesh;
        _mesh.vertices = _vectices;
        _mesh.uv = _UV;
        _mesh.triangles = _triangles;
		_mesh.normals = _normals;
		
		//Graphics.DrawMesh(_mesh, Vector3.zero, Quaternion.identity, matToDraw, 0);
	}
	
	void Update()
	{
		if((GameManager.Manager.CurrentDisplay & GameManager.Layer.Node) == GameManager.Layer.Node)
		{
			if((ObjectList[0]._securityState == GameManager.SecurityState.Tracking) ||
				(ObjectList[1]._securityState == GameManager.SecurityState.Tracking))
			{
				Graphics.DrawMesh(_mesh, Vector3.zero, Quaternion.identity, SecurityCaptureLineMat, 0);
			}
			else if((ObjectList[0]._state == GameManager.NodeState.Neutral) && 
				(ObjectList[1]._state == GameManager.NodeState.Neutral))
			{
				
				Graphics.DrawMesh(_mesh, Vector3.zero, Quaternion.identity, possibleMat, 0);
			}
			else if(
				((ObjectList[0]._state == GameManager.NodeState.Controlled) || 
				(ObjectList[0]._state >= GameManager.NodeState.Viewing) || 
				(ObjectList[0]._state == GameManager.NodeState.Releasing) ||
				(ObjectList[0]._state == GameManager.NodeState.Encrypting) ||
				(ObjectList[0]._state == GameManager.NodeState.Encrypted))
				&& 
				((ObjectList[1]._state == GameManager.NodeState.Controlled) || 
				(ObjectList[1]._state == GameManager.NodeState.Releasing) ||
				(ObjectList[1]._state == GameManager.NodeState.Encrypting) ||
				(ObjectList[1]._state == GameManager.NodeState.Encrypted) ||
				(ObjectList[1]._state >= GameManager.NodeState.Viewing)
				))
			{
				Graphics.DrawMesh(_mesh, Vector3.zero, Quaternion.identity, playerCaptureLineMat, 0);
					//gameObject.
					//gameObject.GetComponent<LineRenderer>().renderer.sharedMaterial = gameObject.renderer.sharedMaterial[0];
					//gameObject.renderer.sharedMaterials[0].color = Color.blue;
					//gameObject.GetComponent<LineRenderer>().SetColors(Color.blue, Color.green);
			}
			else if(
				(ObjectList[0]._state == GameManager.NodeState.Controlled) || 
				(ObjectList[1]._state == GameManager.NodeState.Controlled) || 
				(ObjectList[0]._state == GameManager.NodeState.Releasing) || 
				(ObjectList[1]._state == GameManager.NodeState.Releasing) ||
				(ObjectList[0]._state == GameManager.NodeState.Encrypting) || 
				(ObjectList[1]._state == GameManager.NodeState.Encrypting) ||
				(ObjectList[0]._state == GameManager.NodeState.Encrypted) || 
				(ObjectList[1]._state == GameManager.NodeState.Encrypted) ||
				(ObjectList[0]._state >= GameManager.NodeState.Viewing) || 
				(ObjectList[1]._state >= GameManager.NodeState.Viewing)
				)
			{
				Graphics.DrawMesh(_mesh, Vector3.zero, Quaternion.identity, canCaptureLineMat, 0);
					//gameObject.renderer.sharedMaterials[0].color = Color.white;
					//gameObject.GetComponent<LineRenderer>().renderer.sharedMaterial = gameObject.renderer.sharedMaterial[1];
					//gameObject.GetComponent<LineRenderer>().SetColors(Color.white, Color.white);				
			}
			else if((ObjectList[0]._state == GameManager.NodeState.Neutral) || 
				(ObjectList[1]._state == GameManager.NodeState.Neutral) ||
				(ObjectList[0]._state == GameManager.NodeState.Capturing) || 
				(ObjectList[1]._state == GameManager.NodeState.Capturing)
				)
			{
				Graphics.DrawMesh(_mesh, Vector3.zero, Quaternion.identity, possibleMat, 0);
			}
								
		}
	}
	*/
	
	/* the old one
	// Use this for initialization
	void Start () 
	{
		gameObject.GetComponent<LineRenderer>().SetPosition(0, ObjectList[0].transform.position - Vector3.up); 
		gameObject.GetComponent<LineRenderer>().SetPosition(1, ObjectList[1].transform.position - Vector3.up);
		
		if((ObjectList[0]._state == GameManager.NodeState.Released) && 
			(ObjectList[1]._state == GameManager.NodeState.Released))
		{
			gameObject.GetComponent<LineRenderer>().renderer.enabled = false;
				
		}
		else if((ObjectList[0]._state == GameManager.NodeState.PlayerCaptured) && 
				(ObjectList[1]._state == GameManager.NodeState.PlayerCaptured))
		{
			gameObject.GetComponent<LineRenderer>().renderer.enabled = true;
			gameObject.renderer.sharedMaterials[0].color = Color.blue;
			//gameObject.GetComponent<LineRenderer>().SetColors(Color.blue, Color.green);
		}
		else if((ObjectList[0]._state == GameManager.NodeState.PlayerCaptured) || 
				(ObjectList[1]._state == GameManager.NodeState.PlayerCaptured))
		{
			gameObject.GetComponent<LineRenderer>().renderer.enabled = true;
			gameObject.renderer.sharedMaterials[0].color = Color.clear;
			//gameObject.GetComponent<LineRenderer>().SetColors(Color.white, Color.white);				
		}
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(ObjectList[0].LineNeedUpdate() || ObjectList[1].LineNeedUpdate())
		{
			if((ObjectList[0]._state == GameManager.NodeState.Released) && 
				(ObjectList[1]._state == GameManager.NodeState.Released))
			{
				gameObject.GetComponent<LineRenderer>().renderer.enabled = false;
				
			}
			else if((ObjectList[0]._state == GameManager.NodeState.PlayerCaptured) && 
				(ObjectList[1]._state == GameManager.NodeState.PlayerCaptured))
			{
				gameObject.GetComponent<LineRenderer>().renderer.enabled = true;
				//gameObject.
				//gameObject.GetComponent<LineRenderer>().renderer.sharedMaterial = gameObject.renderer.sharedMaterial[0];
				//gameObject.renderer.sharedMaterials[0].color = Color.blue;
				//gameObject.GetComponent<LineRenderer>().SetColors(Color.blue, Color.green);
			}
			else if((ObjectList[0]._state == GameManager.NodeState.PlayerCaptured) || 
				(ObjectList[1]._state == GameManager.NodeState.PlayerCaptured))
			{
				gameObject.GetComponent<LineRenderer>().renderer.enabled = true;
				//gameObject.renderer.sharedMaterials[0].color = Color.white;
				//gameObject.GetComponent<LineRenderer>().renderer.sharedMaterial = gameObject.renderer.sharedMaterial[1];
				//gameObject.GetComponent<LineRenderer>().SetColors(Color.white, Color.white);				
			}
			//ObjectList[0].LineUpdateComplete();
			//ObjectList[1].LineUpdateComplete();
		}
	}
	*/
	
}
