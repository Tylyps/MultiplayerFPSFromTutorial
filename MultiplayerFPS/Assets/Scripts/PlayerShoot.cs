using UnityEngine;
using Mirror;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{
    private const string PLAYER_TAG = "Player";

    private PlayerWeapon currentWeapon = null;
    [SerializeField]
    private Camera cam = null;

    [SerializeField]
    private LayerMask mask = 0;
    
    private WeaponManager weaponManager;

    void Start()
    {
        if(cam == null){
            Debug.LogError("PlaterShoot: No camera referenced!");
            this.enabled = false;
        }

        weaponManager = GetComponent<WeaponManager>();

    }

    // Update is called once per frame
    void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();

        if(currentWeapon.fireRate <= 0){
            if(Input.GetButtonDown("Fire1")) {
                Shoot(); 
            }
        } else {
            if(Input.GetButtonDown("Fire1")) {
                InvokeRepeating("Shoot", 0f, 1f/currentWeapon.fireRate);
            } else if(Input.GetButtonUp("Fire1")){
                CancelInvoke("Shoot");
            }
        }
    }

    [Client]
    private void Shoot() {
        RaycastHit _hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currentWeapon.range, mask)){
            if(_hit.collider.tag == PLAYER_TAG){
                CmdPlayerShot(_hit.collider.name, currentWeapon.damage);
            }
        }
    }

    [Command]
    void CmdPlayerShot(string _playerID, int _damage) {
        Debug.Log(_playerID + "has been shot.");
        Player _player = GameManager.GetPlayer(_playerID);
        _player.RpcTakeDamage(_damage);
    }
}
