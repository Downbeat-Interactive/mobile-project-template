using UnityEngine;
using DG.Tweening;

public class KeepRotation : MonoBehaviour
{
    Quaternion rotation = Quaternion.identity;
    [SerializeField]
    UpdateType update;


    void Start(){
        rotation = transform.rotation;
    }

    void FixedUpdate(){
        if (update.Equals(UpdateType.Fixed))
            KeepRot();
    }
    void LateUpdate(){
        if (update.Equals(UpdateType.Late))
            KeepRot();

    }
    void Update(){
        if (update.Equals(UpdateType.Normal))
            KeepRot();
    }

    void KeepRot() {
        transform.rotation = rotation;
    }
}
