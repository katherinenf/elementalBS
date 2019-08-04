using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Element : MonoBehaviour
{
    public int period;

    public int group;

    public int number;

    public string symbol;

    public float mass;

    public string name;

    public TextMeshPro numberText;
    public TextMeshPro symbolText;
    public TextMeshPro massText;
    public TextMeshPro nameText;

    public GameObject target;

    [HideInInspector]
    public Ship ship;

    private SpriteRenderer sprite;

    private bool isBombed;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        numberText.text = number.ToString();
        symbolText.text = symbol;
        massText.text = mass.ToString();
        nameText.text = name;
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
        SetAsTarget(false);
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
        target.SetActive(val);
    }
}
