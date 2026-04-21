using System;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
using System.IO.Ports;
using System.Text;
#endif
using UnityEngine;

public class SerialReader : MonoBehaviour
{
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
    private SerialPort sp;
    private StringBuilder serialBuffer = new StringBuilder();
#endif

    [Header("Mode")]
    public bool testMode = false;

    [Header("Serial")]
    public string portName = "COM4";
    public int baudRate = 9600;

    [Header("Angles")]
    public float armAngle = 0f;
    public float handAngle = 0f;
    public float wristAngle = 0f;

    [Header("Serial Monitor")]
    public bool showLogs = true;
    public int maxLogLines = 30;

    private string[] logBuffer;
    private int logIndex = 0;

    [Header("Serial Monitor Mode")]
    public bool serialMonitorMode = true;

    void Start()
    {
        logBuffer = new string[maxLogLines];

        if (testMode)
        {
            Debug.Log("Test mode enabled.");
            return;
        }

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        try
        {
            sp = new SerialPort(portName, baudRate);
            sp.ReadTimeout = 10;
            sp.Open();
            Log("[SYSTEM] Serial port opened");
        }
        catch (Exception e)
        {
            Debug.LogError("Serial open failed: " + e.Message);
        }
#endif
    }

    void Update()
    {
        if (testMode) return;

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        if (sp == null || !sp.IsOpen) return;

        try
        {
            if (sp.BytesToRead > 0)
            {
                string incoming = sp.ReadExisting();
                serialBuffer.Append(incoming);

                while (true)
                {
                    string bufferString = serialBuffer.ToString();
                    int newlineIndex = bufferString.IndexOf('\n');

                    if (newlineIndex < 0) break;

                    string line = bufferString.Substring(0, newlineIndex).Trim();
                    serialBuffer.Remove(0, newlineIndex + 1);

                    if (!string.IsNullOrEmpty(line))
                    {
                        Log("[RX] " + line);
                        ProcessData(line);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
            {
                SendResetCommand();
            }
        }
        catch (Exception e)
        {
            Log("[ERROR] " + e.Message);
        }
#endif
    }

    void ProcessData(string data)
    {
        if (data.StartsWith("S:"))
        {
            string payload = data.Substring(2);
            string[] parts = payload.Split(',');

            if (parts.Length >= 3)
            {
                float.TryParse(parts[0], out armAngle);
                float.TryParse(parts[1], out handAngle);
                float.TryParse(parts[2], out wristAngle);

                handAngle = -handAngle;
                wristAngle = -wristAngle;
            }
        }
    }

    public void SendTarget(float x, float y, float theta)
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        if (sp == null || !sp.IsOpen) return;

        string line = $"T:{x:F2},{y:F2},{theta:F2}\n";
        sp.Write(line);

        Log("[TX] " + line.Trim());
#endif
    }

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
    void SendResetCommand()
    {
        if (sp != null && sp.IsOpen)
        {
            sp.WriteLine("1");
            Log("[TX] RESET");
        }
    }
#endif

    void Log(string msg)
    {
        if (!serialMonitorMode) return;

        string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        string final = $"[{timestamp}] {msg}";

        logBuffer[logIndex] = final;
        logIndex = (logIndex + 1) % maxLogLines;

        Debug.Log(final);
    }

    void OnGUI()
    {
            if (!serialMonitorMode) return;
            if (!showLogs) return;

        GUILayout.BeginArea(new Rect(10, 10, 600, 400));
        GUILayout.Label("SERIAL MONITOR");

        for (int i = 0; i < maxLogLines; i++)
        {
            int idx = (logIndex + i) % maxLogLines;
            if (!string.IsNullOrEmpty(logBuffer[idx]))
                GUILayout.Label(logBuffer[idx]);
        }

        GUILayout.EndArea();
    }

    void OnApplicationQuit()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        if (sp != null && sp.IsOpen)
        {
            sp.Close();
        }
#endif
    }
}