using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableManager : Singleton<TableManager>
{
    public Transform furnitureParent;

    public List<Table> tables = new List<Table>();
    public List<Chair> chairs = new List<Chair>();

    int tableCount = 0;
    int chairCount = 0;

    public float checkInterval;
    float timer;

    private void Start()
    {
        tables.Clear();

        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform t = transform.GetChild(i);
            if (t.tag == "Table")
            {
                AddTable(t);
            }
            if (t.tag == "Chair")
            {
                AddChair(t);
            }
        }

        UpdateTables();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            timer = 0f;
            UpdateTables();
        }
    }

    public void AddTable(Transform table)
    {
        tableCount++;
        tables.Add(new Table(table, new List<Chair>()));
    }

    public void RemoveTable(Transform table)
    {
        tableCount--;
        int index = 0;
        foreach (Table t in tables)
        {
            if (t.table == table)
            {
                break;
            }
            index++;
        }

        tables.RemoveAt(index);
    }

    public void AddChair(Transform chair)
    {
        chairCount++;
        chairs.Add(new Chair(chair, false));
    }

    void UpdateTables()
    {
        int childCount = transform.childCount;

        //Empty all table chairs
        for (int c = 0; c < tableCount; c++)
        {
            Table t = tables[c];
            t.closeChairs.Clear();
        }

        //Fill all table chairs
        for (int i = 0; i < chairCount; i++)
        {
            Chair c = chairs[i];
            Transform t = c.transform;

            float closest = Mathf.Infinity;
            int xIndex = 0;

            for (int x = 0; x < tableCount; x++)
            {
                float distance = (tables[x].table.transform.position - t.position).magnitude;
                if (distance < closest)
                {
                    closest = distance;
                    xIndex = x;
                }
            }

            tables[xIndex].closeChairs.Add(c);
        }
    }

    public Chair[] GetBestSeats(int customerAmount) //Implement: if 1 customer, find a random table with a free seat
    {
        int closestMore = int.MaxValue;
        int closestMoreIndex = -1;

        int totalFreeChairs = 0;
        List<Table> freeTables = new List<Table>();
        List<int> freeSeats = new List<int>();

        int i = 0;
        foreach (Table t in tables)
        {
            int chairCount = t.closeChairs.Count;

            if (chairCount > 0)
            {
                int amountOfFreeSeats = GetAmountOfFreeSeats(t);
                if (amountOfFreeSeats > 0)
                {
                    totalFreeChairs += amountOfFreeSeats;

                    if (chairCount == customerAmount && amountOfFreeSeats == customerAmount) //The perfect table
                    {
                        return GetFreeSeats(t, customerAmount, chairCount);
                    }
                    else if (chairCount > customerAmount && chairCount < closestMore && amountOfFreeSeats >= customerAmount) //A table with more chairs
                    {
                        closestMore = chairCount;
                        closestMoreIndex = i;
                    }
                    else //We might need multiple tables, list up some with free seats
                    {
                        freeTables.Add(t);
                        freeSeats.Add(amountOfFreeSeats);
                    }
                }
            }

            i++;
        }

        if (closestMoreIndex != -1)
        {
            Table t = tables[closestMoreIndex];
            return GetFreeSeats(t, customerAmount, t.closeChairs.Count);
        }
        else //We need multiple tables
        {
            if (totalFreeChairs > customerAmount) //There are enough seats
            {
                //Find index
                return GetFreeSeats(FindFreeTables(freeTables, freeSeats, customerAmount), customerAmount);
            }
            else //There are not enough seats
            {
                //For the moment everyone leaves
                return null;
            }
        }
    }

    int GetAmountOfFreeSeats(Table t)
    {
        int freeSeats = 0;
        foreach (Chair c in t.closeChairs)
        {
            if (!c.occupied)
            {
                freeSeats++;
            }
        }
        return freeSeats;
    }

    //Finds random tables with free seats
    public Table[] FindFreeTables(List<Table> freeTables, List<int> freeSeats, int requiredSeats)
    {
        int count = freeTables.Count;
        int start = Random.Range(0, count);

        int total = 0;
        List<Table> selectedTables = new List<Table>();

        for (int i = 0; i < count; i++)
        {
            int index = start + i;
            if (index >= count)
            {
                index = start + i - count;
            }

            total += freeSeats[index];
            selectedTables.Add(freeTables[index]);

            if (total > requiredSeats)
            {
                break;
            }
        }

        return selectedTables.ToArray();
    }

    //Returns an amount of free seats from a single table
    public Chair[] GetFreeSeats(Table table, int amount, int chairCount)
    {
        Chair[] seats = new Chair[amount];
        int index = 0;

        for (int i = 0; i < chairCount; i++)
        {
            if (!table.closeChairs[i].occupied)
            {
                seats[index] = table.closeChairs[i];
                index++;

                if (index == amount)
                {
                    return seats;
                }
            }
        }
        return null;
    }

    //From multiple tables
    public Chair[] GetFreeSeats(Table[] table, int amount)
    {
        Chair[] seats = new Chair[amount];
        int index = 0;

        int i = 0;
        foreach (Table t in table)
        {
            Chair[] foundSeats = GetFreeSeats(t, amount - index, t.closeChairs.Count);

            int len = foundSeats.Length;
            for (int x = 0; x < len; x++)
            {
                seats[index] = foundSeats[x];

                if (index == amount)
                {
                    return seats;
                }
            }

            if (index == amount)
            {
                return seats;
            }

            i++;
        }

        if (index == amount)
        {
            return seats;
        }

        return null;
    }
}
