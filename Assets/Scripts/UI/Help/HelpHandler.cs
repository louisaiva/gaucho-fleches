using System.Collections.Generic;
using System.Windows.Forms;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HelpHandler : MonoBehaviour
{
    [Header("Help settings")]
    bool show = true;

    [Header("Helps")]
    List<Help> helps = new List<Help>();

    [Header("Logs")]
    float log_time = -1000;
    float log_duration = 4;
    List<string> logs = new List<string>();

    [Header("Components")]
    public TextMeshProUGUI text;
    public Image bg;


    // START
    void Start()
    {
        // we get the components
        text = GetComponent<TextMeshProUGUI>();
        bg = transform.Find("bg").GetComponent<Image>();

        // we clear the logs
        logs.Clear();

        // we hide the help
        Hide();
    }

    public void RegisterHelp(Help help)
    {
        if (helps.Contains(help)) { return; }
        helps.Add(help);
    }
    public void RegisterLog(string log)
    {
        // we add the log
        logs.Add(log);
    }

    // UPDATE
    void Update()
    {
        // we check if we show at all
        if (!show) { Hide(); return; }

        // we check if we need to show some logs
        bool show_logs = UpdateLogs();
        if (show_logs) { Show(); return; }

        // we check if we need to show some helps
        bool show_help = UpdateHelps();
        if (show_help) { Show(); return; }

        // we hide
        Hide();
    }

    bool UpdateLogs()
    {
        // we check if we are still showing the log
        if (Time.time - log_time < log_duration) { return true; }

        // we check if we have logs
        if (logs.Count == 0) { return false; }

        // we show the next log
        SetText(logs[0]);
        logs.RemoveAt(0);

        // we set the time
        log_time = Time.time;

        return true;
    }
    bool UpdateHelps()
    {
        bool we_show_help = false;

        // we check if an Help is hovered
        foreach (Help help in helps)
        {
            // we check if the mouse is over the rect
            bool mouse_over = RectTransformUtility.RectangleContainsScreenPoint(help.rect, Input.mousePosition);
            if (mouse_over)
            {
                SetText(help.message);
                we_show_help = true;
                break;
            }
        }

        return we_show_help;
    }


    // main function
    private void SetText(string message)
    {
        text.text = message;
        text.ForceMeshUpdate();

        // we set the size
        Vector2 size = text.textBounds.size;
        GetComponent<RectTransform>().sizeDelta = size + new Vector2(20, 20);
        // bg.rectTransform.sizeDelta = size + new Vector2(20, 20);
    }

    // SHOW
    private void Show()
    {
        text.enabled = true;
        bg.enabled = true;
    }
    private void Hide()
    {
        text.enabled = false;
        bg.enabled = false;
    }
}