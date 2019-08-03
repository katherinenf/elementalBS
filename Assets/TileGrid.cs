using System;
using UnityEngine;

public static class TileGrid
{
    public static float kPixelsPerUnit = 100f;
    public static float kTileSize = 78f/kPixelsPerUnit;
    public static float kHalfTileSize = kTileSize/2f;

    public static int WorldToTileIndex(float value) {
        return (int)Mathf.Floor(value/kTileSize);
    }

    public static Vector2 ClampToTileCenter(Vector2 value) {
        return new Vector2(
            Mathf.Floor(value.x/kTileSize) * kTileSize + kHalfTileSize,
            Mathf.Floor(value.y/kTileSize) * kTileSize + kHalfTileSize
        );
    }
}
