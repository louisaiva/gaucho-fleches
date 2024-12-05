using UnityEngine;

public class Help : MonoBehaviour
{
    
    [Header("Help settings")]
    public string message;
    public RectTransform rect;
    public HelpHandler helpHandler;

    // START
    void Start()
    {
        // we get the components
        rect = GetComponent<RectTransform>();
        helpHandler = GameObject.Find("ui/help").GetComponent<HelpHandler>();
        helpHandler.RegisterHelp(this);
    }

    // LOG
    /* public void Log(string log)
    {
        helpHandler.RegisterLog(log);
    } */

}