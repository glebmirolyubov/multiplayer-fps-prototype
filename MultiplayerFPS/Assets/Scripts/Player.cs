﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour {

    [SyncVar]
    private bool _isDead = false;
    public bool isDead {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SerializeField]
    private int maxHealth = 100;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

	[SyncVar]
	private int currentHealth;

    public int kills;
    public int deaths;

    [SerializeField]
    private GameObject[] disableGameObjectOnDeath;

    [SerializeField]
    private GameObject deathEffect;

	[SerializeField]
	private GameObject spawnEffect;

    private bool firstSetup = true;

    public void SetupPlayer()
    {
        if (isLocalPlayer) {
			//Switch cameras
			GameManager.instance.SetSceneCameraActive(false);
			GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);

			CmdBroadcastNewPlayerSetup();
        }
    }

    [Command]
    private void CmdBroadcastNewPlayerSetup () {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients () {
        if (firstSetup) {
			wasEnabled = new bool[disableOnDeath.Length];
			for (int i = 0; i < wasEnabled.Length; i++)
			{
				wasEnabled[i] = disableOnDeath[i].enabled;
			}
            firstSetup = false;
        }

		SetDefaults();
    }

    /*
    private void Update()
    {
        if (!isLocalPlayer) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.K)) {
            RpcTakeDamage(9999);
        }
    }
    */

    [ClientRpc]
    public void RpcTakeDamage (int _amount, string _sourceID) {
        if (isDead) {
            return;
        }

        currentHealth -= _amount;

        Debug.Log(transform.name + " has now " + currentHealth + " health ");

        if (currentHealth <= 0) {
            Die(_sourceID);
        }
    }

    private void Die (string _sourceID) {
        isDead = true;

        Player sourcePlayer = GameManager.GetPlayer(_sourceID);

        if (_sourceID != null) {
            sourcePlayer.kills++;
        }

        deaths++;

        //Disable components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        //Disable game objects
        for (int i = 0; i < disableGameObjectOnDeath.Length; i++)
		{
            disableGameObjectOnDeath[i].SetActive(false);
		}

        //Disable collider
		Collider _col = GetComponent<Collider>();
		if (_col != null)
		{
            _col.enabled = false;
		}

        //Spawn a death effect
        GameObject _gfxIns = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(_gfxIns, 3f);

        //Switch cameras
        if (isLocalPlayer) {
            GameManager.instance.SetSceneCameraActive(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }

        Debug.Log(transform.name + " is Dead!");

        //Respawn method
        StartCoroutine(Respawn());

    }

    private IEnumerator Respawn () {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        yield return new WaitForSeconds(0.1f);
		
        SetupPlayer();

        Debug.Log(transform.name + " respawned.");
    }


    public void SetDefaults () {
        isDead = false;

        currentHealth = maxHealth;

        // Enable components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

		for (int i = 0; i < disableGameObjectOnDeath.Length; i++)
		{
            disableGameObjectOnDeath[i].SetActive(true);
		}

        // Enable collider
        Collider _col = GetComponent<Collider>();
        if (_col != null){
            _col.enabled = true;
        }

		//Create spawn effect
        GameObject _gfxIns = (GameObject)Instantiate(spawnEffect, transform.position, Quaternion.identity);
		Destroy(_gfxIns, 3f);
    }

}