using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class UDPBroadCaster 
{

	//Socket _socket;
	UdpClient _udp;
	IPEndPoint _groupEP;
	string _localIP;
	public UDPBroadCaster()
	{

	}

	public void Init()
	{
		int GroupPort = 5436;
		_groupEP = new IPEndPoint(IPAddress.Broadcast, 5436);
		_udp = new UdpClient();

		
		IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
		foreach (IPAddress ip in host.AddressList)
		{
			if (ip.AddressFamily == AddressFamily.InterNetwork)
			{
				_localIP = ip.ToString();
			}
		}
	}

	public void BroadcastMessage(string i_message)
	{	
		byte[] sendBytes4 = Encoding.ASCII.GetBytes(_localIP + "#" + i_message);
		
		_udp.Send(sendBytes4, sendBytes4.Length, _groupEP);
	}

	public void CloseCaster()
	{
		_groupEP =  null;
		_udp.Close();
		_udp = null;
	}
	
}
