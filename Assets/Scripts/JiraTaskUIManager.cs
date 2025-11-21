using UnityEngine;
using System.Collections.Generic;

public class JiraTaskUIManager : MonoBehaviour
{
    [Header("Referencje")]
    public JiraTaskManager taskManager;
    public Transform entriesParent;           // np. Panel z VerticalLayoutGroup
    public JiraTaskUIEntry entryPrefab;

    // mapowanie: ActiveTask -> UI Entry
    private Dictionary<ActiveJiraTask, JiraTaskUIEntry> entryMap = new Dictionary<ActiveJiraTask, JiraTaskUIEntry>();

    private void OnEnable()
    {
        if (taskManager != null)
        {
            taskManager.TaskAdded += OnTaskAdded;
            taskManager.TaskCompleted += OnTaskCompleted;
            taskManager.TaskRemoved += OnTaskRemoved;
        }
    }

    private void OnDisable()
    {
        if (taskManager != null)
        {
            taskManager.TaskAdded -= OnTaskAdded;
            taskManager.TaskCompleted -= OnTaskCompleted;
            taskManager.TaskRemoved -= OnTaskRemoved;
        }
    }

    private void OnTaskAdded(ActiveJiraTask active)
    {
        if (entryPrefab == null || entriesParent == null || active == null)
            return;

        JiraTaskUIEntry entry = Instantiate(entryPrefab, entriesParent);
        entry.Init(active, taskManager);

        entryMap[active] = entry;
    }

    private void OnTaskCompleted(ActiveJiraTask active)
    {
        // Opcjonalny efekt wizualny przy ukoñczeniu ticketa
        if (entryMap.TryGetValue(active, out var entry) && entry != null)
        {
            // np. rozjaœnij badge punktów
            if (entry.pointsBackground != null)
            {
                entry.pointsBackground.color = Color.white;
            }

            // oraz zaktualizuj progress, gdyby coœ siê jeszcze nie odœwie¿y³o
            entry.UpdateProgressUI();
        }
    }

    private void OnTaskRemoved(ActiveJiraTask active)
    {
        if (entryMap.TryGetValue(active, out var entry) && entry != null)
        {
            Destroy(entry.gameObject);
        }

        entryMap.Remove(active);
    }
}
