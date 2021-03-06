﻿using UnityEngine;
using Mirror;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentsToDisable = {};

    [SerializeField]
    string remoteLayerName = "RemotePlayer";

    [SerializeField]
    string dontDrawLayerName = "DontDraw";

    [SerializeField]
    GameObject playerGraphics = null;

    // Camera sceneCamera;

    [SerializeField]
    GameObject playerUIPrefab = null;
    [HideInInspector]
    public GameObject playerUIInstance;

    void Start() {
        if(!isLocalPlayer){
            DisableComponents();
            AssignRemoteLayer();
        } else {
            // sceneCamera = Camera.main;
            // if(sceneCamera != null){
            //     sceneCamera.gameObject.SetActive(false);
            // }

            SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));

            //Crate PlayerUI
            playerUIInstance = Instantiate(playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;
            //Configure PlayerUI
            PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
            if(ui == null){
                Debug.LogError("No playerUI component on PlayerUI prefab");
            }
            ui.SetContriller(GetComponent<PlayerController>());
        }

        GetComponent<Player>().Setup();
    }

    void SetLayerRecursively(GameObject obj, int newLayer){
        obj.layer = newLayer;
        foreach(Transform child in obj.transform){
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    public override void OnStartClient() {
        base.OnStartClient();
        string _netID = GetComponent<NetworkIdentity>().netId.ToString();
        Player _player = GetComponent<Player>();
        GameManager.RegisterPlayer(_netID, _player);
    }
    
    void AssignRemoteLayer() {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    void DisableComponents() {
        for(int i = 0; i < componentsToDisable.Length; i++){
            componentsToDisable[i].enabled = false;
        }
    }

    void OnDisable() {
        Destroy(playerUIInstance);

        // if(sceneCamera != null) {
        //     sceneCamera.gameObject.SetActive(true);
        // }

        GameManager.instance.SetSceneCameraActive(true);

        GameManager.UnRegisterPlayer(transform.name);
    }


}
