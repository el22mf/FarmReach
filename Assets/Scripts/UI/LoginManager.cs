using UnityEngine;
using UnityEngine.UI;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using System;
using TMPro;

[Serializable]
public class AuthRequest
{
    public string type;      // "login" or "register"
    public string username;
    public string password;
}

[Serializable]
public class AuthResponse
{
    public bool success;
    public string message;
    public string type;      // echo back "login" or "register"
}

public class LoginManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public Text feedbackText;
    public MainMenu mainMenu;

    [Header("ROS Topics")]
    public string requestTopic = "/auth_request";
    public string responseTopic = "/auth_response";

    private ROSConnection ros;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();

        ros.RegisterPublisher<StringMsg>("/auth_request");

        ros.Subscribe<StringMsg>(responseTopic, OnAuthResponse);

        feedbackText.text = "";
    }

    // -------------------- LOGIN --------------------
    public void AttemptLogin()
    {
        SendAuthRequest("login");
    }

    // -------------------- REGISTER --------------------
    public void AttemptRegister()
    {
        SendAuthRequest("register");
    }

    // -------------------- CORE SEND --------------------
    void SendAuthRequest(string type)
    {
        string username = usernameInput.text.Trim();
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            feedbackText.text = "Username or password cannot be empty.";
            return;
        }

        AuthRequest request = new AuthRequest
        {
            type = type,
            username = username,
            password = password
        };

        string json = JsonUtility.ToJson(request);

        StringMsg msg = new StringMsg(json);
        ros.Publish(requestTopic, msg);

        feedbackText.text = type == "login"
            ? "Attempting login..."
            : "Attempting registration...";
    }

    // -------------------- RESPONSE HANDLER --------------------
    void OnAuthResponse(StringMsg msg)
    {
        if (string.IsNullOrEmpty(msg.data))
        {
            Debug.LogWarning("Received empty response");
            feedbackText.text = "Error: empty response from server.";
            return;
        }

        AuthResponse response;

        try
        {
            response = JsonUtility.FromJson<AuthResponse>(msg.data);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to parse response JSON: " + e.Message);
            feedbackText.text = "Error parsing server response.";
            return;
        }

        if (response.success)
        {
            Debug.Log("Success: " + response.message);
            feedbackText.text = response.message;

            // Only trigger menu transition on login
            if (response.type == "login")
            {
                mainMenu.LoginConfirmed(usernameInput.text);
            }
        }
        else
        {
            Debug.LogWarning("Failed: " + response.message);
            feedbackText.text = response.message;
        }
    }
}