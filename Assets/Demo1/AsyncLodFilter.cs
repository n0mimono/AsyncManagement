using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace AsyncManagement {
  public class AsyncLodFilter : MonoBehaviour {
    private int level = -1;

    public static int maxLevel = 3;
    public Func<int,int> splitFunc;

    private int count;
    private int split { get { return splitFunc(level); } }

    public int levelAsync = maxLevel;
    private bool useAsync;

    private bool isActive;

    public delegate void UpdateHandler(float dt);
    public event UpdateHandler OnPreUpdate;
    public event UpdateHandler OnAsyncUpdate;
    public event UpdateHandler OnPostUpdate;

    public delegate void LevelHandler(int level);
    public event LevelHandler OnLevelChanged;

    void Start() {
      if (splitFunc == null) {
        splitFunc = (level) => Mathf.Max(1, level + 1);
      }

      AsyncUpdateManager asyncUpdate = GameObject.FindObjectOfType<AsyncUpdateManager>();

      // shuffle
      count = (int)(UnityEngine.Random.value * 100 * (maxLevel + 1));

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

      SetActive(false);
    }

    public void SetLevel(int level) {
      level = Mathf.Clamp (level, 0, maxLevel);
      if (this.level == level) return;

      if (OnLevelChanged != null) {
        OnLevelChanged(level);
      }
    }

    public void SetActive(bool isActive) {
      this.isActive = isActive;
    }

    public AsyncLodFilter AddPreUpdate(UpdateHandler onUpdate, Func<bool> predicate) {
      OnPreUpdate += (dt) => {
        if (predicate()) {
          onUpdate(dt);
        }
      };
      return this;
    }

    public AsyncLodFilter AddAsyncUpdate(UpdateHandler onUpdate, Func<bool> predicate) {
      OnAsyncUpdate += (dt) => {
        if (predicate()) {
          onUpdate(dt);
        }
      };
      return this;
    }

    public AsyncLodFilter AddPostUpdate(UpdateHandler onUpdate, Func<bool> predicate) {
      OnPostUpdate += (dt) => {
        if (predicate()) {
          onUpdate(dt);
        }
      };
      return this;
    }

  }
}
