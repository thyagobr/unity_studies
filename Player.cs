using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{

    public Vector2 player_position = Vector2.zero;
    public Vector3 next_destination;
    public Path tile_path;
    public Tile current_tile;
    Color original_color;
    public bool is_moving = false;
    public bool is_my_turn = false;
    // START SNIPPET -- synapse quest system
    [SerializeField]
    public List<Quest> questLog = new List<Quest>();
    // END SNIPPET -- synapse quest system

    // Use this for initialization
    void Awake()
    {
        tile_path = null;
        next_destination = transform.position;
        // START SNIPPET -- synapse quest system
        //questLog = new List<Quest>();
        // END SNIPPET -- synapse quest system
    }

    void Start()
    {
        original_color = GetComponent<Renderer>().material.color;
    }

    // START SNIPPET -- synapse quest system
    public void addQuest(Quest quest)
    {
        if (!questLog.Contains(quest))
        {
            print(string.Format("New quest: {0}", quest.name));
            questLog.Add(quest);
        }
        else
        {
            print(string.Format("You're already on {0}", quest.name));
        }
    }
    // END SNIPPET -- synapse quest system


    public void turn_update()
    {
        // retrieve current tile
        RaycastHit raycast_result;
        if (Physics.Raycast(transform.position, Vector3.down, out raycast_result))
        {
            current_tile = (Tile)raycast_result.collider.GetComponent<Tile>();
        }

        // move to next square
        if (is_moving)
        {
            if (Vector3.Distance(next_destination, transform.position) > 0.1f)
            {
                transform.position += (next_destination - transform.position).normalized * 5f * Time.deltaTime;
                print("moving to: " + next_destination);
            }
            else {
                if ((is_moving) && (Vector3.Distance(next_destination, transform.position) <= 0.1f))
                {
                    transform.position = next_destination;
                    if (tile_path.path.Count > 0)
                    {
                        next_destination = tile_path.path[0].transform.position + 1.2f * Vector3.up;
                        tile_path.path.RemoveAt(0);
                    }
                    else
                    {
                        is_moving = false;
                        is_my_turn = false;
                        GetComponent<Renderer>().material.color = original_color;
                        Overlord.instance.next_turn();
                    }
                }
            }
        }
    }

    public void move_on_tile_path(Path tile_path)
    {
        // turn this path[0] followed by a RemoveAt(0) into a "Tile.Pop()" or something
        this.tile_path = tile_path;
        if (tile_path == null)
        throw new System.Exception("No tile path found by pathfinding");
        this.next_destination = tile_path.path[0].transform.position + 1.2f * Vector3.up;
        tile_path.path.RemoveAt(0);
        this.is_moving = true;
        Overlord.instance.clear_screen();
    }

    void OnMouseEnter()
    {
        GetComponent<Renderer>().material.color = is_my_turn ? Color.white : Color.blue;
    }

    void OnMouseExit()
    {
        GetComponent<Renderer>().material.color = original_color;
    }

    public void start_turn()
    {
        GetComponent<Renderer>().material.color = Color.magenta;
        is_my_turn = true;
    }
}
