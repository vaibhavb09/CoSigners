using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class StateObject {
	// Client socket.
	public Socket workSocket = null;
	// Size of receive buffer.
	public const int BufferSize = 256;
	// Receive buffer.
	public byte[] buffer = new byte[BufferSize];
	// Received data string.
	public StringBuilder sb = new StringBuilder();
}

public class UDPBroadcastReceiver {

	Socket _socket;
	string _data;
	EndPoint _ep;
	private bool _launching = false;
	private bool _started = true;
	// State object for receiving data from remote device.


	public UDPBroadcastReceiver()
	{
		//int recv = _socket.ReceiveFrom(data, ref ep);
		//_data = Encoding.ASCII.GetString(data, 0, 1024);
		//Debug.Log("received: " + _data + "from: " + _ep.ToString());
		//data = new byte[1024];
		//_socket.BeginReceiveFrom(data, 0, data.Length, SocketFlags.None, ref _ep, new AsyncCallback(ReceiveCallback),  state);
		//_data = Encoding.ASCII.GetString(data, 0, 1024);
		//Debug.Log("received: " + _data + "from: " + _ep.ToString());
	}

	public bool IsBound()
	{
		return _socket.IsBound;
	}

	public void Init()
	{
		_socket = new Socket(AddressFamily.InterNetwork,
		                     SocketType.Dgram, ProtocolType.Udp);
		IPEndPoint iep = new IPEndPoint(IPAddress.Any, 5436);
		_socket.Bind(iep);
		_ep = (EndPoint)iep;
		
		
		//Debug.LogError("Ready to receive…");
		byte[] data = new byte[1024];
		
		StateObject state = new StateObject();
		state.workSocket = _socket;
		
		_socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback),  state);
	}

	public void ReceiveMessage()
	{
		StateObject state = new StateObject();
		state.workSocket = _socket;
		byte[] data = new byte[1024];
		//_socket.ReceiveFromAsync();
		_socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback),  state);
		//_data = Encoding.ASCII.GetString(data, 0, 1024);
		//Debug.Log("received: " + _data + "from: " + _ep.ToString());
	}

	private void ReceiveCallback( IAsyncResult ar ) 
	{
		StateObject obj = ar.AsyncState as StateObject;

		//IPEndPoint iep = (IPEndPoint)(_socket.RemoteEndPoint);
		//if(iep == null)
		//{
		//	Debug.Log("Remote EndPoint is null");
		//}
		int bytesRead = _socket.EndReceive(ar);

		if(bytesRead > 0)
		{
			_data = Encoding.ASCII.GetString(obj.buffer, 0, bytesRead);

			//Debug.LogError("Getting Called?" + ar.IsCompleted + "Data: " + _data);
		}
		else
		{
			//Debug.Log("No bytes read");
		}
	}


	public string GetMessage()
	{
		if(_data != null)	return _data;
		return " ";
	}

	public void CloseReceiver()
	{
		if(_socket.Connected)
		{
			_socket.Shutdown(SocketShutdown.Both);
			_socket.Disconnect(true);
		}
		_socket.Close();
		_socket = null;
	}
}
