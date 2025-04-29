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
    [SerializeField] private Transform knobController;
    [SerializeField] private Flame flame;

    public SmartAction<StoveSetting> OnSettingChanged = new SmartAction<StoveSetting>();

    private StoveSetting _currentSetting = StoveSetting.Off;

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
        float controllerRotation = Mod(knobController.eulerAngles.y, 360f); // Normalize the rotation to [0, 360]
        StoveSetting newSetting = GetStoveSetting(controllerRotation);

        if (newSetting != _currentSetting)
        {
            _currentSetting = newSetting;

            float knobRotation = GetKnobRotation(_currentSetting);
            knob.rotation = Quaternion.Euler(0f, knobRotation, 0f);

            float flameSize = (knobRotation == 0f || knobRotation == 360f) ? 0f : 1f - knobRotation / 360f;
            flame.SetFlameSize(flameSize);

            Debug.Log($"Stove setting changed to: {_currentSetting}");

            OnSettingChanged.Invoke(_currentSetting);
        }
    }

    private float Mod(float a, float b)
    {
        return ((a % b) + b) % b;
    }

    private StoveSetting GetStoveSetting(float rotation)
    {
        if (rotation >= 330f || rotation < 30f) return StoveSetting.Off; // 330 - 30
        if (rotation < 90f) return StoveSetting.High; // 30 - 90
        if (rotation < 150f) return StoveSetting.MediumHigh; // 90 - 150
        if (rotation < 210f) return StoveSetting.Medium; // 150 - 210
        if (rotation < 270f) return StoveSetting.MediumLow; // 210 - 270
        if (rotation < 330f) return StoveSetting.Low; // 270 - 330

        throw new System.ArgumentOutOfRangeException(nameof(rotation), rotation, null);
    }

    private float GetKnobRotation(StoveSetting setting)
    {
        switch (setting)
        {
            case StoveSetting.Off:
                return 0f;
            case StoveSetting.High:
                return 60f;
            case StoveSetting.MediumHigh:
                return 120f;
            case StoveSetting.Medium:
                return 180f;
            case StoveSetting.MediumLow:
                return 240f;
            case StoveSetting.Low:
                return 300f;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(setting), setting, null);
        }
    }
}
