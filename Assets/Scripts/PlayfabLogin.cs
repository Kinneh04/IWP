using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class PlayfabLogin : MonoBehaviour
{

    [SerializeField] private TMP_InputField RegisterUsernameInput,RegisterEmailInput, RegisterPasswordInput, LoginUsernameInput, LoginPasswordInput, RegisterRepeatPasswordInput, RecoverEmailInputField;
    [SerializeField] private Button LoginButton, RegisterButton, LoginAfterRegisterButton;
    [SerializeField] private TMP_Text ResultText, OnRegisterSuccessTMP_Text;
    [SerializeField] private Toggle RememberMeToggle;
    [SerializeField] private GameObject PopUp, OnRegisterSuccessMenu;
    public string savedNewlyRegisteredPassword, SavedNewlyRegisteredUsername;
    public void ResetPassword()
    {
        if (string.IsNullOrEmpty(RecoverEmailInputField.text))
        {
            DisplayPopupText("Please enter a valid email address");
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

    public void EnterGameAsGuest()
    {
        SceneManager.LoadScene("Landing");
    }

    public void DisplayPopupText(string s)
    { 
        PopUp.SetActive(true);
        ResultText.text = s;
    }
    public void TryRegisterNewUser()
    {

        if (string.IsNullOrEmpty(RegisterEmailInput.text) || string.IsNullOrEmpty(RegisterPasswordInput.text) || string.IsNullOrEmpty(RegisterUsernameInput.text))
        {
            DisplayPopupText("ERROR: Please fill in all sections");
            return;
        }
        else if (RegisterRepeatPasswordInput.text != RegisterPasswordInput.text)
        {
            DisplayPopupText("ERROR: repeat Password does not match entered password!");
            return;
        }
        else if(RegisterUsernameInput.text.Contains("@") || RegisterUsernameInput.text.Contains("."))
        {
            DisplayPopupText("ERROR: Name cannot contain symbols. Only letters and numbers allowed!");
            return;
        }

        var request = new RegisterPlayFabUserRequest {

            Email = RegisterEmailInput.text.ToString(),
            Password = RegisterPasswordInput.text.ToString(),
            Username = RegisterUsernameInput.text.ToString(),
            DisplayName = RegisterUsernameInput.text.ToString()
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }

    public void AttemptLogin()
    {
        if (string.IsNullOrEmpty(LoginPasswordInput.text)) DisplayPopupText("Error: Please enter a password!");
        else if (string.IsNullOrEmpty(LoginUsernameInput.text)) DisplayPopupText("Error: Please enter a valid username / email address!");
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
    public void Start()
    {
        RegisterButton.onClick.AddListener(delegate { TryRegisterNewUser(); });
        LoginButton.onClick.AddListener(delegate { AttemptLogin(); });
        LoginAfterRegisterButton.onClick.AddListener(delegate { AttemptAutologinAfterRegister(SavedNewlyRegisteredUsername, savedNewlyRegisteredPassword); });
        //LogoutButton.onClick.AddListener(delegate { LogoutPlayer(); });
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            /*
            Please change the titleId below to your own titleId from PlayFab Game Manager.
            If you have already set the value in the Editor Extensions, this can be skipped.
            */
            PlayFabSettings.staticSettings.TitleId = "42";
        }
        AttemptReloginOnStart();
     
    }

    public void AttemptReloginOnStart()
    {
        string RecordedPassword = PlayerPrefs.GetString("Password");
        string RecordedUsername = PlayerPrefs.GetString("Username");
        if (RecordedPassword == "") return;
        else if (RecordedUsername != "" && RecordedPassword != "")
        {
            LoginUsernameInput.text = RecordedUsername;
            LoginPasswordInput.text = RecordedPassword;
            RememberMeToggle.isOn = true;
        }
    }
    public void OnResetPasswordSuccess(SendAccountRecoveryEmailResult S)
    {
        DisplayPopupText("Password reset instructions sent successfully. Check your email!");
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult R)
    {
        SavedNewlyRegisteredUsername = R.Username;
        savedNewlyRegisteredPassword = RegisterPasswordInput.text;
        DisplayOnRegisterSuccessMenu("New user registered successfully! Log in using this email?");
    }

    public void DisplayOnRegisterSuccessMenu(string s)
    {
        OnRegisterSuccessMenu.SetActive(true);
        OnRegisterSuccessTMP_Text.text = s;
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
        DisplayPopupText(error.GenerateErrorReport());
        PlayerPrefs.SetString("Password", "");
        PlayerPrefs.SetString("Email", "");
        PlayerPrefs.SetString("Username", "");
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        DisplayPopupText(error.GenerateErrorReport());
    }
}
