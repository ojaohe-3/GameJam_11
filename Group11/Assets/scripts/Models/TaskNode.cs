using System;

namespace Models
{
    [Serializable]
    public struct TaskNode
    {
        public bool status;
        public bool active;
        public string taskType; //TODO make enum.

    }
}
