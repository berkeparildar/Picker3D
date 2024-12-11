using Runtime.Data.ValueObjects;
using UnityEngine;

namespace Runtime.Data.UnityObjects
{
    [CreateAssetMenu(fileName = "InputSO", menuName = "Picker3D/Data/InputSO", order = 0)]
    public class InputSO : ScriptableObject
    {
        public InputData InputData;
    }
}