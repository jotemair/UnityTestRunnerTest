﻿using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ItemCollectEvent : UnityEvent<Item> { }

public class Item : MonoBehaviour
{
    public int score = 1;
    public ItemCollectEvent onCollect = new ItemCollectEvent();

    public void Collect()
    {
        GameManager.Instance.AddScore(1);
        // Run collect event
        onCollect.Invoke(this);
        // Destroy item
        Destroy(gameObject);
    }
}
