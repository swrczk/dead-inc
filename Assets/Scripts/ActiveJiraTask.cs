using System;

[Serializable]
public class ActiveJiraTask
{
    public JiraTaskData Data;
    public int CurrentCount;
    public bool IsCompleted;
    public bool IsFailed;

    public ActiveJiraTask(JiraTaskData data)
    {
        Data = data;
        CurrentCount = 0;
        IsCompleted = false;
        IsFailed = false;
    }
}
