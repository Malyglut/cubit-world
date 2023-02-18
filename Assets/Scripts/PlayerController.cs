using System;
using UnityEngine;

namespace Malyglut.CubitWorld
{
    public class PlayerController : MonoBehaviour
    {
        private const float RAYCAST_MAX_DISTANCE = 500000f;
        
        [SerializeField]
        private LayerMask _cubitsLayer;
        
        [SerializeField]
        private LayerMask _cubesLayer;

        [SerializeField]
        private Camera _camera;
        
        [SerializeField]
        private float _moveSpeed = 2f;

        [SerializeField]
        private CubitData _selectedCubit;

        [SerializeField]
        private CubitPlacementSystem _placement;

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                var ray = new Ray(_camera.transform.position, _camera.transform.forward);
                

                    if (Physics.Raycast(ray, out var cubitHit, RAYCAST_MAX_DISTANCE, _cubitsLayer))
                    {
                        var targetCubit = cubitHit.transform.GetComponentInParent<Cubit>();
                        _placement.PlaceCubit(targetCubit, cubitHit.normal, _selectedCubit);
                    }
                
            }
        }

        private void OnDrawGizmos()
        {
            
            var ray = new Ray(_camera.transform.position, _camera.transform.forward);

            if (Physics.Raycast(ray, out var hit, RAYCAST_MAX_DISTANCE, _cubitsLayer))
            {
                Gizmos.color=Color.red;
                Gizmos.DrawSphere(hit.point, .2f);

                var normalRayLength = 2f;
                
                Gizmos.color=Color.yellow;
                Gizmos.DrawLine(hit.point, hit.point + hit.normal * normalRayLength);
            }
        }
    }
}