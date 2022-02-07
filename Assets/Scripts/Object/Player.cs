using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float maxDamage;
    [SerializeField] private int coin;
    [SerializeField] private List<int> checks = new List<int>();
    [SerializeField] private int[] checks1;
    private Dictionary<int, int> checkMap = new Dictionary<int, int>();

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
        int count = Random.Range(1, 3);
        for (int i = 0; i < count; i++)
        {
            checks.Add(Random.Range(1, 10));
        }
        checks1 = new int[count];
        for (int i = 0; i < count; i++)
        {
            checks1[i] = Random.Range(1, 10);
        }
    }
}
