using System.Collections;

namespace BehaviorTree.Controllers
{
    public abstract class conditionalRepeater : iNodeController
    {
        // indicates if the condition to be checked is true for this frame
        protected bool isConditionTrue = false;

        // Stores the node that is being repeated by this controller
        protected parentNode mRepeatedNode;

        // Constructor
        public conditionalRepeater() { }

        override public void setControlledNode(treeNode i_controlledNode)
        { mRepeatedNode = (parentNode)i_controlledNode; }

        /**
         * @Description : Checks if the condition is true
         * @Return : True if it is, False if it is not
         * */
        public abstract bool checkCondition();

        override public bool Start()
        {
            // if the condition is true
            if (checkCondition())
            {
                // Restart the node being repeated without considering the impact of repetition
                isConditionTrue = true;

                // The condition was true so restarting the child without considering what was running
                // Return the result of starting the child up for deciding whether to bail out or not
                return mRepeatedNode.startChild(0);
            }
            else
            {
                // If the condition was false but something was running then too return true so as to not bail out
                // However if the condition was false and nothing was running , return false
                return mRepeatedNode.isRunning();
            }
        }

        override public bool Execute()
        {
            // Since conditional repeaters only have one child, execute it
            return mRepeatedNode.getChild(0).Execute();
        }

        override public bool End()
        {
            isConditionTrue = false;
            return true;
        }

    }
}