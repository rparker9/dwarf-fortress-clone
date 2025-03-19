using System.Collections.Generic;
using UnityEngine;

namespace ECS
{
    // Goal evaluator and manager
    public class GoalManager : Component
    {
        private List<Goal> availableGoals = new List<Goal>();
        private Goal currentGoal;

        public void AddGoal(Goal goal)
        {
            availableGoals.Add(goal);
        }

        public void EvaluateGoals()
        {
            // Calculate priorities for all goals
            foreach (var goal in availableGoals)
            {
                goal.CalculatePriority();
            }

            // Find highest priority goal
            Goal highestPriorityGoal = null;
            float highestPriority = 0;

            foreach (var goal in availableGoals)
            {
                if (goal.Priority > highestPriority)
                {
                    highestPriority = goal.Priority;
                    highestPriorityGoal = goal;
                }
            }

            // If we should switch goals
            if (highestPriorityGoal != currentGoal && highestPriority > 0)
            {
                if (currentGoal != null)
                    currentGoal.Terminate();

                currentGoal = highestPriorityGoal;
                currentGoal.Activate();
            }
        }

        public void Process()
        {
            if (currentGoal != null)
            {
                // Check if current goal is completed
                if (currentGoal.IsCompleted())
                {
                    currentGoal.Terminate();
                    currentGoal = null;
                    EvaluateGoals();
                }
                else
                {
                    currentGoal.Process();
                }
            }
            else
            {
                EvaluateGoals();
            }
        }
    }
}
