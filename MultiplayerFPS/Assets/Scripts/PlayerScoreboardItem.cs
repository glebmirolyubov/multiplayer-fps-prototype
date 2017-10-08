using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreboardItem : MonoBehaviour {

    [SerializeField]
    Text usernameText;

	[SerializeField]
	Text killsText;

	[SerializeField]
	Text deathsText;

    public void Setup (string username, int kills, int deaths) {
        usernameText.text = username;
        killsText.text = "Убийств: " + kills;
        deathsText.text = "Смертей: " + deaths;
    } 

}
