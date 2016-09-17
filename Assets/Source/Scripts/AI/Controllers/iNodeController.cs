namespace BehaviorTree.Controllers
{
    public abstract class iNodeController
    {
        public iNodeController() { }

        // Initializes the task in this node
        public abstract bool Start();

        // Executes the actions associated with this task
        public abstract bool Execute();

        // Safely ends the task in this node
        public abstract bool End();

        // Used to set the node that is being controlled by this controller
        public abstract void setControlledNode(treeNode i_controlledNode);
    }
}