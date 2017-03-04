using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AsyncManagement;

public class Demo1Object : MonoBehaviour {
  [SerializeField] Mesh[] meshes;
  private Quaternion rot;

  void Start() {
    AsyncLodFilter filter = GetComponent<AsyncLodFilter>();

    filter.OnAsyncUpdate += (dt) => {
      rot = Quaternion.AngleAxis(30f * dt, Vector3.forward);
    };
    filter.OnPostUpdate += (dt) => {
      transform.rotation *= rot;
    };
    filter.OnLevelChanged += (level) => {
      MeshFilter mf = GetComponent<MeshFilter>();
      mf.mesh = meshes[level];
    };

  }

}
