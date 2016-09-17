// TODO : REMOVE DEBUGGING CODE
using System.Diagnostics;

namespace BehaviorTree.Controllers
{
    public class sequenceController : iNodeController
    {
        // Indicates the next child that needs to be executed / started 
        private int nextChild = -1;

        // Sequence controllers are never leaves they are always parent nodes
        protected parentNode mControlledParentNode;

        // Indicates whether one task of this sequence failing bails out or continues execution
        private bool mNonBlocking = false;

        /**
         * @summary : Constuctor
         * @param name="i_IsNonBlocking" : Indicates whether one task of this sequence failing bails out or continues execution
         * */
        public sequenceController(bool i_IsNonBlocking) { mNonBlocking = i_IsNonBlocking; }

        /**
         * @summary : Sets the node that is being managed by this controller
         * @param name="i_controlledNode" : Indicates the tree node that is being managed by this controller
         * */
        public override void setControlledNode(treeNode i_controlledNode)
        {
            mControlledParentNode = (parentNode)i_controlledNode;
        }

        /**
        * @summary : Initializes the next task to be executed by the sequence
        * */
        public override bool Start()
        {
            // If the sequence is in the middle of doing something
            if (nextChild > 0)
            {
                if (mControlledParentNode.getChild(nextChild).isRunning())
                    // Safely kill the task that was next up for running
                    mControlledParentNode.getChild(nextChild).End();

                // error recovery reqd ?
            }

            // if the sequence has been started, set the next child to be executed as the first one
            nextChild = 0;

            return true;
        }

        /**
        * @summary : Executes the actions associated with this task
        * */
        public override bool Execute()
        {
            // if the task was already running just execute it
            if (mControlledParentNode.getChild(nextChild).isRunning())
                // if the task execution finishes
                if (mControlledParentNode.getChild(nextChild).Execute())
                {
                    // start ececuting tasks after the one that was just finished in sequence
                    while (nextChild + 1 < mControlledParentNode.getChildCount() - 1)
                    {
                        nextChild++;
                        // Start the next child task
                        if (mNonBlocking || mControlledParentNode.startChild(nextChild))
                        {
                            // if it got started properly
                            // Execute it
                            if (mControlledParentNode.getChild(nextChild).Execute())
                                // if the execution finished succesfully this frame, fetch the next task
                                continue;
                            else
                            {
                                // returning false
                                return false;
                            }
                        }
                        else
                            // startup failed return true to bail out
                            return true;
                    }

                }
                // if no part of the sequence was running (should only happen if the sequence was reset to 0)
                else
                {
                    // TODO : REMOVE DEBUGGING CODE
                    Debug.Assert(nextChild == 0);

                    // start excecuting tasks after the one that was just finished in sequence
                    while (nextChild < mControlledParentNode.getChildCount() - 1)
                    {
                        // If the next child started up successfully
                        if (mNonBlocking || mControlledParentNode.startChild(nextChild))
                        {
                            // Execute it 
                            // If the execution finished succesfully this frame
                            if (mControlledParentNode.getChild(nextChild).Execute())
                            {
                                // get the next child for this sequence
                                nextChild++;
                                continue;
                            }
                            else
                            {
                                // if execution did not finish succesfully this frame indicate that the task is still running
                                mControlledParentNode.isRunning();

                                return false;
                            }
                        }
                        else
                            // startup failed return true to bail out
                            return true;
                    }
                }
            // TODO : REMOVE DEBUGGING CODE
            throw new System.ApplicationException("Code should not have reached here in te sequence controller, something is wrong");
        }

        /**
          * @summary : Ends this sequence gracefully
          * */
        public override bool End()
        {
            // kill the child that was currently running
            mControlledParentNode.getChild(nextChild).End();

            // reset the next child to a negative value
            nextChild = -1;

            // Indicate that nothing in this node is running 
            mControlledParentNode.stoppedRunning();

            return true;
        }
    }
}