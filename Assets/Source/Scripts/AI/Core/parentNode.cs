using System.Collections.Generic;

public class parentNode : treeNode
{
	// Stores the list of all children node for this node
	List<treeNode> mChildren;

    public parentNode(BehaviorTree.Controllers.iNodeController i_NodeController)
	{
        // Initialize the children list
        mChildren = new List<treeNode>();

        // set the node controller
		mNodeController = i_NodeController;

        // Indicate the controlled node to the controller
		mNodeController.setControlledNode(this);
	}

    /**
     * @summary : Initializes the task in this node
     * */
    override public bool Start()
	{
		// Ask the controller to start itself up
		// if the startup is successfully finished
		if(mNodeController.Start())
		{
			// Go to execution
            Execute();
						
			// return with a succesful status
			return true;
		}
		// if the startup was unsusccesful
		else
		{
			// Bail out
			return false;
		}
	}
	
	/**
     * @summary : Executes the actions associated with this task
     * */
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
            setRunning();
			// return false to indicate that Execution has not finished in this frame
			return false;
		}
	}

    /**
     * @summary : Safely ends the task in this node
     * */
    override public bool End()
	{
		stoppedRunning();
		// Does there need to be a provision for unsuccesful ending ? 
		mNodeController.End ();
		
		return true;
	}

    /**
     * @summary : Adds a child to the Children list
     * @param name="i_childToBeAdded" : The child that is to be added to the list 
     * */
    public void addChild(treeNode i_childToBeAdded)
    {
        mChildren.Add(i_childToBeAdded);
    }

    /**
     * @summary : Returns the number of children of this node
     * */
    public int getChildCount() { return mChildren.Count;}

    /**
     * @summary : Starts up the indicated child node
     * @param name="i_childNodeIdx" : The id of the child to be started up
     * */
    public bool startChild(int i_childNodeIdx)
	{
		// If the indicated child exists
		if(i_childNodeIdx < mChildren.Count)
		{
			// Start it up
			return mChildren[i_childNodeIdx].Start();
			
		}else
		{
			// If any child exists
			if(mChildren.Count > 0)
				// Start it up
				return mChildren[0].Start();
			else 
				// if no child is available to start then bail out
				return false;
		}
	}

    /**
     * @summary : returns the indicated child as a treeNode object (may require conversion)
     * @param name="i_childNodeIdx" : The index of the child node to be returned
     */
    public treeNode getChild(int i_childNodeIdx)
	{
		// If the indicated child exists
		if(i_childNodeIdx < mChildren.Count)
		{
			// Return it
			return mChildren[i_childNodeIdx];
			
		}else
		{
			// otherwise indicate that there are no children availablt
			return null;
		}
	}
}

