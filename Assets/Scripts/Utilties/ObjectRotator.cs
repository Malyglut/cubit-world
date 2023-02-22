using UnityEngine;

namespace Malyglut.CubitWorld.Utilties
{
    public class ObjectRotator : MonoBehaviour
    {
        [SerializeField]
        private float _rotationSpeed = 1f;

        private void OnEnable()
        {
            transform.rotation = Quaternion.identity;
        }

        private void Update()
        {
            var rotation = Vector3.up * (_rotationSpeed * Time.deltaTime);
            transform.Rotate(rotation);
        }
    }
}