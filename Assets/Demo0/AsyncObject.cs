using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace AsyncManagement {
  [Serializable]
  public class AsyncObject {
    private bool useAsync;
    private bool isActive;

    public event Action OnPreUpdate;
    public event AsyncUpdateManager.UpdateHandler OnAsyncUpdate;
    public event Action OnPostUpdate;

    public AsyncObject() {
      AsyncUpdateManager asyncUpdate = GameObject.FindObjectOfType<AsyncUpdateManager>();

      asyncUpdate.AddTask(
        () => {
          if (OnPreUpdate != null) {
            OnPreUpdate();
          }
        },
        (state, isAsync) => {
          if (OnAsyncUpdate != null) {
            OnAsyncUpdate(state, isAsync);
          }
        },
        () => {
          if (OnPostUpdate != null) {
            OnPostUpdate();
          }
        },
        () => useAsync,
        () => isActive
      );
    }

    public void SetActive(bool isActive) {
      this.isActive = isActive;
    }

    public void SetAsync(bool useAsync) {
      this.useAsync = useAsync;
    }

  }

}
