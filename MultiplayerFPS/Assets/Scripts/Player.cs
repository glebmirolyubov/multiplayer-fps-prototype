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

    [SerializeField]
    private GameObject[] disableGameObjectOnDeath;

    [SerializeField]
    private GameObject deathEffect;

	[SerializeField]
	private GameObject spawnEffect;

    public void Setup()
    {
        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++)
        {
            wasEnabled[i] = disableOnDeath[i].enabled;
        }

        SetDefaults();
    }

    private void Update()
    {
        if (!isLocalPlayer) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.K)) {
            RpcTakeDamage(9999);
        }
    }

    [ClientRpc]
    public void RpcTakeDamage (int _amount) {
        if (isDead) {
            return;
        }

        currentHealth -= _amount;

        Debug.Log(transform.name + " has now " + currentHealth + " health ");

        if (currentHealth <= 0) {
            Die();
        }
    }

    private void Die () {
        isDead = true;

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

        SetDefaults();

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

		//Switch cameras
		if (isLocalPlayer)
		{
            GameManager.instance.SetSceneCameraActive(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
		}

		//Create spawn effect
        GameObject _gfxIns = (GameObject)Instantiate(spawnEffect, transform.position, Quaternion.identity);
		Destroy(_gfxIns, 3f);
    }

}