using UnityEngine;
using TMPro;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using System;

public class AuthManagerUI : MonoBehaviour
{
    [Header("Sign up")]
    public UITransition signupScreen;
    public TMP_InputField nameInput, mobileInput, emailInput, passwordInput, confirmPasswordInput, areaInput;
    public TMP_Dropdown city, area;
    public GameObject countryCodeText;
    [Header ("Login")]
    public UITransition loginScreen;
    public TMP_InputField loginMobileInput, loginPasswordInput;
    public GameObject loginCountryCodeText;
    [Header ("Verify Mobile")]
    public UITransition verifyMobileScreen;
    public TMP_InputField code;
    public TMP_Text mobileNumber;

    private void Awake () {
        if (PlayerData.IsAuth) {
            AfterAuth ();
            return;
        }
        loginMobileInput.text = PlayerPrefs.GetString ("MobileNumber", "");
        loginPasswordInput.text = PlayerPrefs.GetString ("Password", "");
        if (!string.IsNullOrEmpty (loginMobileInput.text) && !string.IsNullOrEmpty (loginPasswordInput.text)) {
            loginCountryCodeText.SetActive (true);
            Login ();
        }
    }
    private void Start () {
        city.onValueChanged.AddListener((int option) => {
            areaInput.gameObject.SetActive (option != 0);
            area.gameObject.SetActive (option == 0);
        });
        mobileInput.onValueChanged.AddListener ((string mobile) => {
            countryCodeText.SetActive (!string.IsNullOrEmpty (mobile));
        });
        loginMobileInput.onValueChanged.AddListener ((string mobile) => {
            loginCountryCodeText.SetActive (!string.IsNullOrEmpty (mobile));
        });
    }
    private void OnDestroy () {
        city.onValueChanged.RemoveAllListeners ();
        mobileInput.onValueChanged.RemoveAllListeners ();
        loginMobileInput.onValueChanged.RemoveAllListeners ();
    }
    public void Login () {
        if (ValidateLogin ()) {
            loginScreen.GetComponent<CanvasGroup> ().interactable = false;
            WebRequests.Instance.Login (loginMobileInput.text, loginPasswordInput.text, (res) => {
                if(res.Value<int>("statusCode") == 200) {
                    PlayerPrefs.SetString ("MobileNumber", loginMobileInput.text);
                    PlayerPrefs.SetString ("Password", loginPasswordInput.text);
                    AfterAuth (res);
                } else {
                    loginScreen.GetComponent<CanvasGroup> ().interactable = true;
                }
            });
        }
    }
    public void SignUp () {
        if (mobileInput.text.Length == 10 && mobileInput.text.StartsWith ("05"))
            mobileInput.text = mobileInput.text.Substring (1);
        if(ValidateSignup()) {
            signupScreen.GetComponent<CanvasGroup> ().interactable = false;
            string area = areaInput.gameObject.activeInHierarchy ? areaInput.text : this.area.options[this.area.value].text;
            WebRequests.Instance.Register (nameInput.text, mobileInput.text, emailInput.text, passwordInput.text, city.options[city.value].text, area, (res) => {
                if (res.Value<int> ("statusCode") == 200) {
                    PlayerPrefs.SetString ("MobileNumber", mobileInput.text);
                    PlayerPrefs.SetString ("Password", passwordInput.text);
                    AfterAuth (res);
                } else {
                    signupScreen.GetComponent<CanvasGroup> ().interactable = true;
                }
            });
        }
    }
    public void VerifyMobile () {
        verifyMobileScreen.GetComponent<CanvasGroup> ().interactable = false;
        WebRequests.Instance.VerifyMobile (code.text, PlayerData.mobile, (res) => {
            if (res.Value<int> ("statusCode") == 200) {
                PlayerData.isVerified = true;
                AfterAuth ();
            } else {
                verifyMobileScreen.GetComponent<CanvasGroup> ().interactable = true;
            }
        });
    }
    private void AfterAuth (JObject res = null) {
        if(res != null)
            PlayerData.FillFromJson (res);
        loginScreen.Hide ();
        signupScreen.Hide ();
        verifyMobileScreen.Hide ();
        if (PlayerData.isVerified) {
            SceneManager.LoadScene (1, LoadSceneMode.Additive);
        } else {
            mobileNumber.text = "+971" + PlayerData.mobile;
            verifyMobileScreen.Show ();
            code.Select();
        }
    }

    private void sceneLoaded (AsyncOperation obj) {
        throw new NotImplementedException ();
    }

    private bool ValidateSignup () {
        Regex regex = new Regex (@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        if (string.IsNullOrEmpty(mobileInput.text) || string.IsNullOrEmpty (passwordInput.text) || string.IsNullOrEmpty (emailInput.text) || string.IsNullOrEmpty (nameInput.text) || string.IsNullOrEmpty (confirmPasswordInput.text)) {
            VLogger.ShowMessage("Fill all details");
            return false;
        }else if (passwordInput.text != confirmPasswordInput.text) {
            VLogger.ShowMessage("Password doesn't match");
            return false;
        } else if (!mobileInput.text.StartsWith ("5") || mobileInput.text.Length != 9) {
            VLogger.ShowMessage("Mobile not valid!");
            return false;
        } else if (!regex.IsMatch(emailInput.text)){
            VLogger.ShowMessage("Email not valid!");
            return false;
        }
        return true;
    }
    private bool ValidateLogin () {
        if (string.IsNullOrEmpty (loginMobileInput.text) || string.IsNullOrEmpty (loginPasswordInput.text)) {
            VLogger.ShowMessage("Fill all details");
            return false;
        } else if (!loginMobileInput.text.StartsWith ("5") || loginMobileInput.text.Length != 9) {
            VLogger.ShowMessage("Mobile not valid!");
            return false;
        }
        return true;
    }
}
