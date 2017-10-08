﻿using UnityEngine;

public class PlayerUI : MonoBehaviour {
    
    [SerializeField]
    RectTransform thrusterFuelFill;

    [SerializeField]
    GameObject pauseMenu;

	[SerializeField]
	GameObject scoreboard;

    private PlayerController controller;

    public void SetController (PlayerController _controller) {
        controller = _controller;
    }

    private void Start()
    {
        PauseMenu.isOn = false;
    }

    private void Update()
    {
        SetFuelAmount(controller.GetThrusterFuelAmount());

        if (Input.GetKeyDown(KeyCode.Escape)) {
            TogglePauseMenu();
        }

        if (Input.GetKeyDown(KeyCode.Tab)) {
            scoreboard.SetActive(true);
        } else if (Input.GetKeyUp(KeyCode.Tab)) {
            scoreboard.SetActive(false);
        }
    }

    public void TogglePauseMenu () {
        pauseMenu.SetActive(!pauseMenu.activeSelf);

        PauseMenu.isOn = pauseMenu.activeSelf;
    }

    void SetFuelAmount (float _amount) {
        thrusterFuelFill.localScale = new Vector3(1f, _amount, 1f);
    }

}
