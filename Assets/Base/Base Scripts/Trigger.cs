using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TriggerEvent : UnityEvent { };
[RequireComponent(typeof(Collider))]
public class Trigger : MonoBehaviour
{
    

    [SerializeField]
    TriggerEvent onEnter =null;

    [SerializeField]
    TriggerEvent onExit = null;

    [SerializeField]
    private LayerMask mask = new LayerMask();

    private void OnTriggerEnter(Collider other)
    {
        if (GameUtil.IsInLayerMask(other.gameObject.layer, mask)){
            onEnter?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (GameUtil.IsInLayerMask(other.gameObject.layer, mask))
        {
            onExit?.Invoke();
        }
    }

    [ExecuteInEditMode]
    private void OnEnable(){
        GetComponent<Collider>().isTrigger = true;
    }
}
