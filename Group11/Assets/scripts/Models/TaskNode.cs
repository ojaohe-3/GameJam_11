using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskNode 
{
    public bool Status {get;set;}
    public bool Active {get;set;} 
    public string TaskType{get;set;} //TODO make enum.

    public TaskNode(bool status, bool active, string taskType)
    {
        Status = status;
        Active = active;
        TaskType = taskType;
    }
}
