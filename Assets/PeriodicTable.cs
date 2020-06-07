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

    public void OnStartTurn(bool showShipGhosts)
    {
        // Disable bombing until the black screen (turn hider) is gone
        bombingEnabled = false;

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
                ship.SetAlpha(1.0f);
            }
            else
            {
                if (showShipGhosts)
                {
                    ship.gameObject.SetActive(true);
                    ship.SetAlpha(.4f);
                }
                else
                {
                    ship.gameObject.SetActive(false);
                }
            }
            ship.SetMoveable(false);
        }
    }

    public void OnDoneClick()
    {
        // Notify game manager
        GameplayManager.Instance.EndPhase();
    }

    public void OnFireClick()
    {
        if (bombingEnabled && bombTarget != null)
        {
            // Evaluate target
            bool q1Correct = q1.Evaluate(bombTarget, uiQuestion1.value);
            bool q2Correct = q2.Evaluate(bombTarget, uiQuestion2.value);

            if(!q1Correct || !q2Correct)
            {
                //if either question is wrong assign bomb target to random
                bombTarget.SetAsTarget(false);
                bombTarget = ChooseRandomElement();
                bombTarget.SetAsTarget(true);
            }

            // Run the animation
            StartCoroutine(PlayEndOfTurnSequence(q1Correct, q2Correct, bombTarget));
        }
    }

    // Returns a random element that has not already been bombed
    Element ChooseRandomElement()
    {
        // Choose only from the non-null elements that have not already been bombed
        Element[] valid = Array.FindAll(elements, e => e != null && !e.IsBombed());

        // Select a random element
        int index = UnityEngine.Random.Range(0, valid.Length);

        return valid[index];
    }

    // Coroutine that animates the happity-haps at the end of the turn
    IEnumerator PlayEndOfTurnSequence(bool q1Correct, bool q2Correct, Element target)
    {
        // Prevent further input
        bombingEnabled = false;

        // Q1 animation
        uiQuestion1.ShowAnswer(q1Correct);
        yield return new WaitForSeconds(.25f);

        // Q2 animation
        uiQuestion2.ShowAnswer(q2Correct);
        yield return new WaitForSeconds(1f);

        // Play the bombing
        target.Bomb();
        yield return new WaitForSeconds(1f);

        // Notify game manager
        GameplayManager.Instance.EndPhase();
    }

    public void SetBombingEnabled(bool val)
    {
        bombingEnabled = val;
    }

    public Element WorldToElement(Vector2 worldPos)
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

    public List<Element> GetAdjacentElements(Element e)
    {
        Vector3[] offsets = new Vector3[] {
            new Vector3(TileGrid.kTileSize, 0),
            new Vector3(-TileGrid.kTileSize, 0),
            new Vector3(0, TileGrid.kTileSize),
            new Vector3(0, -TileGrid.kTileSize)
        };
        List<Element> ret = new List<Element>();
        for (int i = 0; i < 4; i++)
        {
            Element adj = WorldToElement(e.transform.position + offsets[i]);
            if (adj)
            {
                ret.Add(adj);
            }
        }
        return ret;
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
                q2 = qPicker.Choose(bombTarget, new List<IQuestion>() { q1 });

                // update question UIs
                uiQuestion1.Load(bombTarget, q1);
                uiQuestion2.Load(bombTarget, q2);

                // focus the first questions input field
                uiQuestion1.Select();
            }

            // update done button state
            uiBombButton.interactable = (bombTarget != null);
        }
    }

    public bool AllShipsDestroyed()
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
