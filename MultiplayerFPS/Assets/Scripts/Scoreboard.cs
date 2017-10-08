using System.Collections;
using UnityEngine;

public class Scoreboard : MonoBehaviour {

    [SerializeField]
    GameObject playerScoreboardItem;

	[SerializeField]
    Transform playerScoreboardList;

    private void OnEnable()
    {
        // Get an array of players
        Player[] players = GameManager.GetAllPlayers();

        foreach (Player player in players) {
            GameObject itemGO = (GameObject)Instantiate(playerScoreboardItem, playerScoreboardList);
            PlayerScoreboardItem item = itemGO.GetComponent<PlayerScoreboardItem>();
            if (item != null) {
                item.Setup(player.username, player.kills, player.deaths);
            }
        }
    }

    private void OnDisable()
    {
        // Clean up list of items
        foreach (Transform child in playerScoreboardList) {
            Destroy(child.gameObject);
        }
    }

}
