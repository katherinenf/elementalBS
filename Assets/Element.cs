using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element : MonoBehaviour
{
    public int period;

    public int group;
    
    public Ship ship;

    private SpriteRenderer sprite;

    private bool isBombed;
    
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool HasShip()
    {
        return (ship != null);
    }

    public void SetShip(Ship value)
    {
        ship = value;
    }

    public bool IsBombed()
    {
        return isBombed;
    }
    

    public void Bomb()
    {
        if (HasShip())
        {
            sprite.color = Color.red;
            ship.OnBomb();
        }
        else
        {
            sprite.color = Color.blue;
        }

        isBombed = true;
        
    }

    public void SetAsTarget(bool val)
    {
        if (val)
        {
            sprite.color = Color.green;
        }
        else
        {
            sprite.color = Color.white;
        }
    }
}
