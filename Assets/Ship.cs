using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public SpriteRenderer sprite;
    public new Collider2D collider;
    public bool rotated;
    public int shipSize;
    public int anchorOffset;

    private bool grabbed;
    public PeriodicTable GM;
    private List<Element> elements;
    private Vector3 startPos;
    private bool startRotation;

    private int currentHits;
    private bool isMoveable = true;

    // Start is called before the first frame update
    void Awake()
    {
        startPos = transform.position;
        startRotation = rotated;
        elements = new List<Element>();
    }

    // Update is called once per frame
    void Update()
    {
        // check to see if user has released mouse to drop ship
        if (grabbed && !Input.GetMouseButton(0))
        {
            grabbed = false;
            OnShipDropped();
        }

        // while ship is grabbed, follow the mouse
        if (grabbed)
        {
            // rotate the ship when R is pressed
            if (Input.GetKeyDown(KeyCode.R))
            {
              SetShipRotation(!rotated);
            }

            // follow the mouse
            Vector3 screenPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            screenPos.x = TileGrid.ClampToTileCenter(screenPos).x;
            screenPos.y = TileGrid.ClampToTileCenter(screenPos).y;
            screenPos.z = 0f;
            transform.position = screenPos;
        }

    }

   // called when user attempts to place ship on the board
    void OnShipDropped()
    {
        bool didDrop = GM.TryDropShip(this);
        if (!didDrop)
        {
            // reset position and rotation
            transform.position = startPos;
            SetShipRotation(startRotation);
        }
    }

    // called when ship is clicked on
    void OnMouseDown()
    {
        if (!isMoveable)
        {
            return;
        }
        grabbed = true;
        GM.PickUpShip(this);

    }

    public void SetElements(List<Element> value)
    {
        elements = value;
    }

    public List<Element> GetElements()
    {
        return elements;
    }

    // assigns rotated and updates sprite
    void SetShipRotation(bool isRotated)
    {
        rotated = isRotated;
        if (rotated)
        {
            transform.eulerAngles = new Vector3(0, 0, 90);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

    // returns true if ship is on the board
    public bool IsPlaced()
    {
       return elements.Count > 0;
    }

    public void OnBomb()
    {
        currentHits++;
        if (IsDestroyed())
        {
            gameObject.SetActive(true);
        }
    }

    public bool IsDestroyed()
    {
        return currentHits >= shipSize;
    }

    public void SetMoveable(bool val)
    {
        isMoveable = val;
        collider.enabled = isMoveable;
    }

    public void SetAlpha(float alpha)
    {
        Color c = sprite.color;
        c.a = alpha;
        sprite.color = c;
    }
}
