using EzySlice;
using UnityEngine;
using System.Collections.Generic;
using System;

public class Blender : MonoBehaviour
{
    [SerializeField] private Liquid liquid;
    [SerializeField] private Transform knob;
    [SerializeField] private Transform blade;
    [SerializeField] private Material cutTomatoMaterial;
    [SerializeField] private int maxPowerLevel = 4; // # of buttons - 1
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float minChunkSize;
    [SerializeField] private float minSliceCooldown; // At the highest power level, time between slices
    [SerializeField] private float maxSliceCooldown; // At the lowest power level, time between slices
    [SerializeField] private float minSliceForce;
    [SerializeField] private float maxSliceForce;
    [SerializeField] private float maxSpinForceStrength;
    [SerializeField] private float maxLiftForceStrength;
    [SerializeField] private float scaleIncrease;
    [SerializeField] private int maxFillAmountPerSlice;
    [SerializeField] private int minFillAmountPerSlice;
    [SerializeField] private int fillAmountDecreaseObjectCountCutoff; // Determines the number of objects after which the fill amount per slice stops decreasing
    [SerializeField] private float maxObjectSpeed;

    private int _powerLevel = 0;
    private float _speed = 0f;
    private float _sliceForce = 0f;
    private float _sliceCooldown = 0f;
    private float _lastSliceTime = 0f;
    private int _maxBlendableObjectCount = 0;
    private HashSet<Collider> _blendableObjects = new HashSet<Collider>();

    private void Update()
    {
        float angle = knob.localEulerAngles.y;
        if (angle > 90f) angle -= 360f; // Convert to -90 to 90 range
        if (angle < -90f) angle += 360f;
        _powerLevel = 2 - Mathf.RoundToInt(angle / 45f); // 90 to -90 degrees mapped to 0 to 5

        if (_powerLevel > 0)
        {
            float t = (_powerLevel - 1) / ((float)maxPowerLevel - 1);
            _sliceCooldown = Mathf.Lerp(maxSliceCooldown, minSliceCooldown, t);
        }
        else
        {
            _sliceCooldown = float.MaxValue; // effectively disable slicing
        }

        _sliceForce = Mathf.Lerp(minSliceForce, maxSliceForce, (_powerLevel - 1) / ((float)maxPowerLevel - 1));

        // Debug.Log("Power level: " + _powerLevel);
        // Debug.Log("Slice cooldown: " + _sliceCooldown);
        // Debug.Log("Slice force: " + _sliceForce);
        // Debug.Log("Time since last slice: " + (Time.time - _lastSliceTime));
        // Debug.Log("Blendable objects in range: " + _blendableObjects.Count);

        AttemptSlice();
    }

