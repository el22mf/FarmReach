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

    void Start()
    {
        if (testMode)
        {
            Debug.Log("Test mode enabled. Using Inspector values.");
            return;
        }

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        try
        {
            sp = new SerialPort(portName, baudRate);
            sp.ReadTimeout = 10;
            sp.Open();
            Debug.Log("Serial port opened!");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to open serial port: " + e.Message);
        }
#endif
    }

    void Update()
    {
        if (testMode)
            return;

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        if (sp == null || !sp.IsOpen)
            return;

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

                    if (newlineIndex < 0)
                        break;

                    string line = bufferString.Substring(0, newlineIndex).Trim();
                    serialBuffer.Remove(0, newlineIndex + 1);

                    if (!string.IsNullOrEmpty(line))
                    {
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
            Debug.LogWarning("Serial read error: " + e.Message);
        }
#endif
    }

    void ProcessData(string data)
    {
        string[] parts = data.Split(',');

        if (parts.Length == 3)
        {
            float.TryParse(parts[0], out armAngle);
            float.TryParse(parts[1], out handAngle);
            float.TryParse(parts[2], out wristAngle);
            handAngle = -handAngle;
            wristAngle = -wristAngle;
        }
    }

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
    void SendResetCommand()
    {
        if (sp != null && sp.IsOpen)
        {
            sp.WriteLine("1");
            Debug.Log("Sent reset command: 1");
        }
    }
#endif

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