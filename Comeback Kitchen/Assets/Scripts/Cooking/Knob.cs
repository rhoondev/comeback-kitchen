using UnityEngine;

public class Knob : MonoBehaviour
{
    [SerializeField] private Flame flame;

    // Update is called once per frame
    private void Update()
    {
        float rotation = Mod(transform.eulerAngles.y, 360f); // Normalize the rotation to [0, 360]
        float setting = rotation == 0f ? 0f : 1f - rotation / 360f;
        flame.SetFlameSize(setting);
    }

    private float Mod(float a, float b)
    {
        return ((a % b) + b) % b;
    }
}
