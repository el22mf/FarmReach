using UnityEngine;
using UnityEngine.UI;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.YourCustom; // replace with your service namespace

public class LoginManager : MonoBehaviour
{
    [Header("UI References")]
    public InputField usernameInput;
    public InputField passwordInput;
    public Text feedbackText;
    public MainMenu mainMenu;

    [Header("ROS Settings")]
    public string loginServiceName = "/login_user";
    public string registerServiceName = "/register_user";

    private ROSConnection ros;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        feedbackText.text = "";
    }

    // -------------------- LOGIN --------------------
    public void AttemptLogin()
    {
        string username = usernameInput.text.Trim();
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            feedbackText.text = "Username or password cannot be empty.";
            return;
        }

        var request = new LoginRequestMsg
        {
            username = username,
            password = password
        };

        ros.SendServiceMessage<LoginRequestMsg, LoginResponseMsg>(
            loginServiceName,
            request,
            OnLoginResponse
        );
    }

    void OnLoginResponse(LoginResponseMsg response)
    {
        if (response.success)
        {
            Debug.Log("Login successful: " + response.message);
            feedbackText.text = "Login successful!";
            mainMenu.LoginConfirmed(usernameInput.text);
        }
        else
        {
            Debug.LogWarning("Login failed: " + response.message);
            feedbackText.text = "Login failed: " + response.message;
        }
    }

    // -------------------- REGISTER --------------------
    public void AttemptRegister()
    {
        string username = usernameInput.text.Trim();
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            feedbackText.text = "Username or password cannot be empty.";
            return;
        }

        var request = new RegisterRequestMsg
        {
            username = username,
            password = password
        };

        ros.SendServiceMessage<RegisterRequestMsg, RegisterResponseMsg>(
            registerServiceName,
            request,
            OnRegisterResponse
        );
    }

    void OnRegisterResponse(RegisterResponseMsg response)
    {
        if (response.success)
        {
            Debug.Log("Registration successful: " + response.message);
            feedbackText.text = "Registration successful! You can now login.";
        }
        else
        {
            Debug.LogWarning("Registration failed: " + response.message);
            feedbackText.text = "Registration failed: " + response.message;
        }
    }
}