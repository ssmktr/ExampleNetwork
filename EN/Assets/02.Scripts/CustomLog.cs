using UnityEngine;
using System.Collections;
using System.Collections.Generic;

struct LogMessage
{
    public string message;
    public LogType type;
}

public class CustomLog : MonoBehaviour {

    public int MaxHistory = 50;
    List<LogMessage> Messages = new List<LogMessage>();
    Vector2 scrollPos = Vector2.zero;

    private void OnEnable()
    {
        Application.RegisterLogCallback(HandLog);
    }

    private void OnDisable()
    {
        Application.RegisterLogCallback(null);
    }

    private void OnGUI()
    {
        scrollPos = GUI.BeginScrollView(new Rect(100, 100, 500, 300), scrollPos, new Rect(105, 105, 490, 290));
        for (int i = 0; i < Messages.Count; ++i)
        {
            Color color = Color.white;
            if (Messages[i].type == LogType.Warning)
            {
                color = Color.yellow;
            }
            else if (Messages[i].type == LogType.Log)
            {
                color = Color.red;
            }

            GUI.color = color;
            GUI.Label(new Rect(105, 105 + i * 50, 490, 50), Messages[i].message);
        }
        GUI.EndScrollView();
    }

    void HandLog(string message, string stackTrace, LogType type)
    {
        LogMessage msg = new LogMessage();
        msg.message = message;
        msg.type = type;

        Messages.Add(msg);

        if (Messages.Count > MaxHistory)
        {
            Messages.RemoveAt(0);
        }

        scrollPos.y = 1000f;
    }
}
