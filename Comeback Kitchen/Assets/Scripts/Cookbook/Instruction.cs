using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Instruction
{
    [field: SerializeField] public string Text { get; private set; }
    [field: SerializeField] public List<Sprite> Images { get; private set; }
}
