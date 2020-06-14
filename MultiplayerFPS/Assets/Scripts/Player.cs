using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    [SerializeField]
    private int maxHelth = 100;

    [SyncVar]
    private int currentHelth;

    void Awake(){
        SetDefaults();
    }

    public void TakeDamage(int _amount){
        currentHelth -= _amount;

        Debug.Log(transform.name + " now has " + currentHelth + " health.");
    }

    public void SetDefaults(){
        currentHelth = maxHelth;
    }
}
