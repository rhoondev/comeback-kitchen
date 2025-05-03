using UnityEngine;

public class SliceCooldown : MonoBehaviour
{
    private float cooldownTime = 0.1f;
    public bool CanBeSliced => Time.time >= _spawnTime + cooldownTime;

    private float _spawnTime;
    public int futureSlicesCount = 4; // Max Number of cuts before destroying

    void Awake()
    {
        _spawnTime = Time.time;
    }
}