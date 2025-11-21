using UnityEngine;
using System;
using System.Collections.Generic;

public class JiraTaskManager : MonoBehaviour
{
    [Header("Dostêpne tickety (asset-y JiraTaskData)")]
    public List<JiraTaskData> possibleTasks;

    [Header("Parametry systemu")]
    public int maxActiveTasks = 3;
    public float minDelayBetweenTasks = 5f;
    public float maxDelayBetweenTasks = 15f;

    [Header("Aktywne tickety (podgl¹d)")]
    public List<ActiveJiraTask> activeTasks = new List<ActiveJiraTask>();

    private float taskTimer;

    // Eventy do UI / punktów
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

        // np. nie chcemy duplikatów tego samego assetu
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
        // proste równomierne losowanie – mo¿esz póŸniej dodaæ wagi, jeœli chcesz
        return possibleTasks[UnityEngine.Random.Range(0, possibleTasks.Count)];
    }

    /// <summary>
    /// Docelowa wersja – wo³aj to przy zabiciu NPC, gdy bêdziesz mia³ mechanikê killu.
    /// Na razie mo¿esz tego nie u¿ywaæ.
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

            // Forbidden – jeœli chcesz, mo¿esz traktowaæ je jako fail
            if (KillMatchesAnyTask(active.Data.Forbidden, killedBodyPart, usedItem))
            {
                active.IsFailed = true;
                completedNow.Add(active);
                Debug.Log($"JiraTaskManager: ticket odrzucony (Forbidden trafione)");
                continue;
            }

            // Required – czy ten kill pasuje do któregoœ warunku?
            bool matchesRequired = KillMatchesAnyTask(active.Data.Required, killedBodyPart, usedItem);
            if (!matchesRequired)
                continue;

            active.CurrentCount++;

            if (active.CurrentCount >= active.Data.Amount)
            {
                active.IsCompleted = true;
                TaskCompleted?.Invoke(active);
                completedNow.Add(active);

                // punkty za ukoñczenie ticketa
                if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.AddPoints(active.Data.Points);
                }

                Debug.Log($"JiraTaskManager: ticket ukoñczony! +{active.Data.Points} punktów");
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
    /// Debug: rêczne zwiêkszanie postêpu ticketa (do testowania UI bez zabijania NPC).
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

            // punkty za debugowe ukoñczenie ticketa
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddPoints(task.Data.Points);
            }

            activeTasks.Remove(task);
            TaskRemoved?.Invoke(task);

            Debug.Log($"[DEBUG] JiraTaskManager: ticket ukoñczony! +{task.Data.Points} punktów");
        }
    }

}
