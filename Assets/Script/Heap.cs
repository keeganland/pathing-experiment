using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Necessary rule for the heap's binary tree: every parent node is less than its children
 * Given a specific index n 
 * Finding a parent node: floor of (n-1)/2 
 * Finding left child: 2n + 1
 * Finding right child: 2n + 2s
 */

public class Heap<T> where T : IHeapItem<T> {

    private T[] items; //Usually PathNodes, given what I'm coding this for
    private int currentItemCount;

    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
        currentItemCount = 0; //Not in video. isn't it better to explicitly do this?
    }

    public void Add(T item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }

    public T RemoveFirst()
    {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount]; //Stick the item that was originally at the end of the array at the start of the array.
        items[0].HeapIndex = 0;
        SortDown(items[0]); //Have it travel down the tree in such a way as to maintain correct heap structure.
        return firstItem;
    }

    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }

    public int Count
    {
        get
        {
            return currentItemCount;
        }
    }

    /*
     * Changes the priority of an item - like when we find a node in the open set that we now have a better priority for.
     */ 
    public void UpdateItem(T item)
    {
        SortUp(item);
        //We don't need a SortDown here too as long as we're just doing pathfinding - priorities can only get better
    }

    private void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            if (childIndexLeft < currentItemCount)
            {
                swapIndex = childIndexLeft;

                if (childIndexRight < currentItemCount)
                {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else //If the child nodes are both bigger than the parent, we satisfy the condition, and don't need t
                {
                    return;
                }
            }
            else //If there aren't any child nodes then of course we also still satisfy the condition.
            {
                return;
            }
        }
    }    
    private void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;

        while (true)
        {
            T parentItem = items[parentIndex];
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(parentItem, item);
            }
            else
            {
                break;
            }
        }
    }
    private void Swap(T itemA, T itemB)
    {
        int temp;

        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;

        temp = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = temp;
    }

}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}
