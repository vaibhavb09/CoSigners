using UnityEngine;
using System.Collections;

public class LineEffect : MonoBehaviour {
	
	public int materialIndex = 1;
    private float uvAnimationRate = 0.005f;
    public string textureName = "_MainTex";
	public float newOffset = 0;
	public Vector2 uvOffset = Vector2.zero;
	private float ticker = 0;
	private float timer = 0.05f;
	public ThiefGrid.LinkState _state = ThiefGrid.LinkState.UNAVAILABLE;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log("offsetting");
		ticker += Time.deltaTime;
		if ( _state == ThiefGrid.LinkState.CONNECTED || _state == ThiefGrid.LinkState.POWERED || _state == ThiefGrid.LinkState.AVAILABLE )
		{
			if ( ticker > timer )
			{
				ticker = 0;
				newOffset += uvAnimationRate;
				uvOffset.Set( 0.0f, newOffset);
		        if( renderer.enabled )
		        {
		            renderer.materials[ 1 ].SetTextureOffset( textureName, uvOffset );
		        }
			}
		}
	}
	
	public void SetState( ThiefGrid.LinkState i_state )	
	{
		if ( i_state == ThiefGrid.LinkState.UNAVAILABLE )
		{
			renderer.enabled = false;
		}
		else{
			renderer.enabled = true;
		}
		
		_state = i_state;
		//Debug.Log("Set State to " + _state);
		if ( _state == ThiefGrid.LinkState.UNAVAILABLE )
		{
			Material[] mats = new Material[1];
			mats[0] = ThiefGrid.Manager.UnavailableMaterial;
			renderer.materials = mats;
		}
		else if ( _state == ThiefGrid.LinkState.AVAILABLE )
		{
			Material[] mats = new Material[2];
			mats[0] = ThiefGrid.Manager.AvailableMaterial;
			mats[1] = ThiefGrid.Manager.AvailableMaterial;
			renderer.materials = mats;
		}
		else if ( _state == ThiefGrid.LinkState.CONNECTED )
		{
			Material[] mats = new Material[2];
			mats[0] = ThiefGrid.Manager.ConnectedMaterial;
			mats[1] = ThiefGrid.Manager.ConnectedNoise;
			renderer.materials = mats;
		}
		else if ( _state == ThiefGrid.LinkState.POWERED)
		{
			Material[] mats = new Material[2];
			mats[0] = ThiefGrid.Manager.PoweredMaterial;
			mats[1] = ThiefGrid.Manager.PoweredNoise;
			renderer.materials = mats;
		}
		else if ( _state == ThiefGrid.LinkState.JAMMED)
		{
			Material[] mats = new Material[1];
			mats[0] = ThiefGrid.Manager.JammedMaterial;
			renderer.materials = mats;
		}
		//renderer.materials = mats;
	}
}
