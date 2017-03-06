using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AsyncManagement {
  public class AsyncLodGroup : MonoBehaviour {
    private AsyncLodFilter[] filters;

    void Start() {
      filters = GetComponentsInChildren<AsyncLodFilter>();
    }

    public void SetLevel(int level) {
      foreach (AsyncLodFilter filter in filters) {
        filter.SetLevel(level);
      }
    }

    public void SetActive(bool isActive) {
      foreach (AsyncLodFilter filter in filters) {
        filter.SetActive(isActive);
      }
    }

  }
}
