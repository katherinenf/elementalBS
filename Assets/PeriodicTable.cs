using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PeriodicTable : MonoBehaviour
{
    public Element[] elements;

    public Ship[] ships;

    // the done button to show when all ships are placed
    public Button uiDoneButton;
    public Button uiBombButton;
    public QuestionPicker qPicker;
    public UIQuestion uiQuestion1;
    public UIQuestion uiQuestion2;

    // when set clicking tiles bombs them
    private bool bombingEnabled;

    private Element bombTarget;
    private IQuestion q1;
    private IQuestion q2;

    // Start is called before the first frame update
    void Start()
    {
        UpdateDoneBtn();
    }

    public void OnStartTurn()
    {
        uiQuestion1.Clear();
        uiQuestion2.Clear();
        uiDoneButton.gameObject.SetActive(false);
        uiBombButton.gameObject.SetActive(true);
        uiBombButton.interactable = false;
        bombTarget = null;

        foreach (Ship ship in ships)
        {
            if (ship.IsDestroyed())
            {
                ship.gameObject.SetActive(true);
            }
            else
            {
                ship.gameObject.SetActive(false);
            }
            ship.SetMoveable(false);
        }
    }

    public void OnDoneClick()
    {
        GameplayManager.Instance.ShipPlacementComplete();
    }

    public void OnFireClick()
    {
        if (bombingEnabled && bombTarget != null)
        {
            // evaluate questions
            if(!q1.Evaluate(bombTarget, 1) || !q2.Evaluate(bombTarget, 1))
            {
                bombTarget.SetAsTarget(false);
                bombTarget = ChooseRandomElement();
            }
            //if either question is wrong assign bomb target to random
            StartCoroutine("PlayBombSequence", bombTarget);

        }
    }

    Element ChooseRandomElement()
    {
        Element[] valid = Array.FindAll(elements, e => e != null);
        int index = UnityEngine.Random.Range(0, valid.Length);
        return valid[index];
    }

    IEnumerator PlayBombSequence(Element target)
    {
        bombingEnabled = false;
        target.Bomb();
        yield return new WaitForSeconds(.5f);
        if (AllShipsDestroyed())
        {
            GameplayManager.Instance.Victory();
        }
        else
        {
            GameplayManager.Instance.EndTurn();
        }
    }

    public void SetBombingEnabled(bool val)
    {
        bombingEnabled = val;
    }

    Element WorldToElement(Vector2 worldPos)
    {
        Vector2 localPos = worldPos - new Vector2(transform.position.x, transform.position.y);
        int x = TileGrid.WorldToTileIndex(localPos.x);
        int y = TileGrid.WorldToTileIndex(-localPos.y);
        Element element = GetElement(x, y);
        return element;
    }

    Element GetElement(int x, int y)
    {
        if (x < 0 || x >= 18 || y < 0 || y >= 7)
        {
            return null;
        }
        int idx = 18 * y + x;
        if (idx < 0 || idx < elements.Length)
        {
            return elements[idx];
        }

        return null;
    }

    public bool TryDropShip(Ship ship)
    {
        // find overlapping tiles
        List<Element> tiles = GetOverlappingElements(ship);

        // check ship is fully on board
        if (tiles.Count != ship.shipSize)
        {

            return false;
        }

        // check that no tiles already contain ships
        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].HasShip())
            {

                return false;
            }
        }

        // fill tiles
        for (int i = 0; i < tiles.Count; i++)
        {
            Element element = tiles[i];
            element.SetShip(ship);
        }

        // associate tiles with ship
        ship.SetElements(tiles);

        // update UI
        UpdateDoneBtn();

        return true;
    }

    public List<Element> GetOverlappingElements(Ship ship)
    {
        List<Element> eles = new List<Element>();
        for (int i = 0; i < ship.shipSize; i++)
        {
            Vector3 offset;
            if (ship.rotated)
            {
                offset = new Vector3((-i + ship.anchorOffset) * TileGrid.kTileSize, 0, 0);
            }
            else
            {
                offset = new Vector3(0, (i - ship.anchorOffset) * TileGrid.kTileSize, 0);
            }

            Element element = WorldToElement(ship.transform.position + offset);
            if (element != null)
            {
              eles.Add(element);
            }

        }

        return eles;
    }

    public void PickUpShip(Ship ship)
    {
        // disassociate ship from elements
        List < Element > elements = ship.GetElements();
        foreach (Element element in elements)
        {
            element.SetShip(null);
            element.GetComponent<SpriteRenderer>().color = Color.white;

        }

        // disassociate elements from ship
        ship.SetElements(new List<Element>());

        // update UI
        UpdateDoneBtn();
    }

    // update the visibility of the done button based on if all ships placed
    void UpdateDoneBtn()
    {
        // determine if all ships placed
        bool allPlaced = true;
        foreach (Ship ship in ships)
        {
            allPlaced &= ship.IsPlaced();
        }

        // update button visibility
        uiDoneButton.interactable = allPlaced;
    }

    void OnMouseDown()
    {
        if (bombingEnabled)
        {
            // find possible clicked tile
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Element element = WorldToElement(worldPos);

            // if a tile was in fact clicked, set bomb target
            if (element != null && !element.IsBombed() && element != bombTarget)
            {
                if (bombTarget != null)
                {
                    bombTarget.SetAsTarget(false);
                }
                bombTarget = element;
                bombTarget.SetAsTarget(true);

                // select questions
                q1 = qPicker.Choose(bombTarget);
                q2 = qPicker.Choose(bombTarget);

                // update question UIs
                uiQuestion1.Load(bombTarget, q1);
                uiQuestion2.Load(bombTarget, q2);
            }

            // update done button state
            uiBombButton.interactable = (bombTarget != null);
        }
    }

    bool AllShipsDestroyed()
    {
        foreach (Ship ship in ships)
        {
            if (!ship.IsDestroyed())
            {
                return false;
            }
        }

        return true;

    }

}
