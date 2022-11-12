using System;
using Models;
using UnityEngine;

namespace Objects
{
    public class NodeObject : MonoBehaviour
    {
        public Action<bool> StatusChange { get; set; }
        
        [SerializeField] private TaskNode _node;

        public bool isCompleteable()
        {
            return _node.active && !_node.status;
        }

        public void Complete()
        {
            if(_node.active != true) return;
            
            _node.status = true;
            StatusChange?.Invoke(true);
        }
        public bool Sabotarge()
        {
            if(_node.active != true) return false;

            _node.status = false;
              StatusChange?.Invoke(false);
              return true;
        }
    }
}
