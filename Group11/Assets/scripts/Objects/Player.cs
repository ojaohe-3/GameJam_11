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
                    var node = o.GetComponent<NodeObject>();
                    node.Complete();
                    return;
                }
            }
        }

        void OnSabotage()
        {
            foreach (var o in _tasks)
            {
                if (Vector2.Distance(o.transform.position, transform.position) < _interactionDistance)
                {
                    var node = o.GetComponent<NodeObject>();
                    node.Sabotarge();
                    return;
                    ;
                }
            }
        }

        void FixedUpdate()
        {
            var queue = GameHandler.MovementQueues.GetValueOrDefault("player", null);
            while (queue is { Count: > 0 })
            {
                Vector2 move;
                if (queue.TryDequeue(out move))
                    MovePlayer(move);
            }

            if (_moveInput != Vector2.zero)
            {
                // _animator.SetBool("isMoving", success);
                AttemptMove(_moveInput);
            }
            else
            {
                // _animator.SetBool("isMoving", false);
            }
        }

        void AttemptMove(Vector2 move)
        {
            // Try to move player in input direction, followed by left right and up down input if failed
            if (DetectCollision(move) > 0)
            {
                if (DetectCollision(new Vector2(move.x, 0)) == 0)
                    move = new Vector2(move.x, 0);
                else if (DetectCollision(new Vector2(0, move.y)) == 0)
                    move = new Vector2(0, move.y);
                else
                    move = Vector2.zero;
            }

            // Send to Server movement if movement
            if (!move.Equals(Vector2.zero))
                GameHandler.Instance.NotifyMove(move);
        }

        // Tries to move the player in a direction by casting in that direction by the amount
        // moved plus an offset. If no collisions are found, it moves the players
        // Returns true or false depending on if a move was executed
        public void MovePlayer(Vector2 direction)
        {
            Vector2 moveVector = direction * _speed * Time.fixedDeltaTime;
            _body.MovePosition(_body.position + moveVector);
        }

        private int DetectCollision(Vector2 direction)
        {
            return _body.Cast(
                direction, // X and Y values between -1 and 1 that represent the direction from the body to look for collisions
                movementFilter, // The settings that determine where a collision can occur on such as layers to collide with
                _castCollisions, // List of collisions to store the found collisions into after the Cast is finished
                _speed * Time.fixedDeltaTime +
                collisionOffset);
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