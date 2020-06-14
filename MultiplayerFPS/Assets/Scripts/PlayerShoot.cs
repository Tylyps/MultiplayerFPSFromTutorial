using UnityEngine;
using Mirror;

public class PlayerShoot : NetworkBehaviour
{
    private const string PLAYER_TAG = "Player";
    public PlayerWeapon weapon;
    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;
    // Start is called before the first frame update
    void Start()
    {
        if(cam == null){
            Debug.LogError("PlaterShoot: No camera referenced!");
            this.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1")) {
           Shoot(); 
        }
    }

    [Client]
    private void Shoot() {
        RaycastHit _hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, weapon.range, mask)){
            if(_hit.collider.tag == PLAYER_TAG){
                CmdPlayerShot(_hit.collider.name, weapon.damage);
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
