using System;
using Models;
using UnityEngine;

namespace Objects
{
    public class NodeObject : MonoBehaviour
    {
        public Action<bool> StatusChange { get; set; }
        private SpriteRenderer _sprite;
        [SerializeField] private TaskNode _node;
        
        public bool isCompleteable()
        {
            return _node.active && !_node.status;
        }

        public void Complete()
        {
            if(_node.active != true || _node.status) return;
            
            _sprite.color = Color.green;
            _node.status = true;
            StatusChange?.Invoke(true);
        }
        public bool Sabotarge()
        {
            if(_node.active != true || _node.status == false) return false;

            _node.status = false;
              StatusChange?.Invoke(false);
              _sprite.color = Color.red;
              return true;
        }

        private void Start()
        {
            _sprite = GetComponent<SpriteRenderer>();
            _sprite.color = Color.red;

        }
    }
}
