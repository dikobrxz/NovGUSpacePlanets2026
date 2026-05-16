using UnityEngine;
using UnityEngine.Events;

public class ConsoleLogger : MonoBehaviour
{
    public string logMessage = "Объект активирован!";

    public void LogToConsole()
    {
        Debug.Log(logMessage);
    }

    public void LogToConsole(string customMessage)
    {
        Debug.Log(customMessage);
    }
}
