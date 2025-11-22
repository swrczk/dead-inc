using UnityEngine;
using System.Collections.Generic;

public class JiraTaskManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    public Transform container;

    [SerializeField]
    public JiraTaskUIEntry jiraTaskPrefab;

    [Header("Available Tasks")]
    public List<JiraTaskData> possibleTasks;

    [Header("Parametry systemu")]
    public int maxActiveTasks = 3;

    public List<JiraTaskUIEntry> activeTasks = new List<JiraTaskUIEntry>();

    private int _tasksTotalCounter = 0;

    private void Start()
    {
        NpcTypeKilledSignal.AddListener(OnNpcKilled);
        ClearChildren(container.gameObject);
        FillTasks();
    }

    private void OnNpcKilled(NpcData npc, MurderousItemData item)
    {
        foreach (var ticket in activeTasks)
        {
            if (ticket.TryUpdateProgress(npc, item))
            {
                JiraTicketCompleted.Invoke(ticket.Data.Points);
            }
        }

        for (var index = 0; index < activeTasks.Count; index++)
        {
            if (activeTasks[index].IsCompleted)
            {
                Destroy(activeTasks[index].gameObject);
                activeTasks.RemoveAt(index);
                index--; // żeby nie przeskoczyć elementu po RemoveAt
            }
        }

        FillTasks();
    }

    private void FillTasks()
    {
        for (var i = Mathf.Max(activeTasks.Count - 1, 0); i < maxActiveTasks; i++)
        {
            SpawnNewTask();
        }
    }

    private void ClearChildren(GameObject parent)
    {
        foreach (Transform child in parent.transform) Destroy(child.gameObject);
    }

    private void SpawnNewTask()
    {
        var chosen = GetRandomTask();
        var task = Instantiate(jiraTaskPrefab, container);
        task.Setup(chosen, ++_tasksTotalCounter);
        activeTasks.Add(task);

        Debug.Log($"JiraTaskManager: dodano ticket (Id={_tasksTotalCounter}, Points={chosen.Points})");
    }

    private JiraTaskData GetRandomTask()
    {
        return possibleTasks[Random.Range(0, possibleTasks.Count)];
    }
}