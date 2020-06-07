using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


enum ElementIcon
{
    None,
    Target,
    Hit,
    Miss
}

public class Element : MonoBehaviour
{
    // Periodic table properties
    public int period;
    public int group;
    public int number;
    public int valence;
    public int energyShells;
    public string symbol;
    public float mass;
    public string elementName;

    // Texts
    public TextMeshPro numberText;
    public TextMeshPro symbolText;
    public TextMeshPro massText;
    public TextMeshPro nameText;

    // Icons
    public GameObject targetIcon;
    public GameObject hitIcon;
    public GameObject missIcon;

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
        if (mass > 0)
        {

            massText.text = mass.ToString();

        }
        nameText.text = name;
        SetIcon(ElementIcon.None);
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

    public bool Bomb()
    {
        sprite.color = Color.white;
        isBombed = true;
        if (HasShip())
        {
            SetIcon(ElementIcon.Hit);
            ship.OnBomb();
            return true;
        }
        else
        {
            SetIcon(ElementIcon.Miss);
            return false;
        }

    }

    public void SetAsTarget(bool val)
    {
        if (val)
        {
            sprite.color = Color.green;
            SetIcon(ElementIcon.Target);
        }
        else
        {
            sprite.color = Color.white;
            SetIcon(ElementIcon.None);
        }
    }

    void SetIcon(ElementIcon icon)
    {
        targetIcon.SetActive(false);
        missIcon.SetActive(false);
        hitIcon.SetActive(false);

        switch(icon)
        {
        case ElementIcon.Target:
            targetIcon.SetActive(true);
            break;
        case ElementIcon.Miss:
            missIcon.SetActive(true);
            break;
        case ElementIcon.Hit:
            hitIcon.SetActive(true);
            break;
        }
    }
}
