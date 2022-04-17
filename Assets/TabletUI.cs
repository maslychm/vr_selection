using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TabletUI : MonoBehaviour
{
    [SerializeField] private TMP_Text tabletText;
    [SerializeField] private int maxLines = 5;

    private List<string> lines;

    private void Awake()
    {
        lines = new List<string>();

        if (tabletText == null)
            tabletText = GetComponentInChildren<TMP_Text>();
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

        UpdateText(string.Join("\n",lines));
    }
}
