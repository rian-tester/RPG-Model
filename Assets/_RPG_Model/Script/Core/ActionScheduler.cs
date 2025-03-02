using UnityEngine;

namespace RPG.Core
{
    public  class ActionScheduler : MonoBehaviour
    {
        IAction currentAction;
        
        public void  StartAction(IAction action)
        {
            // if the current action is the same as the action we are trying to start, do nothing
            if (currentAction == action) return;

            // if there is a current action, cancel it
            if (currentAction != null) 
            {
                currentAction.Cancel();
                print("Cancelling " + currentAction);
            }

            // set the current action to the action we are trying to start
            currentAction = action;
        }

        // just passing null to StartAction method will cancel the current action
        public void CancelCurrentAction()
        {
            StartAction(null);
        }
    }
}
