using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance = null;
    private void Awake(){
        if (Instance){
            Destroy(this.gameObject);
        }
        else {
            Instance = this;
        }
    }
}
