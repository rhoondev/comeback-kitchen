using UnityEngine;

public class SliceCooldown : MonoBehaviour
{
    private float cooldownTime = 1f;
    public bool CanBeSliced => Time.time >= _spawnTime + cooldownTime;

    private float _spawnTime;

    void Awake()
    {
        Debug.Log("Spawn time");
        _spawnTime = Time.time;
    }
}