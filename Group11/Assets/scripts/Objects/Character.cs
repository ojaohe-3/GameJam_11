using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Objects
{
    public class Character : MonoBehaviour
    {
        [SerializeField] private float _speed = 10.0f;
        private Rigidbody2D _body;
        private Vector2 _target;
        private Action<Vector2> invoke_target;
        private void Start()
        {
            _body = GetComponent<Rigidbody2D>();
            Assert.IsNotNull(_body);
            _target = _body.position;
            invoke_target += OnSetTarget;
        }

        private void FixedUpdate()
        {
            invoke_target?.Invoke(Vector2.right);

            if (Vector2.Distance(_target, _body.position) > 0.1f)
            {
                _body.MovePosition(_target * (_speed * Time.deltaTime));
            }
        }

        public void OnSetTarget(Vector2 target)
        {
            
            this._target = target;
        }
        
    }
}