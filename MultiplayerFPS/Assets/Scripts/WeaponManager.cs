using UnityEngine;
using Mirror;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField]
    private PlayerWeapon primaryWeapon = null;

    private PlayerWeapon currentWeapon = null;
    private WeaponGraphics currentGraphics;

    [SerializeField]
    private string weaponLayerName = "Weapon";

    [SerializeField]
    private Transform weaponHolder = null;

    void Start() {
        EquipWeapon(primaryWeapon);
    }

    void EquipWeapon(PlayerWeapon _weapon){
        currentWeapon = _weapon;

        GameObject _weaponIns = (GameObject)Instantiate(_weapon.graphics, weaponHolder.position, weaponHolder.rotation);
        _weaponIns.transform.SetParent(weaponHolder);

        currentGraphics = _weaponIns.GetComponent<WeaponGraphics>();
        if(currentGraphics == null) {
            Debug.LogError("No WeaponGraphics component on the weapon boject: " + _weaponIns.name);
        }

        if(isLocalPlayer) {
            Util.SetLayerRecursively(_weaponIns, LayerMask.NameToLayer(weaponLayerName));
        }
    }

    public PlayerWeapon GetCurrentWeapon() {
        return currentWeapon;
    }

    public WeaponGraphics GetCurrentGraphics(){
        return currentGraphics;
    }
}
