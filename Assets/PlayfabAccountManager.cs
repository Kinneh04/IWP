using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab.ServerModels;
using PlayFab;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayfabAccountManager : MonoBehaviour
{
    [Header("AccountOptions")]
    public GameObject SignedInAccountOptions;
    public GameObject GuestAccountOptions;

    [Header("LoginPage")]
    public GameObject LoginPage;
    [SerializeField]
    private Button LoginButton;
    [SerializeField] private TMP_InputField LoginUsernameInput, LoginPasswordInput;
    [SerializeField] private Toggle RememberMeToggle;
    [SerializeField] private Toggle AutoLoginToggle;
    public TMP_Text LoginFeedback;

    [Header("SignUpPage")]
    public GameObject SignUpPage;
    [SerializeField]
    private TMP_InputField RegisterUsernameInput, RegisterEmailInput, RegisterPasswordInput, RegisterRepeatPasswordInput;
    public Button RegisterButton;
    public string savedNewlyRegisteredPassword, SavedNewlyRegisteredUsername;
    public TMP_Text SignUpFeedback;
    [SerializeField] private GameObject OnRegisterSuccessMenu;

    [Header("Recovery")]
    public TMP_InputField RecoverEmailInputField;
    public TMP_Text RecoverEmailFeedback;
    public void OpenAccountOptions()
    {
        if(PlayFabClientAPI.IsClientLoggedIn())
        {
            SignedInAccountOptions.SetActive(true);
        }
        else
        {
            GuestAccountOptions.SetActive(true);
        }
    }

    public void OpenLoginPage()
    {
        LoginPage.SetActive(true);
    }

    public void OpenSignUpPage()
    {
        SignUpPage.SetActive(true);
    }
    public void TryRegisterNewUser()
    {

        if (string.IsNullOrEmpty(RegisterEmailInput.text) || string.IsNullOrEmpty(RegisterPasswordInput.text) || string.IsNullOrEmpty(RegisterUsernameInput.text))
        {
            DisplayPopupText(SignUpFeedback,"ERROR: Please fill in all sections");
            return;
        }
        else if (RegisterRepeatPasswordInput.text != RegisterPasswordInput.text)
        {
            DisplayPopupText(SignUpFeedback, "ERROR: repeat Password does not match entered password!");
            return;
        }
        else if (RegisterUsernameInput.text.Contains("@") || RegisterUsernameInput.text.Contains("."))
        {
            DisplayPopupText(SignUpFeedback, "ERROR: Name cannot contain symbols. Only letters and numbers allowed!");
            return;
        }

        var request = new RegisterPlayFabUserRequest
        {

            Email = RegisterEmailInput.text.ToString(),
            Password = RegisterPasswordInput.text.ToString(),
            Username = RegisterUsernameInput.text.ToString(),
            DisplayName = RegisterUsernameInput.text.ToString()
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }
    public void AttemptLogin()
    {
        if (string.IsNullOrEmpty(LoginPasswordInput.text)) DisplayPopupText(LoginFeedback, "Error: Please enter a password!");
        else if (string.IsNullOrEmpty(LoginUsernameInput.text)) DisplayPopupText(LoginFeedback, "Error: Please enter a valid username / email address!");
        else if (LoginUsernameInput.text.Contains("@") || LoginUsernameInput.text.Contains("."))
        {
            LoginViaEmail();
            AttemptSaveLoginDetails();
        }
        else
        {
            LoginViaPlayfab();
            AttemptSaveLoginDetails();
        }
    }
    private void OnRegisterSuccess(RegisterPlayFabUserResult R)
    {
        SavedNewlyRegisteredUsername = R.Username;
        savedNewlyRegisteredPassword = RegisterPasswordInput.text;
        DisplayOnRegisterSuccessMenu();
    }
    public void DisplayOnRegisterSuccessMenu()
    {
        OnRegisterSuccessMenu.SetActive(true);
    }
    public void AttemptAutologinAfterRegister(string UN, string PW)
    {
        if (UN.Contains("@") || UN.Contains("."))
        {
            LoginViaEmailSet(UN, PW);
            AttemptSaveLoginDetails();
        }
        else
        {
            LoginViaPlayfabSet(UN, PW);
            AttemptSaveLoginDetails();
        }
    }

    public void AttemptSaveLoginDetails()
    {
        if (RememberMeToggle.isOn)
        {
            PlayerPrefs.SetString("Username", LoginUsernameInput.text.ToString());
            PlayerPrefs.SetString("Password", LoginPasswordInput.text.ToString());
            Debug.LogError("PasswordSaved!");
        }
        else
        {
            PlayerPrefs.SetString("Password", "");
            PlayerPrefs.SetString("Username", "");
            Debug.LogError("Password UNSAVED!");
        }
    }

    public void LoginViaEmailSet(string UN, string PW)
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = UN,
            Password = PW
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    public void LoginViaPlayfabSet(string UN, string PW)
    {
        var request = new LoginWithPlayFabRequest
        {
            Username = UN,
            Password = PW
        };

        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginFailure);
    }

    public void LoginViaEmail()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = LoginUsernameInput.text,
            Password = LoginPasswordInput.text
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    public void LoginViaPlayfab()
    {
        var request = new LoginWithPlayFabRequest
        {
            Username = LoginUsernameInput.text,
            Password = LoginPasswordInput.text
        };

        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginFailure);
    }
    public void DisplayPopupText(TMP_Text text, string s)
    {
        text.gameObject.SetActive(true);
        text.text = s;
    }

    private void OnLoginSuccess(LoginResult result)
    {
        SceneManager.LoadScene("Landing");

        //ShowResultText("Logged in successfully!");
        //LogoutButton.gameObject.SetActive(true);
        //GameMenu.SetActive(true);
        //LoginMenu.SetActive(false);
        //PFLB.GetMessageOfTheDay();

    }

    //private void LogoutPlayer()
    //{
    //    DisplayPopupText("Player logged out successfully!");
    //    LogoutButton.gameObject.SetActive(false);
    //    GameMenu.SetActive(false);
    //    LoginMenu.SetActive(true);
    //    PlayFabClientAPI.ForgetAllCredentials();
    //}

    private void OnLoginFailure(PlayFabError error)
    {
        DisplayPopupText(LoginFeedback, error.GenerateErrorReport());
        PlayerPrefs.SetString("Password", "");
        PlayerPrefs.SetString("Email", "");
        PlayerPrefs.SetString("Username", "");
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        DisplayPopupText(SignUpFeedback, error.GenerateErrorReport());
    }

    public void ResetPassword()
    {
        if (string.IsNullOrEmpty(RecoverEmailInputField.text))
        {
            DisplayPopupText(RecoverEmailFeedback, "Please enter a valid email address");
        }
        else
        {
            var request = new SendAccountRecoveryEmailRequest
            {
                Email = RecoverEmailInputField.text,
                TitleId = PlayFabSettings.staticSettings.TitleId

            };

            PlayFabClientAPI.SendAccountRecoveryEmail(request, OnResetPasswordSuccess, OnLoginFailure);
        }
    }

    public void OnResetPasswordSuccess(SendAccountRecoveryEmailResult S)
    {
        DisplayPopupText(RecoverEmailFeedback, "Password reset instructions sent successfully. Check your email!");
    }
}
