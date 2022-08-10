using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TabletUI : MonoBehaviour
{
    [SerializeField] private MeshRenderer tabletBodyMesh;
    [SerializeField] private TMP_Text tabletText;
    [SerializeField] private int maxLines = 5;

    private List<string> lines;

    private void Awake()
    {
        lines = new List<string>();

        if (tabletText == null)
            tabletText = GetComponentInChildren<TMP_Text>();
        if (tabletBodyMesh == null)
            tabletBodyMesh = GetComponentInChildren<MeshRenderer>();
    }

    public void SetTabletActive(bool state)
    {
        tabletBodyMesh.enabled = state;
        tabletText.enabled = state;
    }

    public void UpdateText(string str)
    {
        tabletText.text = str;
    }

    public void WriteLine(string str)
    {
        if (lines.Count == maxLines)
        {
            lines.RemoveAt(0);
        }
        lines.Add(str);

        UpdateText(string.Join("\n", lines));
    }
}