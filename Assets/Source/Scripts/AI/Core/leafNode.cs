using System.Collections.Generic;

public class leafNode : treeNode
{
	// Constructor (NO I DO NOT EXCESSIVELY COMMENT MY SHIT ! )
    public leafNode(BehaviorTree.Controllers.iNodeController i_NodeController)
	{ 
        mNodeController = i_NodeController;

        mNodeController.setControlledNode(this);
    }
	
	// Initializes the task in this node
	override public bool Start()
	{
		// Ask the controller to start itself up
		// if the startup is successfully finished
		if(mNodeController.Start())
		{
			// Go to execution
			// if execution did not finish in this frame
			if(!Execute())
			{
				// Indicate that the task is still running
				setRunning();}
			
			// return with a succesful status
			return true;
		}
		// if the startup was unsusccesful
		else
		{
			// Bail out
			return false;
		}

        // CHANGE 
        // this start method can probably be replaced by just return mNodeController.Start()
	}
	
	// Executes the actions associated with this task
	override public bool Execute()
	{
		// Ask the controller to execute one frame
		// If the execution finished succesfully this frame
		if(mNodeController.Execute())
		{
			// End the task gracefully
			End ();
			// return true to bail out
			return true;
		}
		else
		{
			// return false to indicate that Execution has not finished in this frame
			return false;
		}
	}
	
	// Safely ends the task in this node
	override public bool End()
	{
		stoppedRunning();
		// Does there need to be a provision for unsuccesful ending ? 
		mNodeController.End ();
		
		return true;
	}
}
