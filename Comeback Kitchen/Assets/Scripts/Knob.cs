using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Knob : MonoBehaviour
{
    [SerializeField] private Transform knob;

    [Tooltip("List of Y-angle offsets (in degrees) FROM the initial controller rotation. " +
             "The first element is your ‘0’-position and will be at localRotation.Y = angles[0].")]
    [SerializeField] private List<float> angles = new List<float> { 0f, 60f, 120f, 180f, 240f, 300f };

    [Tooltip("If true, turning past the last returns you to the first. " +
             "If false, input clamps between angles[0] and angles[last].")]
    [SerializeField] private bool isLooping = false;

    /// <summary>
    /// Fires an integer 0…angles.Count-1 whenever the selected setting changes.
    /// </summary>
    public SmartAction<int> OnValueChanged = new SmartAction<int>();

    private float _startY;      // controller Y at start
    private float[] _boundaries;  // mid-angles between each consecutive pair
    private int _currentIndex = -1;

    private void OnValidate()
    {
        BuildBoundaries();
    }

    private void Awake()
    {
        // 1) build up your boundaries in case OnValidate never ran
        BuildBoundaries();

        // 2) record the world-Y of *this* controller as "zero" for your first angle
        _startY = Mod(transform.eulerAngles.y, 360f);

        // 3) force an initial update so the knob child snaps into place
        _currentIndex = -1;
        UpdateKnob(force: true);
    }

    private void Update()
    {
        UpdateKnob(force: false);
    }

    private void UpdateKnob(bool force)
    {
        if (angles == null || angles.Count < 2 || knob == null)
            return;

        // raw world Y, normalized
        float rawY = Mod(transform.eulerAngles.y, 360f);

        // compute relative to the start
        float rel = Mod(rawY - _startY, 360f);

        // clamp along the arc [angles[0] → angles[last]] if not looping
        if (!isLooping)
        {
            float totalArc = Mod(angles[angles.Count - 1] - angles[0], 360f);
            float clampedArc = Mathf.Clamp(rel - angles[0], 0f, totalArc);
            rel = Mod(angles[0] + clampedArc, 360f);
        }

        // pick the new index
        int idx = PickIndex(rel);

        if (force || idx != _currentIndex)
        {
            _currentIndex = idx;

            // set the child’s local rotation so that:
            //   globalKnobY = controllerY * Quaternion.Euler(0, angles[idx],0)
            knob.localRotation = Quaternion.Euler(0f, angles[idx], 0f);

            if (!force)
            {
                Debug.Log($"Knob value changed → index {_currentIndex}");
                OnValueChanged.Invoke(_currentIndex);
            }
        }
    }

    private void BuildBoundaries()
    {
        if (angles == null || angles.Count < 2)
            return;

        int n = angles.Count;
        _boundaries = new float[isLooping ? n : n - 1];

        for (int i = 0; i < n - 1; i++)
        {
            float a = angles[i], b = angles[i + 1];
            float delta = Mod(b - a, 360f);
            _boundaries[i] = Mod(a + delta * 0.5f, 360f);
        }

        if (isLooping)
        {
            float a = angles[n - 1], b = angles[0];
            float delta = Mod(b - a, 360f);
            _boundaries[n - 1] = Mod(a + delta * 0.5f, 360f);
        }
    }

    private int PickIndex(float relAngle)
    {
        int n = angles.Count;
        if (isLooping)
        {
            // find first boundary that relAngle < boundary
            for (int i = 0; i < n; i++)
                if (AngleLessThan(relAngle, _boundaries[i]))
                    return (i + 1) % n;
            return 0;
        }
        else
        {
            for (int i = 0; i < n - 1; i++)
                if (AngleLessThan(relAngle, _boundaries[i]))
                    return i;
            return n - 1;
        }
    }

    private float Mod(float a, float b) => ((a % b) + b) % b;

    /// <summary>
    /// True if, moving “forward” around the circle, a comes before b.
    /// </summary>
    private bool AngleLessThan(float a, float b)
    {
        float diff = Mod(b - a, 360f);
        return diff > 0f && diff < 180f;
    }
}