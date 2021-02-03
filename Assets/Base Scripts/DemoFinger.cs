using UnityEngine;

public class DemoFinger : MonoBehaviour
{

#if UNITY_EDITOR
    [SerializeField]
    Texture2D hand = null;
    [SerializeField]
    Texture2D handDown = null;
    [SerializeField]
    Texture2D handDown2 = null;
    [SerializeField] Vector2 handSize = new Vector2(128f, 128f);
    [SerializeField] Vector2 hotSpot = new Vector2(.25f, .25f);

    bool mouseDown = false;
    bool two = true;

    [ExecuteInEditMode]
    private void OnGUI() {
        Cursor.visible = false;

        if (mouseDown) {
            if (two)
            {
                GUI.DrawTexture(new
                    Rect(
                    Event.current.mousePosition.x - handSize.x / 2 + (hotSpot.x * handSize.x),
                    Event.current.mousePosition.y - handSize.y / 2 + (hotSpot.y * handSize.y),
                    handSize.x, handSize.y), handDown2);

            }
            else
                GUI.DrawTexture(new
                    Rect(
                    Event.current.mousePosition.x - handSize.x / 2 + (hotSpot.x * handSize.x),
                    Event.current.mousePosition.y - handSize.y / 2 + (hotSpot.y * handSize.y),
                    handSize.x, handSize.y), handDown);

        }
        else
        {
            GUI.DrawTexture(new
                Rect(
                Event.current.mousePosition.x - handSize.x / 2 + (hotSpot.x * handSize.x),
                Event.current.mousePosition.y - handSize.y / 2 + (hotSpot.y * handSize.y),
                handSize.x, handSize.y), hand);
        }

    }

    private void Update() {
        mouseDown = Input.GetMouseButton(0);
    }

    void OtherHand() {
        two = !two;
    }

    private void Awake() {
        InvokeRepeating("OtherHand", 0, .45f);
    }

#endif


}
