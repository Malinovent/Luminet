using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using UnityEngine.Events;

public class Health3D : NetworkBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int startingHealth;
    [SerializeField] private GameObject[] healthNodes;

    [SerializeField] private UnityEvent onDie;

    private NetworkVariable<int> currentHealth = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        if (maxHealth > healthNodes.Length)
        {
            Debug.LogError($"Max health exceeds the number of health nodes for GameObject {this.gameObject}.");
            return;
        }

        base.OnNetworkSpawn();

        if (IsServer)
        {
            currentHealth.Value = startingHealth;
        }

        currentHealth.OnValueChanged += (oldVal, newVal) => UpdateHealthNodes();
        UpdateHealthNodes();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            HealDamage();
        }

        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            TakeDamage();
        }
    }

    private void UpdateHealthNodes()
    {
        Color color = Color.green;

        
        if ( currentHealth.Value == 1 || (float)currentHealth.Value <= (float)maxHealth / 4)
        {
            color = Color.red;
        }
        else if ((float)currentHealth.Value <= (float)maxHealth / 2)
        {
            color = Color.yellow;
        }

        for (int i = 0; i < healthNodes.Length; i++)
        {
            if (i < currentHealth.Value)
            {
                healthNodes[i].SetActive(true);
                healthNodes[i].GetComponent<Renderer>().material.color = color;
            }
            else
            {
                healthNodes[i].SetActive(false);
            }
        }
    }

    public void TakeDamage(int amount = 1)
    {
        if(!IsServer) return;

        currentHealth.Value -= amount;
        UpdateHealthNodes();

        if (currentHealth.Value <= 0)
        {              
            OnDie();
        }
    }

    public void HealDamage()
    {
        if (!IsServer) return;

        currentHealth.Value++;

        if (currentHealth.Value > maxHealth)
        {
            currentHealth.Value = maxHealth;
        }

        UpdateHealthNodes();
    }

    public void OnDie()
    {
        Debug.Log("GameObject destroyed.");
        onDie?.Invoke();
    }
}
