using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Malyglut.CubitWorld
{
    public class Cube : MonoBehaviour
    {
        [SerializeField]
        private MeshFilter _meshFilter;

        [SerializeField]
        private MeshRenderer _meshRenderer;

        [SerializeField]
        private List<Cubit> _cubits = new();

        [Button]
        public void CombineMeshes()
        {
            var originalPosition = transform.position;
            var originalScale = transform.localScale;
            
            transform.position = Vector3.zero;
            transform.localScale = Vector3.one;

            CombineCubitMeshes();

            transform.position = originalPosition;
            transform.localScale = originalScale;
        }

        private void CombineCubitMeshes()
        {
            var cubitsSorted = new Dictionary<CubitData, List<Cubit>>();
            var cubitMaterials = new Dictionary<CubitData, Material>();

            //sort cubits by color
            foreach (var cubit in _cubits)
            {
                if (!cubitsSorted.ContainsKey(cubit.Data))
                {
                    cubitsSorted.Add(cubit.Data, new List<Cubit>());
                    cubitMaterials.Add(cubit.Data, cubit.Material);
                }

                cubitsSorted[cubit.Data].Add(cubit);
            }

            var coloredIdx = 0;
            var coloredMeshes = new Mesh[cubitMaterials.Count];
            var coloredCombined = new CombineInstance[coloredMeshes.Length];
            
            //combine cubits of same color into separate meshes
            foreach (var cubitData in cubitsSorted.Keys)
            {
                var cubits = cubitsSorted[cubitData];
                var cubitsCombine = new CombineInstance[cubits.Count];

                //combine cubits into mesh
                for (int i = 0; i < cubits.Count; i++)
                {
                    var cubit = cubits[i];

                    cubitsCombine[i] = new CombineInstance
                    {
                        mesh = cubit.Mesh,
                        transform = cubit.transform.localToWorldMatrix
                    };

                    cubit.DisableVisuals();
                }

                coloredMeshes[coloredIdx] = new Mesh();
                coloredMeshes[coloredIdx].CombineMeshes(cubitsCombine, true, true);

                coloredCombined[coloredIdx] = new CombineInstance
                {
                    mesh = coloredMeshes[coloredIdx],
                    subMeshIndex = 0
                };

                coloredIdx++;
            }

            _meshFilter.sharedMesh = new Mesh();
            _meshFilter.sharedMesh.CombineMeshes(coloredCombined, false, false);

            _meshRenderer.materials = cubitMaterials.Values.ToArray();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }

        public void Add(Cubit cubit)
        {
            _cubits.Add(cubit);
        }
    }
}