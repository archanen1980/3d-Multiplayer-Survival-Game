using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_InputField emailInput; // New input field for email
    public Button loginButton;
    public Button registerButton; // New button for registration
    public PlayFabAuthManager authManager;

    void Start()
    {
        loginButton.onClick.AddListener(() =>
        {
            authManager.Login(usernameInput.text, passwordInput.text);
        });

        registerButton.onClick.AddListener(() =>
        {
            authManager.Register(usernameInput.text, passwordInput.text, emailInput.text);
        });
    }
}
