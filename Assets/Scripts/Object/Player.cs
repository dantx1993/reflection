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
    [SerializeField] private Test test = new Test();

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
        for (int i = 0; i < count; i++)
        {
            checkMap.Add(i, Random.Range(1, 5));
        }
        // test = new Test();
    }
}

[System.Serializable]
public class Test
{
    public List<int> myList = new List<int>();
    public Dictionary<string, int> myDict = new Dictionary<string, int>();
    public int number;

    public Test()
    {
        Debug.LogWarning("Contruction");
        number = 100;
        myList.Add(3);
        myList.Add(2);
        myList.Add(1);
        myDict.Add("Dan1", 10);
        myDict.Add("Dan2", 10);
        // ReflectionStorage.Bind(this);
    }
    

    ~Test()
    {
        Debug.LogWarning("Destruction");
        // ReflectionStorage.Unbind(this);
    }
}
