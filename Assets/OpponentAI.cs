using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OpponentAI
{
    public void PlaceShips(PeriodicTable table)
    {
        table.elements.Where(e => e != null);
    }

    public IEnumerator BombTile(PeriodicTable table)
    {
        List<Element> eles = table.elements.Where(e => e != null && !e.IsBombed()).ToList();
        Element tile = eles[Random.Range(0, eles.Count)];
        yield return new WaitForSeconds(2f);
        tile.SetAsTarget(true);
        yield return new WaitForSeconds(3f);
        tile.Bomb();
        yield return new WaitForSeconds(3f);
    }
}