    private void FixedUpdate()
    {
        // Spin blade
        float targetSpeed = maxSpeed * (_powerLevel / (float)maxPowerLevel);
        _speed = Mathf.MoveTowards(_speed, targetSpeed, acceleration * Time.fixedDeltaTime);
        blade.Rotate(0f, 0f, _speed * Time.fixedDeltaTime);

        // Spin contents
        foreach (Collider obj in _blendableObjects)
        {
            if (obj != null)
            {
                Rigidbody rb = obj.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    Vector2 offset = new Vector2(
                        obj.transform.position.x - transform.position.x,
                        obj.transform.position.z - transform.position.z
                    );

                    Vector2 spinForceDirection = new Vector2(-offset.y, offset.x).normalized;
                    float spinForceStrength = _speed / maxSpeed * maxSpinForceStrength;
                    Vector3 spinForce = new Vector3(spinForceDirection.x, 0f, spinForceDirection.y) * spinForceStrength;

                    float liftForceStrength = _speed / maxSpeed * maxLiftForceStrength;
                    Vector3 liftForce = Vector3.up * liftForceStrength;

                    if (rb.linearVelocity.magnitude < maxObjectSpeed)
                    {
                        rb.AddForce(spinForce, ForceMode.Force);
                    }

                    rb.AddForce(liftForce, ForceMode.Force);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Blendable"))
        {
            _blendableObjects.Add(other);
            _maxBlendableObjectCount = Mathf.Max(_maxBlendableObjectCount, _blendableObjects.Count);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _blendableObjects.Remove(other);
    }

    private void AttemptSlice()
    {
        if (Time.time - _lastSliceTime < _sliceCooldown)
        {
            return;
        }

        // Clean up any null references of objects that have been destroyed
        _blendableObjects.RemoveWhere(x => x == null);

        // Check if we have any objects to slice
        if (_blendableObjects.Count == 0)
        {
            return;
        }

        // Always cut the largest object first
        Collider largestObj = null;
        float maxVolume = -1f;

        foreach (var col in _blendableObjects)
        {
            float volume = col.bounds.size.sqrMagnitude;
            if (volume > maxVolume)
            {
                maxVolume = volume;
                largestObj = col;
            }
        }

        if (largestObj != null)
        {
            SliceTarget(largestObj);
        }
        else
        {
            // Debug.Log("Largest object calculation failed: largestObj is null");
        }
    }

    private void SliceTarget(Collider other)
    {
        GameObject target = other.gameObject;
        Vector3 center;
        Vector3 normal;

        MeshFilter mf = target.GetComponent<MeshFilter>();
        if (mf != null && mf.sharedMesh != null)
        {
            // Use mesh bounds for center and size calculation
            center = target.transform.TransformPoint(mf.sharedMesh.bounds.center);

            Vector3 size = mf.sharedMesh.bounds.size;
            Vector3 scale = target.transform.lossyScale;

            float xSize = size.x * Mathf.Abs(scale.x);
            float ySize = size.y * Mathf.Abs(scale.y);
            float zSize = size.z * Mathf.Abs(scale.z);

            if (xSize >= ySize && xSize >= zSize)
                normal = target.transform.right;
            else if (ySize >= xSize && ySize >= zSize)
                normal = target.transform.up;
            else
                normal = target.transform.forward;
        }
        else
        {
            // Fallback to collider bounds if no mesh filter (less accurate for rotated objects)
            // Debug.Log("MeshFilter or sharedMesh is null on target: " + target.name + ". Using collider bounds for size calculation.");

            center = other.bounds.center;
            Vector3 size = other.bounds.size;

            if (size.x >= size.y && size.x >= size.z)
                normal = Vector3.right;
            else if (size.y >= size.x && size.y >= size.z)
                normal = Vector3.up;
            else
                normal = Vector3.forward;
        }

        // FIX: Do not construct the Plane manually here. 
        // Passing the World Space vectors directly to the Slice method allows EzySlice 
        // to handle the World-to-Local conversion internally.
        SlicedHull hull = target.Slice(center, normal, cutTomatoMaterial);

        if (hull != null)
        {
            // Create the two halves
            GameObject upperHull = hull.CreateUpperHull(target, cutTomatoMaterial);
            GameObject lowerHull = hull.CreateLowerHull(target, cutTomatoMaterial);

            // Give the new pieces some force, or destroy them if too small
            HandleNewChunk(upperHull);
            HandleNewChunk(lowerHull);

            // Destroy the original
            Destroy(target);

            // Increase how full the blender is
            int fillAmount = Mathf.RoundToInt(Mathf.Lerp(
                maxFillAmountPerSlice,
                minFillAmountPerSlice,
                Mathf.Min(_maxBlendableObjectCount, fillAmountDecreaseObjectCountCutoff) / (float)fillAmountDecreaseObjectCountCutoff
            ));
            // Debug.Log(_maxBlendableObjectCount + " max blendable objects counted, filling blender by " + fillAmount);
            liquid.Fill(fillAmount);

            _lastSliceTime = Time.time;
        }
        else
        {
            // Debug.Log("Slicing failed: hull is null");
        }
    }

    // Logic that is applied to every sliced object that is sliced by the blender
    private void HandleNewChunk(GameObject obj)
    {
        obj.transform.localScale += Vector3.one * scaleIncrease;

        // Calculate world size using Renderer bounds
        float currentSize = 0f;
        Renderer r = obj.GetComponent<Renderer>();
        if (r != null)
        {
            currentSize = r.bounds.size.magnitude;
        }

        // Check if it's too small to keep
        if (currentSize < minChunkSize)
        {
            Destroy(obj);
            // Debug.Log("Chunk destroyed.");
        }
        else
        {
            // Debug.Log("Current chunk size: " + currentSize);
            obj.GetComponent<Rigidbody>().AddForce(Vector3.up * _sliceForce, ForceMode.Impulse);
        }
    }
}
