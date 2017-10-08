using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour {

    public Text killCount;
    public Text deathCount;

    private void Start()
    {
        if (UserAccountManager.isLoggedIn)
          UserAccountManager.instance.GetData(OnReceivedData);
    }

    void OnReceivedData (string data) {

        killCount.text = "Убийств: " + DataTranslator.DataToKills(data).ToString();
        deathCount.text = "Смертей: " + DataTranslator.DataToDeaths(data).ToString();
    }

}
