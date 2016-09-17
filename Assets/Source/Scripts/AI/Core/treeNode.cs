public abstract class treeNode {
	
	// Mantain a reference to the overlord here (for co-ordination)
	// protected GuardOverlord mOverlord;
    // put in a blackboard for the test
	
	// The controller for this node
    protected BehaviorTree.Controllers.iNodeController mNodeController;
	
	// Indicates if this task is currently running
	protected bool running;
		
	// Constructor for initial setup
	public treeNode(){}
	
	// Initializes the task in this node
	public abstract bool Start();
	
	// Executes the actions associated with this task
	public abstract bool Execute();
	
	// Safely ends the task in this node
	public abstract bool End();
	
	// checks to see if a task under this node is running
	public bool isRunning(){return running;}
	// Modifiers for the running indicating
	public void setRunning() {running = true;}
	public void stoppedRunning() {running = false;}
}
