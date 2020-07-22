//A party consists of multiple customers, contains the tables they are at
using UnityEngine;

[System.Serializable]
public class Party
{
    public Customer[] partyMembers;
    public Chair[] seats;

    public Party(Customer[] partyMembers, Chair[] seats)
    {
        this.partyMembers = partyMembers;
        this.seats = seats;
    }
}

