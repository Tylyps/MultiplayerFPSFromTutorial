using UnityEngine;
using Mirror;
using System.Collections;

[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour
{

    [SyncVar]
    private bool _isDead = false;
    public bool isDead {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SerializeField]
    private int maxHelth = 100;

    [SyncVar]
    private int currentHelth;

    [SerializeField]
    private Behaviour[] disableOnDeath = {};
    private bool[] wasEnabled;
    
    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath = {};

    [SerializeField]
    private GameObject deathEffect = null;
    
    [SerializeField]
    private GameObject spawnEffect = null;

    public void Setup(){
        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++){
            wasEnabled[i] = disableOnDeath[i].enabled;
        }

        SetDefaults();
    }

    void Update(){
        if(!isLocalPlayer)
            return;

        if(Input.GetKeyDown(KeyCode.K)){
            RpcTakeDamage(99999);
        }
    }

    [ClientRpc]
    public void RpcTakeDamage(int _amount){
        if(isDead)
            return;
        currentHelth -= _amount;

        Debug.Log(transform.name + " now has " + currentHelth + " health.");
        if(currentHelth <= 0){
            Die();
        }

    }

    private void Die() {
        isDead = true;


        //Disable components
        for (int i = 0; i < disableOnDeath.Length; i++){
            disableOnDeath[i].enabled = false;
        }
        //Disable GameObjects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++){
            disableGameObjectsOnDeath[i].SetActive(false);
        }

        //Disable the collider
        Collider _col = GetComponent<Collider>();
        if(_col != null){
            _col.enabled = false;
        }

        //Spawn a death effect
        GameObject _gfxIns = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(_gfxIns, 3f);

        //Switch cameras
        if(isLocalPlayer){
            GameManager.instance.SetSceneCameraActive(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }

        Debug.Log(transform.name + "is DEAD!");

        //CALL RESPAWN METHOD
        StartCoroutine(Respawn());
    }


    private IEnumerator Respawn() {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        SetDefaults();

        Debug.Log(transform.name + " respawned.");
    }

    public void SetDefaults(){
        isDead = false;

        currentHelth = maxHelth;

        //Enable the components
        for (int i = 0; i < disableOnDeath.Length; i++){
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        //Enable GameObjects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++){
            disableGameObjectsOnDeath[i].SetActive(true);
        }

        //Enable the colider
        Collider _col = GetComponent<Collider>();
        if(_col != null){
            _col.enabled = true;
        }

        //Switch cameras
        if(isLocalPlayer){
            GameManager.instance.SetSceneCameraActive(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
        }

        //Create spawn effect
        //Spawn a death effect
        GameObject _gfxIns = (GameObject)Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(_gfxIns, 3f);
    }
}
