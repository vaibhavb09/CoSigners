using System;
using System.Collections.Generic;

namespace BehaviorTree.Controllers
{
    abstract class conditionalExecutor : iNodeController
    {

        // indicates if the condition to be checked is true for this frame
        protected bool isConditionTrue = false;

        /**
         * @Description : Checks if the condition is true
         * @Return : True if it is, False if it is not
         * */
        public abstract bool checkCondition();

        // Stores the node that is being controlled by this controller
        protected parentNode mControlledNode;

        /**
         * @summary : Sets the node that is being managed by this controller
         * @param name="i_controlledNode" : The node that is being managed by this controller
         * */
        override public void setControlledNode(treeNode i_controlledNode)
        { mControlledNode = (parentNode)i_controlledNode; }

        override public bool Start()
        {
            // If nothing under this node is already running
            if (!mControlledNode.isRunning())
            {
                // and the condition is true
                if (checkCondition())
                {
                    isConditionTrue = true;
                    return mControlledNode.getChild(0).Start();
                }
                else
                {
                    isConditionTrue = false;
                    return false;
                }
            }
            else
            {
                isConditionTrue = checkCondition();
                return true;
            }
        }

        override public bool Execute()
        {
            // execute child task 
            return mControlledNode.getChild(0).Execute();
        }

        override public bool End()
        {
            mControlledNode.getChild(0).End();

            return true;
        }

    }
}
