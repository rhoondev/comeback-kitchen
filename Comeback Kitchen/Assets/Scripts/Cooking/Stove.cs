using UnityEngine;

public enum StoveSetting
{
    Off,
    Low,
    MediumLow,
    Medium,
    MediumHigh,
    High
}

public class Stove : MonoBehaviour
{
    [SerializeField] private Transform knob;
    [SerializeField] private Flame flame;

    public SmartAction<StoveSetting> OnSettingChanged = new SmartAction<StoveSetting>();

    public void LockKnob()
    {
        // Lock the motion of the knob
        Debug.Log("Knob locked");
    }

    public void UnlockKnob()
    {
        // Unlock the motion of the knob
        Debug.Log("Knob unlocked");
    }

    // Update is called once per frame
    private void Update()
    {
        float rotation = Mod(knob.eulerAngles.y, 360f); // Normalize the rotation to [0, 360]
        float setting = rotation == 0f ? 0f : 1f - rotation / 360f;
        flame.SetFlameSize(setting);
    }

    private float Mod(float a, float b)
    {
        return ((a % b) + b) % b;
    }
}
