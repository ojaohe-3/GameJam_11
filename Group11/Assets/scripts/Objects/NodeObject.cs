using System;
using Models;
using UnityEngine;

namespace Objects
{
    public class NodeObject : MonoBehaviour
    {
        public Action<bool> _statusChange { get; set; }
        
        [SerializeField] private TaskNode _node;
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
