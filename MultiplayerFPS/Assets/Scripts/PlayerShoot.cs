using UnityEngine;
using UnityEngine.Networking;

[RequireComponent (typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour {

    private const string PLAYER_TAG = "Player";

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;

    private PlayerWeapon currentWeapon;
    private WeaponManager weaponManager;

	private void Start()
    {
        if (cam == null){
            Debug.Log("PlayerShoot: no camera referenced");
            this.enabled = false;
        }

        weaponManager = GetComponent<WeaponManager>();
    }

    private void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();

        if (PauseMenu.isOn) {
            return;
        }

        if (currentWeapon.fireRate <= 0f) {
			if (Input.GetButtonDown("Fire1"))
			{
				Shoot();
			}
        } else {
            if (Input.GetButtonDown("Fire1")) {
                InvokeRepeating("Shoot", 0f, 1f/currentWeapon.fireRate);
            } else if (Input.GetButtonUp("Fire1")) {
                CancelInvoke("Shoot");
            }
        } 
    }

    //Called on the server when the player shoots
    [Command]
    void CmdOnShoot() {
        RpcDoShootEffect();
    }

    //Called on all clients when need to do shoot effect
    [ClientRpc]
    void RpcDoShootEffect() {
        weaponManager.GetCurrentGraphics().muzzleFlash.Play();
    }

    //Called on the server when we hit smth
    //Takes in the hit point and normal of the surface
	[Command]
    void CmdOnHit(Vector3 _pos, Vector3 _normal)
	{
        RpcDoHitEffect(_pos, _normal);
	}

    //Is called on all clients
    //Spawn in cool effects
	[ClientRpc]
    void RpcDoHitEffect(Vector3 _pos, Vector3 _normal)
	{
        GameObject _hitEffect = (GameObject)Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, _pos, Quaternion.LookRotation(_normal));
        Destroy(_hitEffect, 2f);
	}

    [Client]
    void Shoot()
    {
        if (!isLocalPlayer){
            return;
        }

        //We are shooting, call the OnShoot method on the server
        CmdOnShoot();

        RaycastHit _hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currentWeapon.range, mask))
        {
            if (_hit.collider.tag == PLAYER_TAG) {
                CmdPlayerShot(_hit.collider.name, currentWeapon.damage);
            }

            //We hit something, call the OnHit method on the server
            CmdOnHit(_hit.point, _hit.normal);
        }
    }

    [Command]
    void CmdPlayerShot (string _playerID, int _damage) {
        Debug.Log(_playerID + " has been shot");

        Player _player = GameManager.GetPlayer(_playerID);
        _player.RpcTakeDamage(_damage);
    }
}