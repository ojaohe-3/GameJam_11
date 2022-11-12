using System;
using System.Collections.Generic;
using Models;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace Objects
{
    public class Player : MonoBehaviour
    {

        private Rigidbody2D _body;

        [SerializeField] private float _interactionRadius = 1.0f;

        [SerializeField] private float _speed = 10.0f;
        [SerializeField] private float collisionOffset = 0.05f;
        [SerializeField] private ContactFilter2D movementFilter;
        private bool _interacted = false;
        private Vector2 _moveInput;
        private List<RaycastHit2D> _castCollisions = new List<RaycastHit2D>();

        void Start()
        {
            _body = GetComponent<Rigidbody2D>();
            Assert.IsNotNull(_body);
            NetworkManager.Start(null);
        }

        void OnInteract()
        {
            Debug.Log("interacted !");
        }


        void FixedUpdate()
        {
            while (NetworkManager.Queue.Count > 0)
            {
                Vector2 move = parseMovement(NetworkManager.Queue.Dequeue());
                Debug.Log("parsed movement " + move);
                var success = MovePlayer(move);
                if(!success)
                {
                    // Try Left / Right
                    success = MovePlayer(new Vector2(move.x, 0));

                    if(!success)
                    {
                        success = MovePlayer(new Vector2(0, move.y));
                    }
                }
            }
            NetworkManager.SendMove(_moveInput);
        }

        private static Vector2 parseMovement(string s)
        {
            s = s.Replace(" ", "").Replace("(", "").Replace(")", "");
            var split = s.Trim().Split(",");
            if (split.Length < 2)
                throw new Exception("couldn't parse movement in " + s);
            var x = float.Parse(split[0]);
            var y = float.Parse(split[1]);
            return new Vector2(x, y);
        }

        // Tries to move the player in a direction by casting in that direction by the amount
        // moved plus an offset. If no collisions are found, it moves the players
        // Returns true or false depending on if a move was executed
        public bool MovePlayer(Vector2 direction)
        {
            // Check for potential collisions
            int count = _body.Cast(
                direction, // X and Y values between -1 and 1 that represent the direction from the body to look for collisions
                movementFilter, // The settings that determine where a collision can occur on such as layers to collide with
                _castCollisions, // List of collisions to store the found collisions into after the Cast is finished
                _speed * Time.fixedDeltaTime + collisionOffset); // The amount to cast equal to the movement plus an offset

            if (count == 0)
            {
                Vector2 moveVector = direction * _speed * Time.fixedDeltaTime;

                // No collisions
                _body.MovePosition(_body.position + moveVector);
                return true;
            }
            else
            {
                // Print collisions
                foreach (RaycastHit2D hit in _castCollisions)
                {
                    print(hit.ToString());
                }

                return false;
            }
        }
        public void OnMove(InputValue inputValue)
        {
            this._moveInput = inputValue.Get<Vector2>();
        }
        public void OnFire()
        {
            print("Shots fired");
        }
    }
}