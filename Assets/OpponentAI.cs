using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OpponentAI
{
    // The algorithm here is very naive- but it seems to work!. It just keeps trying to place the ships randomly and hopes for the best.
    // I have not observed > 4 retries in practice
    public void PlaceShips(PeriodicTable table)
    {
        // Get all non-null tiles to select from
        List<Element> eles = table.elements.Where(e => e != null).ToList();

        // For each ship
        foreach (Ship s in table.ships)
        {
            // Select a random rotation
            s.SetShipRotation(Random.value > 0.5f);

            // Keep retrying placement until we succeed.
            int tries = 0;
            bool success = false;
            do
            {

                // Select a random tile
                Element tile = eles[Random.Range(0, eles.Count)];

                // Try to place the ship there
                s.transform.position = tile.transform.position;
                success = table.TryDropShip(s);
                tries++;
            } while (!success);

            Debug.Log("Dropping ship " + s.name + " took " + tries + ((tries != 1) ? " tries" : " try"));
        }
    }

    // Run the AI turns bombing sequence
    public IEnumerator BombTile(PeriodicTable table)
    {
        Element target = SelectTargetTile(table);
        yield return new WaitForSeconds(2f);
        target.SetAsTarget(true);
        yield return new WaitForSeconds(3f);
        target.Bomb();
        yield return new WaitForSeconds(3f);
    }
    
    Element SelectTargetTile(PeriodicTable table)
    {
        // If 'C' is held, always hit a ship. Useful for debugging.
        if (Input.GetKey(KeyCode.C))
        {
            Debug.Log("Selecting a cheat tile");
            return GetCheatTile(table);
        }

        // If we have an suspected tiles, randomly select one of those
        List<Element> suspectedTiles = GetSuspectedTiles(table);
        Debug.Log("Suspected " + suspectedTiles.Count + " tiles");
        if (suspectedTiles.Count > 0)
        {
            return suspectedTiles[Random.Range(0, suspectedTiles.Count)];
        }

        // Otherwise pick a totally random tile
        return PickRandomTile(table);
    }

    // Returns a random unbombed tile that contains a ship
    Element GetCheatTile(PeriodicTable table)
    {
        Ship[] randomShips = table.ships.OrderBy(x => Random.value).ToArray();
        foreach (Ship s in randomShips)
        {
            // Skip already destroyed ships
            if (s.IsDestroyed())
            {
                continue;
            }

            Element[] randomEles = s.GetElements().OrderBy(x => Random.value).ToArray();
            foreach (Element e in randomEles)
            {
                if (!e.IsBombed())
                {
                    return e;
                }
            }
        }
        return PickRandomTile(table);
    }

    // Returns tiles that the AI suspects contains a ship based on the current bombed tiles
    List<Element> GetSuspectedTiles(PeriodicTable table)
    {
        HashSet<Element> suspectedTiles = new HashSet<Element>();

        // Loop through each ship
        foreach (Ship s in table.ships)
        {
            // Skip already destroyed ships
            if (s.IsDestroyed())
            {
                continue;
            }

            // For each element in the ship
            List<Element> eles = s.GetElements();
            for (int i = 0; i < eles.Count; i++)
            {
                // Look for "runs" of bombed segments
                List<Element> run = new List<Element>();
                for (int j = i; j < eles.Count; j++)
                {
                    if (eles[j].IsBombed())
                    {
                        run.Add(eles[j]);
                    } else
                    {
                        break;
                    }
                }
                
                // If it's a run of 1, consider all 4 adjacent tiles
                if (run.Count == 1)
                {
                    List<Element> adjacent = table.GetAdjacentElements(run.First()).Where(e => !e.IsBombed()).ToList();
                    suspectedTiles.UnionWith(adjacent);
                }
                // If it's a run of > 1, consider only the two possible endcap tiles 
                else if (run.Count > 1)
                {
                    // Compute the direction of the 'run'
                    Vector3 dir = (run.First().transform.position - run.Last().transform.position).normalized;

                    // Add the first endcap
                    Element endcap1 = table.WorldToElement(run.First().transform.position + dir * TileGrid.kTileSize);
                    if (endcap1)
                    {
                        suspectedTiles.Add(endcap1);
                    }

                    // Add the second endcap
                    Element endcap2 = table.WorldToElement(run.Last().transform.position + -dir * TileGrid.kTileSize);
                    if (endcap2)
                    {
                        suspectedTiles.Add(endcap2);
                    }

                    // Skip outer loop to the end of the segment
                    i += run.Count - 1;
                }
            }
        }
        return suspectedTiles.ToList();
    }

    // Returns a totally random unbombed tile
    Element PickRandomTile(PeriodicTable table)
    {
        List<Element> eles = table.elements.Where(e => e != null && !e.IsBombed()).ToList();
        return eles[Random.Range(0, eles.Count)];
    }
}
