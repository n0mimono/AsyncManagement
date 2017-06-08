using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AsyncManagement {
  public class AsyncLodManager : MonoBehaviour {
    private AsyncLodGroup[] groups;

    private CullingGroup cull;
    private BoundingSphere[] spheres;

    private readonly float[] distances = new float[] {
      25f, 50f, 100f,
    };

    IEnumerator Start() {
      groups = GameObject.FindObjectsOfType<AsyncLodGroup>();

      cull = new CullingGroup();
      cull.targetCamera = Camera.main;
      cull.SetDistanceReferencePoint(Camera.main.transform);

      spheres = new BoundingSphere[groups.Length];
      for (int i = 0; i < spheres.Length; i++) {
        spheres[i].radius = 1f;
      }
      cull.SetBoundingSpheres(spheres);
      cull.SetBoundingSphereCount(spheres.Length);

      cull.SetBoundingDistances(distances);
      cull.onStateChanged += (ev) => {
        int level = ev.isVisible ? ev.currentDistance : distances.Length;
        groups[ev.index].SetLevel(level);
      };
      yield return null;

      GameObject.FindObjectOfType<AsyncUpdateManager>().StartUpdate();
      yield return null;

      SetActive (true);
    }

    void Update() {
      if (cull == null) return;

      for (int i = 0; i < spheres.Length; i++) {
        spheres[i].position = groups[i].transform.position;
      }

    }

    void OnDestroy() {
      if (cull != null) {
        cull.Dispose();
      }
    }

    void OnDrawGizmos() {
      if (cull == null) return;

      Gizmos.color = Color.grey;
      Matrix4x4 mat = Gizmos.matrix;

      Gizmos.matrix = Camera.main.transform.localToWorldMatrix;
      for (int i = 0; i < distances.Length + 1; i++) {
        float far = (i == distances.Length) ? Camera.main.farClipPlane : distances[i];
        float near = (i == 0) ? Camera.main.nearClipPlane : distances[i - 1];

        Gizmos.DrawFrustum(
          Vector3.forward * near,
          Camera.main.fieldOfView,
          far,
          near,
          Camera.main.aspect
        );
      }

      Gizmos.matrix = mat;
    }

    public void SetActive(bool isActive) {
      foreach (AsyncLodGroup group in groups) {
        group.SetActive(isActive);
      }
    }

  }
}
