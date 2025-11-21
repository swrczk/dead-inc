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


    private void Start()
    {
        NpcTypeKilledSignal.AddListener(OnNpcKilled);
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
            }
        }

        FillTasks();
    } 

    private void FillTasks()
    {
        for (var i = possibleTasks.Count - 1; i < maxActiveTasks; i++)
        {
            SpawnNewTask();
        }
    }

    private void SpawnNewTask()
    {
        var chosen = GetRandomTask();
        var task = Instantiate(jiraTaskPrefab, container);
        task.Init(chosen);
        activeTasks.Add(task); 

        Debug.Log($"JiraTaskManager: dodano ticket (Amount={chosen.Amount}, Points={chosen.Points})");
    } 

    private JiraTaskData GetRandomTask()
    {
        return possibleTasks[Random.Range(0, possibleTasks.Count)];
    } 
}