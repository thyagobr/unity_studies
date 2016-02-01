using System;
using System.Collections.Generic;

public class Path
{
    public List<Tile> path;
    public float current_distance;

    public Path(Tile tile, float current_distance = 0f)
    {
        this.path = new List<Tile>();
        this.path.Add(tile);
        this.current_distance = current_distance;
    }

    public Path(Path previous_path, Tile new_tile, float current_distance)
    {
        this.path = new List<Tile>(previous_path.path);
        this.path.Add(new_tile);
        this.current_distance = current_distance;
    }

    public Tile last_tile()
    {
        return path[path.Count - 1];
    }
}