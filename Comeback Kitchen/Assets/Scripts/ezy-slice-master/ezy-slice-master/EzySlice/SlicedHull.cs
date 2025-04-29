using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EzySlice {

    /**
     * The final generated data structure from a slice operation. This provides easy access
     * to utility functions and the final Mesh data for each section of the HULL.
     */
    public sealed class SlicedHull {
        private Mesh _upperHull;
        private Mesh _lowerHull;

        public SlicedHull(Mesh upperHull, Mesh lowerHull) {
            _upperHull = upperHull;
            _lowerHull = lowerHull;
        }

        public GameObject CreateUpperHull(GameObject original) {
            return CreateUpperHull(original, null);
        }

        public GameObject CreateUpperHull(GameObject original, Material crossSectionMat) {
            GameObject newObject = Object.Instantiate(original);
            newObject.GetComponent<MeshFilter>().mesh = _upperHull;
            newObject.GetComponent<MeshCollider>().sharedMesh = _upperHull;

            if (newObject != null) {
                newObject.transform.localPosition = original.transform.localPosition;
                newObject.transform.localRotation = original.transform.localRotation;
                newObject.transform.localScale = original.transform.localScale;

                Material[] shared = original.GetComponent<MeshRenderer>().sharedMaterials;
                Mesh mesh = original.GetComponent<MeshFilter>().sharedMesh;

                // nothing changed in the hierarchy, the cross section must have been batched
                // with the submeshes, return as is, no need for any changes
                if (mesh.subMeshCount == _upperHull.subMeshCount) {
                    // the the material information
                    newObject.GetComponent<Renderer>().sharedMaterials = shared;

                    return newObject;
                }

                // otherwise the cross section was added to the back of the submesh array because
                // it uses a different material. We need to take this into account
                Material[] newShared = new Material[shared.Length + 1];

                // copy our material arrays across using native copy (should be faster than loop)
                System.Array.Copy(shared, newShared, shared.Length);
                newShared[shared.Length] = crossSectionMat;

                // the the material information
                newObject.GetComponent<Renderer>().sharedMaterials = newShared;
            }

            return newObject;
        }

        public GameObject CreateLowerHull(GameObject original) {
            return CreateLowerHull(original, null);
        }

        public GameObject CreateLowerHull(GameObject original, Material crossSectionMat) {
            GameObject newObject = Object.Instantiate(original);
            newObject.GetComponent<MeshFilter>().mesh = _lowerHull;
            newObject.GetComponent<MeshCollider>().sharedMesh = _lowerHull;

            if (newObject != null) {
                newObject.transform.localPosition = original.transform.localPosition;
                newObject.transform.localRotation = original.transform.localRotation;
                newObject.transform.localScale = original.transform.localScale;

                Material[] shared = original.GetComponent<MeshRenderer>().sharedMaterials;
                Mesh mesh = original.GetComponent<MeshFilter>().sharedMesh;

                // nothing changed in the hierarchy, the cross section must have been batched
                // with the submeshes, return as is, no need for any changes
                if (mesh.subMeshCount == _lowerHull.subMeshCount) {
                    // the the material information
                    newObject.GetComponent<Renderer>().sharedMaterials = shared;

                    return newObject;
                }

                // otherwise the cross section was added to the back of the submesh array because
                // it uses a different material. We need to take this into account
                Material[] newShared = new Material[shared.Length + 1];

                // copy our material arrays across using native copy (should be faster than loop)
                System.Array.Copy(shared, newShared, shared.Length);
                newShared[shared.Length] = crossSectionMat;

                // the the material information
                newObject.GetComponent<Renderer>().sharedMaterials = newShared;
            }

            return newObject;
        }
    }
}