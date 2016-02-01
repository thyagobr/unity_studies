using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Overlord : MonoBehaviour
{
    public static Overlord instance;
    public float player_max_movement = 5f;
    public GameObject tile_prefab;
    public GameObject player_prefab;
    public Vector3 destination;
    public int map_size = 15;

    public Quest shoot_first;

    public GameObject highlight_prefab;
    public GameObject highlight_box;
    // test code
    public QuestManager questManager;

    LineRenderer movement_line;
    
    Stack<Tile> current_movement_path;

    public List<List<Tile>> map = new List<List<Tile>>();
    Player player;
    Player other_player;
    List<Player> players;
    public Player current_player;
    int current_player_index;

    // states
    public bool isHighlightingMovement = false;

    void Awake()
    {
        players = new List<Player>();
        current_player_index = 0;
        current_movement_path = new Stack<Tile>();
        instance = this;
    }

    void Start()
    {
        highlight_box = (GameObject) Instantiate(highlight_prefab, new Vector3(1000f, 1000f, 1000f), Quaternion.identity);
        movement_line = (LineRenderer) ((GameObject) Instantiate(new GameObject("movement_line"), new Vector3(0f, 0f, 0f), Quaternion.identity)).AddComponent<LineRenderer>();
        movement_line.SetColors(Color.cyan, Color.cyan);
        movement_line.SetWidth(.2f, .2f);
        movement_line.SetVertexCount(3);
        highlight_box.GetComponent<Renderer>().enabled = false;
        movement_line.enabled = false;

        for (int y = 0; y < map_size; y++)
        {
            List<Tile> row = new List<Tile>();
            for (int x = 0; x < map_size; x++)
            {
                Tile tile = ((GameObject)Instantiate(tile_prefab, new Vector3(x - Mathf.Floor(map_size / 2), 0f, y - Mathf.Floor(map_size / 2)), Quaternion.identity)).GetComponent<Tile>();
                tile.grid_position = new Vector2(x, y);
                row.Add(tile);
            }
            map.Add(row);
        }

        player = ((GameObject)Instantiate(player_prefab, new Vector3(7f, 1.2f, 7f), Quaternion.identity)).GetComponent<Player>();
        player.gameObject.tag = "Player";
        other_player = ((GameObject)Instantiate(player_prefab, new Vector3(7f, 1.2f, -7f), Quaternion.identity)).GetComponent<Player>();
        other_player.gameObject.AddComponent<QuestGiver>();
        other_player.gameObject.tag = "QuestGiver";
        players.Add(player);
        players.Add(other_player);
        current_player = players[current_player_index];
        destination = player.transform.position;
        current_player.start_turn();
    }

    void Update()
    {
        current_player.turn_update();

        if (Input.GetKeyDown(KeyCode.M))
        {
            isHighlightingMovement = true;
            highlight_movement();
        }

        // ranged attack
        if (Input.GetKeyDown(KeyCode.A))
        {
            // anything above 8f is really risking halting Unity
            // highlight_attack(7f);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isHighlightingMovement = false;
            highlight_box.GetComponent<Renderer>().enabled = false;
            movement_line.enabled = false;
            clear_screen();
        }
    }

    public void move_player(Vector3 next_destination)
    {
        // reset global states
        isHighlightingMovement = false;
        highlight_box.GetComponent<Renderer>().enabled = false;
        movement_line.enabled = false;
        destination = next_destination;
        current_player.move_on_tile_path(find_path());
    }

    public void clear_screen()
    {
        foreach (List<Tile> row in map)
            foreach (Tile t in row)
                t.GetComponent<Renderer>().material.color = t.default_material_color;
    }

    public void next_turn()
    {
        current_player_index = (current_player_index + 1) % players.Count;
        current_player = players[current_player_index];
    }

    Path find_path()
    {
        Path tile_path = null;
        Stack<Path> pathes = new Stack<Path>();

        // change this, so that we can get a path from anywhere to anywhere
        Tile starting_tile = current_player.current_tile;

        Path starting_path = new Path(starting_tile);
        pathes.Push(starting_path);

        Tile closest_tile_to_destination = starting_tile;

        while (pathes.Count > 0)
        {
            bool is_diagonal = false;
            Path current_path = pathes.Pop();
            foreach (Tile tile in current_path.last_tile().simple_neighbours)
                closest_tile_to_destination = check_tiles_smaller_distance_to_destination(closest_tile_to_destination, tile);
            foreach (Tile tile in current_path.last_tile().diagonal_neighbours)
                closest_tile_to_destination = check_tiles_smaller_distance_to_destination(closest_tile_to_destination, tile);

            if (current_path.last_tile().diagonal_neighbours.Contains(closest_tile_to_destination))
                is_diagonal = true;

            float next_movement_cost = current_path.current_distance + (is_diagonal ? Tile.DIAGONAL_MOVEMENT_COST : Tile.SIMPLE_MOVEMENT_COST);

            if ((closest_tile_to_destination.transform.position + 1.2f * Vector3.up) == destination)
            {
                if (next_movement_cost <= player_max_movement)
                {
                    current_path.path.Add(closest_tile_to_destination);
                    tile_path = current_path;
                }

            }
            else
            {
                if (next_movement_cost <= player_max_movement)
                {
                    pathes.Push(new Path(current_path, closest_tile_to_destination, next_movement_cost));
                }
            }
        }
        return tile_path;
    }

    private Tile check_tiles_smaller_distance_to_destination(Tile tile_a, Tile tile_b)
    {
        Tile result = tile_a;
        if (Vector3.Distance(tile_a.transform.position, destination) > Vector3.Distance(tile_b.transform.position, destination))
        {
            result = tile_b;
        }
        return result;
    }

    void highlight_movement(Tile current_tile = null, float movement_cost = 0f)
    {
        if (current_tile == null)
            current_tile = current_player.current_tile;
        List<Tile> highlight = new List<Tile>();
        Stack<Path> pathes = new Stack<Path>();

        Path starting_path = new Path(current_tile);
        pathes.Push(starting_path);

        while (pathes.Count > 0)
        {
            Path current_path = pathes.Pop();

            // think of a nice way to refactor this, okay? =]
            // the best idea so far is to set a Tile's neighbours to be a
            // single Dictionary<Tile, float>, with the float being the
            // appropriate Tile.XXXXX_MOVEMENT_COST.
            foreach (Tile tile in current_path.last_tile().simple_neighbours)
            {
                float next_movement_cost = current_path.current_distance + Tile.SIMPLE_MOVEMENT_COST;
                if (next_movement_cost <= player_max_movement)
                {
                    pathes.Push(new Path(current_path, tile, next_movement_cost));
                }
                else
                {
                    foreach (Tile highlight_tile in current_path.path)
                    {
                        if (!highlight.Contains(highlight_tile))
                            highlight.Add(highlight_tile);
                    }
                }
            }

            foreach (Tile tile in current_path.last_tile().diagonal_neighbours)
            {
                float next_movement_cost = current_path.current_distance + Tile.DIAGONAL_MOVEMENT_COST;
                if (next_movement_cost <= player_max_movement)
                {
                    pathes.Push(new Path(current_path, tile, next_movement_cost));
                }
                else
                {
                    foreach (Tile highlight_tile in current_path.path)
                    {
                        if (!highlight.Contains(highlight_tile))
                            highlight.Add(highlight_tile);
                    }
                }
            }

        }

        foreach (Tile tile in highlight)
        {
            tile.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    // attack

    void highlight_attack(float range)
    {
        Profiler.BeginSample("highlight area");
        Tile current_tile = current_player.current_tile;
        List<Tile> highlight = new List<Tile>();
        Stack<Path> pathes = new Stack<Path>();

        Path starting_path = new Path(current_tile);
        pathes.Push(starting_path);

        while (pathes.Count > 0)
        {
            Path current_path = pathes.Pop();

            // think of a nice way to refactor this, okay? =]
            // the best idea so far is to set a Tile's neighbours to be a
            // single Dictionary<Tile, float>, with the float being the
            // appropriate Tile.XXXXX_MOVEMENT_COST.
            foreach (Tile tile in current_path.last_tile().simple_neighbours)
            {
                float next_movement_cost = current_path.current_distance + Tile.SIMPLE_MOVEMENT_COST;
                if (next_movement_cost <= range)
                {
                    pathes.Push(new Path(current_path, tile, next_movement_cost));
                }
                else
                {
                    foreach (Tile highlight_tile in current_path.path)
                    {
                        if (!highlight.Contains(highlight_tile))
                            highlight.Add(highlight_tile);
                    }
                }
            }

            foreach (Tile tile in current_path.last_tile().diagonal_neighbours)
            {
                float next_movement_cost = current_path.current_distance + Tile.DIAGONAL_MOVEMENT_COST;
                if (next_movement_cost <= range)
                {
                    pathes.Push(new Path(current_path, tile, next_movement_cost));
                }
                else
                {
                    foreach (Tile highlight_tile in current_path.path)
                    {
                        if (!highlight.Contains(highlight_tile))
                            highlight.Add(highlight_tile);
                    }
                }
            }

        }

        foreach (Tile tile in highlight)
        {
            tile.GetComponent<Renderer>().material.color = Color.blue;
        }
        Profiler.EndSample();
    }

    internal void draw_movement_line_from(Vector3 destination_position)
    {
        Vector3[] positions = new Vector3[3];
        positions[0] = current_player.transform.position;
        positions[1] = Vector3.zero;
        positions[2] = destination_position + 1.2f * Vector3.up;
        positions[1] = Vector3.Slerp(positions[0], positions[2], .5f);
        //movement_line.SetPositions(positions);
        int i = 0;
        foreach (Vector3 point in positions)
        {
          movement_line.SetPosition(i++, point);
        }
        movement_line.enabled = true;
        highlight_box.GetComponent<Renderer>().enabled = true;
    }

}
