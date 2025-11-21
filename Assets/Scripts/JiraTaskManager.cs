using UnityEngine;
using System;
using System.Collections.Generic;

public class JiraTaskManager : MonoBehaviour
{
    [Header("Dost?pne tickety (asset-y JiraTaskData)")]
    public List<JiraTaskData> possibleTasks;

    [Header("Parametry systemu")]
    public int maxActiveTasks = 3;
    public float minDelayBetweenTasks = 5f;
    public float maxDelayBetweenTasks = 15f;

    [Header("Aktywne tickety (podgl?d)")]
    public List<ActiveJiraTask> activeTasks = new List<ActiveJiraTask>();

    private float taskTimer;

    // Eventy do UI / punkt?w
    public event Action<ActiveJiraTask> TaskAdded;
    public event Action<ActiveJiraTask> TaskCompleted;
    public event Action<ActiveJiraTask> TaskRemoved;

    private void Start()
    {
        ScheduleNextTask();
    }

    private void Update()
    {
        taskTimer -= Time.deltaTime;

        if (taskTimer <= 0f)
        {
            TrySpawnNewTask();
            ScheduleNextTask();
        }
    }

    private void ScheduleNextTask()
    {
        taskTimer = UnityEngine.Random.Range(minDelayBetweenTasks, maxDelayBetweenTasks);
    }

    private void TrySpawnNewTask()
    {
        if (possibleTasks == null || possibleTasks.Count == 0)
            return;

        if (activeTasks.Count >= maxActiveTasks)
            return;

        JiraTaskData chosen = GetRandomTask();
        if (chosen == null)
            return;

        // np. nie chcemy duplikat?w tego samego assetu
        foreach (var t in activeTasks)
        {
            if (t.Data == chosen)
                return;
        }

        var active = new ActiveJiraTask(chosen);
        activeTasks.Add(active);
        TaskAdded?.Invoke(active);

        Debug.Log($"JiraTaskManager: dodano ticket (Amount={chosen.Amount}, Points={chosen.Points})");
    }

    private JiraTaskData GetRandomTask()
    {
        // proste r?wnomierne losowanie ? mo?esz p?niej doda? wagi, je?li chcesz
        return possibleTasks[UnityEngine.Random.Range(0, possibleTasks.Count)];
    }

    /// <summary>
    /// Docelowa wersja ? wo?aj to przy zabiciu NPC, gdy b?dziesz mia? mechanik? killu.
    /// Na razie mo?esz tego nie u?ywa?.
    /// </summary>
    public void ReportKill(NpcPartData killedBodyPart, MurderousItemData usedItem)
    {
        if (activeTasks.Count == 0)
            return;

        List<ActiveJiraTask> completedNow = new List<ActiveJiraTask>();

        foreach (var active in activeTasks)
        {
            if (active.IsCompleted || active.IsFailed || active.Data == null)
                continue;

            // Forbidden ? je?li chcesz, mo?esz traktowa? je jako fail
            if (KillMatchesAnyTask(active.Data.Forbidden, killedBodyPart, usedItem))
            {
                active.IsFailed = true;
                completedNow.Add(active);
                Debug.Log($"JiraTaskManager: ticket odrzucony (Forbidden trafione)");
                continue;
            }

            // Required ? czy ten kill pasuje do kt?rego? warunku?
            bool matchesRequired = KillMatchesAnyTask(active.Data.Required, killedBodyPart, usedItem);
            if (!matchesRequired)
                continue;

            active.CurrentCount++;

            if (active.CurrentCount >= active.Data.Amount)
            {
                active.IsCompleted = true;
                TaskCompleted?.Invoke(active);
                completedNow.Add(active);

                // punkty za uko?czenie ticketa
                if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.AddPoints(active.Data.Points);
                }

                Debug.Log($"JiraTaskManager: ticket uko?czony! +{active.Data.Points} punkt?w");
            }
        }

        foreach (var t in completedNow)
        {
            activeTasks.Remove(t);
            TaskRemoved?.Invoke(t);
        }
    }

    private bool KillMatchesAnyTask(List<Task> tasks, NpcPartData killedBodyPart, MurderousItemData usedItem)
    {
        if (tasks == null || tasks.Count == 0)
            return false;

        foreach (var cond in tasks)
        {
            if (cond == null)
                continue;

            bool bodyOk = (cond.RequiredBodyPart == null || cond.RequiredBodyPart == killedBodyPart);
            bool itemOk = (cond.ItemToUse == null || cond.ItemToUse == usedItem);

            if (bodyOk && itemOk)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Debug: r?czne zwi?kszanie post?pu ticketa (do testowania UI bez zabijania NPC).
    /// </summary>
    public void DebugIncrementTask(ActiveJiraTask task)
    {
        if (task == null || task.IsCompleted || task.IsFailed || task.Data == null)
            return;

        task.CurrentCount++;

        if (task.CurrentCount >= task.Data.Amount)
        {
            task.IsCompleted = true;
            TaskCompleted?.Invoke(task);

            // punkty za debugowe uko?czenie ticketa
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddPoints(task.Data.Points);
            }

            activeTasks.Remove(task);
            TaskRemoved?.Invoke(task);

            Debug.Log($"[DEBUG] JiraTaskManager: ticket uko?czony! +{task.Data.Points} punkt?w");
        }
    }

}
