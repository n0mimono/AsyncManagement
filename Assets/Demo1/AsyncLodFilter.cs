using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace AsyncManagement {
  public class AsyncLodFilter : MonoBehaviour {
    private int level = -1;

    public int split { get { return Mathf.Max(1, level + 1); } }
    private int count;

    public readonly int levelAsync = 2;
    private bool useAsync;

    private bool isActive;

    public delegate void UpdateHandler(float dt);
    public event UpdateHandler OnPreUpdate;
    public event UpdateHandler OnAsyncUpdate;
    public event UpdateHandler OnPostUpdate;

    public delegate void LevelHandler(int level);
    public event LevelHandler OnLevelChanged;

    void Start() {
      AsyncUpdateManager asyncUpdate = GameObject.FindObjectOfType<AsyncUpdateManager>();

      asyncUpdate.AddTask(
        () => {
          count++;
          if (count % split == 0) {
            if (OnPreUpdate != null) {
              OnPreUpdate(Time.deltaTime * split);
            }
          }
        },
        (state, isAsync) => {
          if (count % split == 0) {
            if (OnAsyncUpdate != null) {
              OnAsyncUpdate(state.deltaTime * split);
            }
          }
        },
        () => {
          if (count % split == 0) {
            if (OnPostUpdate != null) {
              OnPostUpdate(Time.deltaTime * split);
            }
          }
        },
        () => useAsync,
        () => isActive
      );

      OnLevelChanged += (level) => this.level = level;
      OnLevelChanged += (level) => useAsync = (level >= levelAsync);

      SetActive(gameObject.activeInHierarchy);
    }

    public void SetLevel(int level) {
      if (this.level == level) return;

      if (OnLevelChanged != null) {
        OnLevelChanged(level);
      }
    }

    public void SetActive(bool isActive) {
      this.isActive = isActive;
      gameObject.SetActive(isActive);
    }

  }
}
