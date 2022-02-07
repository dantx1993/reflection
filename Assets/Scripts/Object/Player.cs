using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float maxDamage;
    [SerializeField] private int coin;

    private void Awake() 
    {
        ReflectionStorage.Bind(this);
    }

    private void OnDestroy() 
    {
        ReflectionStorage.Unbind(this);
    }

    private void Start() 
    {
        maxHealth = Random.Range(10, 100);
        maxDamage = Random.Range(5, 10);
        coin = Random.Range(1000, 5000);
    }
}
