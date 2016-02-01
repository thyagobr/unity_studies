using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{

    public Vector2 grid_position = Vector2.zero;
    Renderer _renderer;
    public Color default_material_color;
    Color pre_hover_color;
    public bool is_highlighted;
    public List<Tile> simple_neighbours;
    public List<Tile> diagonal_neighbours;
    internal const float DIAGONAL_MOVEMENT_COST = 1.5f;
    internal const float SIMPLE_MOVEMENT_COST = 1F;

    void Start()
    {
        simple_neighbours = new List<Tile>();
        diagonal_neighbours = new List<Tile>();
        generate_neighbours();
        _renderer = GetComponent<Renderer>();
        default_material_color = _renderer.material.color;
        is_highlighted = false;
    }

    void OnMouseEnter()
    {

        //print("hovered is: " + transform.position + " => " + grid_position + "[" + world_to_grid(transform.position) + "]");
        Overlord.instance.highlight_box.transform.position = transform.position + 1.2f * Vector3.up;
        if (Overlord.instance.isHighlightingMovement)
        {
          Overlord.instance.draw_movement_line_from(transform.position);
        }
    }

    void OnMouseExit()
    {

    }

    void OnMouseUp()
    {
        //Overlord.instance.destination = transform.position + 1.2f * Vector3.up;
        Overlord.instance.move_player(transform.position + 1.2f * Vector3.up);
        //Overlord.instance.pathfinding();
    }

    void generate_neighbours()
    {
        int x = (int)grid_position.x;
        int y = (int)grid_position.y;

        // right
        if (x + 1 < Overlord.instance.map_size)
            simple_neighbours.Add(Overlord.instance.map[(int)(y)][(int)(x + 1)]);
        // down
        if (y + 1 < Overlord.instance.map_size)
            simple_neighbours.Add(Overlord.instance.map[(int)(y + 1)][(int)(x)]);
        // bottom-right
        if ((y + 1 < Overlord.instance.map_size) && (x + 1 < Overlord.instance.map_size))
            diagonal_neighbours.Add(Overlord.instance.map[(int)(y + 1)][(int)(x + 1)]);
        // bottom-left
        if ((y + 1 < Overlord.instance.map_size) && (x - 1 >= 0))
            diagonal_neighbours.Add(Overlord.instance.map[(int)(y + 1)][(int)(x - 1)]);
        // up
        if (y - 1 >= 0)
            simple_neighbours.Add(Overlord.instance.map[(int)(y - 1)][(int)(x)]);
        // top-right
        if ((y - 1 >= 0) && (x + 1 < Overlord.instance.map_size))
            diagonal_neighbours.Add(Overlord.instance.map[(int)(y - 1)][(int)(x + 1)]);
        // top-left
        if ((y - 1 >= 0) && (x - 1 >= 0))
            diagonal_neighbours.Add(Overlord.instance.map[(int)(y - 1)][(int)(x - 1)]);
        // left
        if (x - 1 >= 0)
            simple_neighbours.Add(Overlord.instance.map[(int)(y)][(int)(x - 1)]);
    }

    Vector2 world_to_grid(Vector3 position)
    {
        return new Vector2((position.x + Overlord.instance.map_size / 2), (position.z + Overlord.instance.map_size / 2));
    }

}
