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
    [SerializeField] private Text registerStatus;
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
        ShowLogin();
        FirebaseApp.CheckAndFixDependenciesAsync()
            .ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                loginStatus.text = "Firebase 준비 완료";
                registerStatus.text = "Firebase 준비 완료";
            }
            else
            {
                string msg = $"Firebase 초기화 실패: {task.Result}";
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
            loginStatus.text = "이메일과 비밀번호를 모두 입력하세요.";
            return;
        }

        loginButton.interactable = false;
        loginStatus.text = "로그인 중...";

        auth.SignInWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                string error = task.Exception?.GetBaseException().Message ?? "알 수 없는 오류";
                loginStatus.text = $"로그인 실패: {error}";
                loginButton.interactable = true;
            }
            else
            {
                loginStatus.text = $"환영합니다, {auth.CurrentUser.Email}";
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
            registerStatus.text = "이메일과 비밀번호를 모두 입력하세요.";
            return;
        }

        if (password != confirmPassword)
        {
            registerStatus.text = "비밀번호가 일치하지 않습니다.";
            return;
        }

        registerButton.interactable = false;
        registerStatus.text = "회원가입 중...";

        auth.CreateUserWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                string error = task.Exception?.GetBaseException().Message ?? "알 수 없는 오류";
                registerStatus.text = $"가입 실패: {error}";
                registerButton.interactable = true;
            }
            else
            {
                registerStatus.text = $"환영합니다, {auth.CurrentUser.Email}";
                SceneManager.LoadScene("MainMenu");
            }
        });
    }
}
