using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Net.Mail;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;



class EmailClient
{
		
	private static EmailClient _instance; 
	//This is the email address from which the email will be sent.
	private string defaultID = "cyberheistest@yahoo.com"; 
	private string defaultPassword = "Hacknhide1";
	
	// Use this for initialization
	void Start () 
	{
	}
	
	public static EmailClient Manager
	{
		get
		{
			if(_instance == null)
			{
				_instance = new EmailClient();			
			}
			return _instance;
		}
	}

	public EmailClient () 
    { 
        _instance = this;
    }
	
	public void SendEmail( string i_receipnts, string i_subject, string i_body )
	{
		MailMessage message = new MailMessage();
		message.To.Add(i_receipnts);
		message.Subject = i_subject;
		message.Body = i_body;
		message.From = new MailAddress("cyberheistest@yahoo.com");
		SmtpClient smtp = new SmtpClient("smtp.mail.yahoo.com", 587);
		smtp.EnableSsl = true;
		smtp.UseDefaultCredentials = false;
		smtp.Credentials = new NetworkCredential( defaultID ,defaultPassword);
		ServicePointManager.ServerCertificateValidationCallback = 
                delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) 
                    { return true; };
		smtp.SendAsync(message, null);
		//Debug.Log("Email sent successfully");
	}

}