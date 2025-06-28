using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using TMPro;

public class LoginManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject registerPanel;

    [Header("Login UI")]
    [SerializeField] private InputField loginEmail;
    [SerializeField] private InputField loginPassword;
    [SerializeField] private Button loginButton;
    [SerializeField] private TMP_Text loginStatus;
    [SerializeField] private Button goToRegisterButton;

    [Header("Register UI")]
    [SerializeField] private InputField regEmail;
    [SerializeField] private InputField regPassword;
    [SerializeField] private InputField regConfirmPassword;
    [SerializeField] private Button registerButton;
    [SerializeField] private TMP_Text registerStatus;
    [SerializeField] private Button goToLoginButton;

    private FirebaseAuth auth;

    private void Awake()
    {
        loginButton.onClick.AddListener(OnLoginClicked);
        registerButton.onClick.AddListener(OnRegisterClicked);
        goToRegisterButton.onClick.AddListener(ShowRegister);
        goToLoginButton.onClick.AddListener(ShowLogin);
    }

    private void Start()
    {
        SoundManager.Instance.PlayBgm("BGM_MenuBgm");
        ShowLogin();
        FirebaseApp.CheckAndFixDependenciesAsync()
            .ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                loginStatus.text = "Login is ready";
                registerStatus.text = "Login is ready";
            }
            else
            {
                string msg = $"Firebase initialization failed: {task.Result}";
                loginStatus.text = msg;
                registerStatus.text = msg;
                loginButton.interactable = false;
                registerButton.interactable = false;
            }
        });
    }

    public void ShowLogin()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
        loginStatus.text = "";
    }

    public void ShowRegister()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
        registerStatus.text = "";
    }

    private void OnLoginClicked()
    {
        string email = loginEmail.text.Trim();
        string password = loginPassword.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            loginStatus.text = "Please enter both email and password.";
            return;
        }

        loginButton.interactable = false;
        loginStatus.text = "Logging in...";

        auth.SignInWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                string error = task.Exception?.GetBaseException().Message ?? "Unknown error";
                loginStatus.text = $"Login failed: {error}";
                loginButton.interactable = true;
            }
            else
            {
                loginStatus.text = $"Welcome, {auth.CurrentUser.Email}";
                SceneManager.LoadScene(1);
            }
        });
    }

    private void OnRegisterClicked()
    {
        string email = regEmail.text.Trim();
        string password = regPassword.text;
        string confirmPassword = regConfirmPassword.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            registerStatus.text = "Please enter both email and password.";
            return;
        }

        if (password != confirmPassword)
        {
            registerStatus.text = "Passwords do not match.";
            return;
        }

        registerButton.interactable = false;
        registerStatus.text = "Registering...";

        auth.CreateUserWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                string error = task.Exception?.GetBaseException().Message ?? "Unknown error";
                registerStatus.text = $"Registration failed: {error}";
                registerButton.interactable = true;
            }
            else
            {
                registerStatus.text = $"Welcome, {auth.CurrentUser.Email}";
                SceneManager.LoadScene("MainMenu");
            }
        });
    }
}
