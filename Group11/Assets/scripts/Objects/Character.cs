using System;
using Models;
using UnityEngine;
using UnityEngine.Assertions;

namespace Objects
{
    public class Character : MonoBehaviour
    {
        [SerializeField] private float _speed = 200.0f;
        private Rigidbody2D _body;
        [SerializeField]
        private Vector2 _target;
        //private Animator _animator;
        [SerializeField]
        private PlayerCharacter _ch;

        private readonly string _name;

        public Character(string name)
        {
            _name = name;
        }

        private void Start()
        {
            _body = GetComponent<Rigidbody2D>();
            //_animator = GetComponent<Animator>();
            Assert.IsNotNull(_body);
            // Assert.IsNotNull(_animator);
            // _target = _body.position;
        }

        private void FixedUpdate()
        {

            if (Vector2.Distance(_target, _body.position) > 0.1f)
            {

                var direction = (_target - _body.position).normalized;
                _body.velocity = direction * (_speed * Time.deltaTime);
                //_animator.SetBool("isMoving",true);
                // _body.MovePosition(direction * (_speed * Time.deltaTime));
            }
            else
            {
                _body.velocity = Vector2.zero;
                //_animator.SetBool("isMoving", false);
            }
        }

        public void OnSetTarget(Vector2 target)
        {

            this._target = target;
        }

    }
}