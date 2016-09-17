using UnityEngine;
using System.Collections;

public class LANTestStuff : MonoBehaviour {

	UDPBroadCaster _Caster;
	UDPBroadcastReceiver _receiver;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void OnGUI()
	{
		GUI.depth = -1;
		if(GUI.Button(new Rect(0, 0, 150, 20), "Broadcast"))
		{
			if(_receiver != null) 
			{
				_receiver.CloseReceiver();
				_receiver = null;
			}
			if(_Caster == null) _Caster = new UDPBroadCaster();
			else
			{
				_Caster.BroadcastMessage("Hi yo");
			}
		}

		if(GUI.Button(new Rect(0, 20, 150, 20), "ReceiveBroadcast"))
		{
			if(_Caster != null)	
			{
				_Caster.CloseCaster();
				_Caster = null;
			}

			if(_receiver == null) _receiver =  new UDPBroadcastReceiver();

			if(_receiver != null)
			{
				_receiver.ReceiveMessage();
			}
		}

		if(_receiver != null)
			GUI.Label(new Rect(0, 40, 200, 20), _receiver.GetMessage());
	}
}
