using System;
using System.Collections.Generic;
using Models;
using UnityEngine;
using UnityEngine.Assertions;

namespace Objects
{
    public class Character : MonoBehaviour
    {
        //[SerializeField] private float _speed = 200.0f;
        [SerializeField] protected float _speed = 10.0f;
        private Rigidbody2D _body;
        // [SerializeField] private Vector2 _target;
        //private Animator _animator;
        [SerializeField] public string Name;

        private void Start()
        {
            _body = GetComponent<Rigidbody2D>();
            //_animator = GetComponent<Animator>();
            Assert.IsNotNull(_body);
            // Assert.IsNotNull(_animator);
            // _target = _body.position;
        }

        public void Move(Vector2 newPos)
        {
            _body.MovePosition(newPos);
        }
    }
}