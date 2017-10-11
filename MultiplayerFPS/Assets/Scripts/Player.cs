﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour
{

	[SyncVar]
	private bool _isDead = false;
	public bool isDead
	{
		get { return _isDead; }
		protected set { _isDead = value; }
	}

	[SerializeField]
	private int maxHealth = 100;

	[SyncVar]
	private int currentHealth;

	public float GetHealthPct()
	{
		return (float)currentHealth / maxHealth;
	}

	[SyncVar]
	public string username = "Loading...";

	public int kills;
	public int deaths;

	[SerializeField]
	private Behaviour[] disableOnDeath;
	private bool[] wasEnabled;

	[SerializeField]
	private GameObject[] disableGameObjectsOnDeath;

	[SerializeField]
	private GameObject deathEffect;

	[SerializeField]
	private GameObject spawnEffect;

	private bool firstSetup = true;

	public void SetupPlayer()
	{
		if (isLocalPlayer)
		{
			//Switch cameras
			GameManager.instance.SetSceneCameraActive(false);
			GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
		}

		CmdBroadCastNewPlayerSetup();
	}

	[Command]
	private void CmdBroadCastNewPlayerSetup()
	{
		RpcSetupPlayerOnAllClients();
	}

	[ClientRpc]
	private void RpcSetupPlayerOnAllClients()
	{
		if (firstSetup)
		{
			wasEnabled = new bool[disableOnDeath.Length];
			for (int i = 0; i < wasEnabled.Length; i++)
			{
				wasEnabled[i] = disableOnDeath[i].enabled;
			}

			firstSetup = false;
		}

		SetDefaults();
	}

	//void Update()
	//{
	//  if (!isLocalPlayer)
	//      return;

	//  if (Input.GetKeyDown(KeyCode.K))
	//  {
	//      RpcTakeDamage(99999);
	//  }
	//}

	[ClientRpc]
	public void RpcTakeDamage(int _amount, string _sourceID)
	{
		if (isDead)
			return;

		currentHealth -= _amount;

		Debug.Log(transform.name + " now has " + currentHealth + " health.");

		if (currentHealth <= 0)
		{
			Die(_sourceID);
		}
	}

private void Die(string _sourceID)
	{
		isDead = true;

		Player sourcePlayer = GameManager.GetPlayer(_sourceID);
		if (_sourceID != null)
		{
			sourcePlayer.kills++;
			GameManager.instance.onPlayerKilledCallback.Invoke(username, sourcePlayer.username);
		}

		deaths++;

		//Disable components
		for (int i = 0; i < disableOnDeath.Length; i++)
		{
			disableOnDeath[i].enabled = false;
		}

		//Disable game objects
		for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
		{
			disableGameObjectsOnDeath[i].SetActive(false);
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
		if (isLocalPlayer)
		{
			GameManager.instance.SetSceneCameraActive(true);
			GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
		}

		Debug.Log(transform.name + " is Dead!");

		//Respawn method
		StartCoroutine(Respawn());

	}

	private IEnumerator Respawn()
	{
		yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

		Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
		transform.position = _spawnPoint.position;
		transform.rotation = _spawnPoint.rotation;

        Debug.Log(_spawnPoint.name);

		if (_spawnPoint.name == "DesertSpawnPoint1" || _spawnPoint.name == "DesertSpawnPoint2" || _spawnPoint.name == "DesertSpawnPoint3")
            TimeSwap.isDesert = true;

		if (_spawnPoint.name == "WinterSpawnPoint1" || _spawnPoint.name == "WinterSpawnPoint2" || _spawnPoint.name == "WinterSpawnPoint3")
			TimeSwap.isDesert = false;

		yield return new WaitForSeconds(0.1f);

		SetupPlayer();

		Debug.Log(transform.name + " respawned.");
	}

	public void SetDefaults()
	{
		isDead = false;

		currentHealth = maxHealth;

		//Enable the components
		for (int i = 0; i < disableOnDeath.Length; i++)
		{
			disableOnDeath[i].enabled = wasEnabled[i];
		}

		//Enable the gameobjects
		for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
		{
			disableGameObjectsOnDeath[i].SetActive(true);
		}

		//Enable the collider
		Collider _col = GetComponent<Collider>();
		if (_col != null)
			_col.enabled = true;

		//Create spawn effect
		GameObject _gfxIns = (GameObject)Instantiate(spawnEffect, transform.position, Quaternion.identity);
		Destroy(_gfxIns, 3f);
	}

}
