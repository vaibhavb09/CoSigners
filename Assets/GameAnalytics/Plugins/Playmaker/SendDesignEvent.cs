#if false

using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("GameAnalytics")]
	[Tooltip("Sends a design event message to the GameAnalytics server")]
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1164")]
	public class SendDesignEvent : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The event ID")]
		public FsmString EventID;
		
		[Tooltip("The event value")]
		public FsmFloat EventValue;
		
		public override void Reset()
		{
			EventID = new FsmString() { UseVariable = false };
			EventValue = new FsmFloat() { UseVariable = true };
		}
		
		public override void OnEnter()
		{
			if (!EventValue.IsNone)
				GA.API.Design.NewEvent(EventID.Value, EventValue.Value);
			else
				GA.API.Design.NewEvent(EventID.Value);
			
			Finish();
		}
	}
}

#endif