using UnityEngine;
using UnityEngine.UI;

public class UserAccount_Lobby : MonoBehaviour {

    public Text usernameText;

    private void Start()
    {
        if (UserAccountManager.isLoggedIn) {
            usernameText.text = "Вы вошли как: " + UserAccountManager.playerUsername;
        }
    }

    public void LogOut () {
        if (UserAccountManager.isLoggedIn) {
            UserAccountManager.instance.LogOut();
        }
    }

}
