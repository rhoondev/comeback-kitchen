using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] List<SectionManager> sectionManagers;
    [SerializeField] private Cookbook cookbook;

    private SectionManager _activeSectionManager;
    private int _activeSectionManagerIndex;

    private void Start()
    {
        SetActiveManager(0);
    }

    private void SetActiveManager(int index)
    {
        if (index >= 0 && index < sectionManagers.Count)
        {
            _activeSectionManager = sectionManagers[index];
            _activeSectionManagerIndex = index;

            foreach (var manager in sectionManagers)
            {
                manager.gameObject.SetActive(manager == _activeSectionManager);
            }

            _activeSectionManager.OnSectionCompleted += NextSection;
            _activeSectionManager.StartSection();
        }
        else
        {
            Debug.LogError("Invalid section manager index: " + index);
        }
    }

    private void NextSection()
    {
        SetActiveManager(_activeSectionManagerIndex + 1);
    }
}
