using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class StatusManager : Singleton<StatusManager>
{
    [SerializeField] List<Status> registeredStatuses; 

    public Status FindStatus(string name)
    {
        return registeredStatuses.Find(x => x.name.Equals(name));
    }
}
