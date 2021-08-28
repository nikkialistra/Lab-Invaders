using UnityEngine;

namespace Core
{
    public class CopyTransform : MonoBehaviour
    {
        [SerializeField] private Transform _target;

        private void Update()
        {
            transform.position = _target.position;
        }
    }
}