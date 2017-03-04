using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AsyncManagement;

public class Demo1Object : MonoBehaviour {
  [SerializeField] Mesh[] meshes;
  private Quaternion dRot;
	private Quaternion rot;

	void Start() {
    rot = Quaternion.identity;

    AsyncLodFilter filter = GetComponent<AsyncLodFilter>();

    filter.OnAsyncUpdate += (dt) => {
      dRot = Quaternion.AngleAxis(30f * dt, Vector3.forward);
      rot *= dRot;
    };
    filter.OnPostUpdate += (dt) => {
      transform.rotation = rot;
    };
    filter.OnLevelChanged += (level) => {
      MeshFilter mf = GetComponent<MeshFilter>();
      mf.mesh = meshes[level];
    };

  }

}
