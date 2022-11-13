using System;
using System.Collections.Generic;
using Models;
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

        private AudioSource _audioSource;
        private List<GameObject> _tasks;
        private Vector2 _moveInput;
        private List<RaycastHit2D> _castCollisions = new List<RaycastHit2D>();

        void Start()
        {
            _audioSource = GetComponent<AudioSource>();

            _tasks = new List<GameObject>(GameObject.FindGameObjectsWithTag("TaskNode"));
            _body = GetComponent<Rigidbody2D>();
            // _animator = GetComponent<Animator>();
            Assert.IsNotNull(_body);
            GameHandler.Player = this;
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
            if(_ch.impostor != true) return;

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
            if (_moveInput != Vector2.zero)
            {
                // _animator.SetBool("isMoving", success);
                var move = AttemptMove(_moveInput);
                // Send to Server movement if movement
                if (!move.Equals(Vector2.zero))
                {
                    var newPos = Move(move);
                    GameHandler.Instance.NotifyPos(newPos);
                }
            }
            else
            {
                // _animator.SetBool("isMoving", false);
            }
        }

        // Try to move player in input direction, followed by left right and up down input if failed.
        // Returns null vector if move failed.
        Vector2 AttemptMove(Vector2 move)
        {
            if (DetectCollision(move) > 0)
            {
                if (DetectCollision(new Vector2(move.x, 0)) == 0)
                    move = new Vector2(move.x, 0);
                else if (DetectCollision(new Vector2(0, move.y)) == 0)
                    move = new Vector2(0, move.y);
                else
                    move = Vector2.zero;
            }

            return move;
        }

        // Move the player in a direction and returns the new position
        public Vector2 Move(Vector2 direction)
        {
            
            Vector2 moveVector = direction * _speed * Time.fixedDeltaTime;
            var newPos = _body.position + moveVector;
            _body.MovePosition(newPos);
            if (_audioSource.isPlaying == false)
            {
                _audioSource.PlayScheduled(1f);
            }

            return newPos;
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

        public Vector2 GetPos()
        {
            return _body.position;
        }
    }
}