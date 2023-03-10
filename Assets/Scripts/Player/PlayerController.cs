using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _speed = 0.2f;
    private Vector2 _dest = Vector2.zero;
    private Vector2 _dir = Vector2.zero;
    private Vector2 _nextDir = Vector2.zero;

    private int _score = 0;

    public Vector2 Direction
    {
        get { return _dir; }
    }

    private void Start()
    {
        _dest = transform.position;
    }

    private void FixedUpdate()
    {
        ReadInputAndMove();
    }

    /// <summary>
    /// Checks for walls in specific direction
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    bool Valid(Vector2 direction)
    {
        Vector2 pos = transform.position;
        direction += new Vector2(direction.x * 0.45f, direction.y * 0.45f);
        RaycastHit2D hit = Physics2D.Linecast(pos + direction, pos);
        return hit.collider.tag == "Food" || hit.collider.tag == "Powerup" || (hit.collider == GetComponent<Collider2D>());
    }
    public void ResetDestination()
    {
        _dest = new Vector2(15f, 11f);
    }
    void ReadInputAndMove()
    {
        //Move to destination
        Vector2 p = Vector2.MoveTowards(transform.position, _dest, _speed);
        GetComponent<Rigidbody2D>().MovePosition(p);

        //Get inputdirection from keyboard
        if (Input.GetAxis("Horizontal") > 0) _nextDir = Vector2.right;
        if (Input.GetAxis("Horizontal") < 0) _nextDir = -Vector2.right;
        if (Input.GetAxis("Vertical") > 0) _nextDir = Vector2.up;
        if (Input.GetAxis("Vertical") < 0) _nextDir = -Vector2.up;

        //If pacman is in the middle of a tile
        if (Vector2.Distance(_dest, transform.position) < 0.00001f)
        {
            //Check if next direction has been reached
            if (Valid(_nextDir))
            {
                //Update destination and direction
                _dest = (Vector2)transform.position + _nextDir;
                _dir = _nextDir;
            }
            //If next direction is invalid check old direction
            else
            {
                //Old direction valid
                if (Valid(_dir))
                {
                    //Update destination
                    _dest = (Vector2)transform.position + _dir;
                }
                //Otherwise don't update destination (keeps player at position)
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Food")
        {
            Destroy(collision.gameObject);
            Gamemanager.instance.CollectFood();
        }
        else if (collision.tag == "Powerup")
        {
            Destroy(collision.gameObject);
            Gamemanager.instance.CollectPowerup();
        }
        else if (collision.tag == "Enemy")
        {
            if (Gamemanager.Powered)
            {
                collision.gameObject.SetActive(false);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
