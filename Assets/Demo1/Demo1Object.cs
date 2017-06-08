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
    filter.Initialize ();
    filter.levelAsync = 2;
    filter.splitFunc = (level) => Mathf.Max(1, level + 1);

    filter.AddAsyncUpdate (dt => {
      dRot = Quaternion.AngleAxis(30f * dt, Vector3.forward);
      rot *= dRot;
    }, () => true).AddPostUpdate (dt => {
      transform.rotation = rot;
    }, () => true);

    filter.OnLevelChanged += (level) => {
      MeshFilter mf = GetComponent<MeshFilter>();
      mf.mesh = meshes[level];
    };

  }

}
