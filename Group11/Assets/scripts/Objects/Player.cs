using System;
using System.Collections.Generic;
using Models;
using Unity.VisualScripting;
using Newtonsoft.Json;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace Objects
{
    public class Player : MonoBehaviour
    {
        private Rigidbody2D _body;
        // private Animator _animator;

        [SerializeField] private float _interactionDistance = 1.0f;

        [SerializeField] private float _speed = 10.0f;
        [SerializeField] private float collisionOffset = 0.05f;
        [SerializeField] private ContactFilter2D movementFilter;
        [SerializeField] private PlayerCharacter _ch;

        private List<GameObject> _tasks;
        private Vector2 _moveInput;
        private List<RaycastHit2D> _castCollisions = new List<RaycastHit2D>();

        void Start()
        {
            _tasks = new List<GameObject>(GameObject.FindGameObjectsWithTag("TaskNode"));
            _body = GetComponent<Rigidbody2D>();
            // _animator = GetComponent<Animator>();
            Assert.IsNotNull(_body);
        }

        void OnInteract()
        {
            foreach (var o in _tasks)
            {
                if (Vector2.Distance(o.transform.position, transform.position) < _interactionDistance)
                {
                    Debug.Log("interacted with node");
                }
            }
        }


        void FixedUpdate()
        {
            var queue = GameHandler.MovementQueues.GetValueOrDefault("player", null);
            while (queue is { Count: > 0 })
            {
                Vector2 move;
                if (!queue.TryDequeue(out move))
                    continue;
                // Try to move player in input direction, followed by left right and up down input if failed
                var success = MovePlayer(move);
                if (!success)
                {
                    // Try Left / Right
                    success = MovePlayer(new Vector2(move.x, 0));
                    if (!success)
                    {
                        success = MovePlayer(new Vector2(0, move.y));
                    }
                }

                // _animator.SetBool("isMoving", success);
            }

            if (_moveInput != Vector2.zero)
            {
                GameHandler.Instance.NotifyMove(_moveInput);
            }
            else
            {
                // _animator.SetBool("isMoving", false);
            }
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
                _speed * Time.fixedDeltaTime +
                collisionOffset); // The amount to cast equal to the movement plus an offset

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
                // foreach (RaycastHit2D hit in _castCollisions)
                // {
                //     print(hit.ToString());
                // }

                return false;
            }
        }

        public void OnMove(InputValue inputValue)
        {
            _moveInput = inputValue.Get<Vector2>();
        }

        public void OnFire()
        {
            print("Shots fired");
        }
    }
}