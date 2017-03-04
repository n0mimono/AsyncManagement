using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System;
using UnityEngine;

namespace AsyncManagement {
  public class AsyncUpdateManager : MonoBehaviour {
    public float fps { set; get; }

    private bool threadAlive;
    private Thread thread;

    public delegate void UpdateHandler(State state, bool isAsync);

    private class Task {
      public int id;
      public Action preUpdate;
      public UpdateHandler asyncUpdate;
      public Action postUpdate;
      public Func<bool> useAsync;
      public Func<bool> active;

    }
    private List<Task> taskList;

    private Func<bool> isTrue;
    private Func<bool> isFalse;
    private Action noop;

    [Serializable]
    public struct State {
      public int frameElapsed;
      public int elapsed;
      public float scale;
      public float deltaTime;
      public int count;
    }
    private State stateSync;
    private State stateAsync;

    public bool IsReady { private set; get; }

    void Awake() {
      fps = 60f;

      threadAlive = true;
      taskList = new List<Task>();

      isTrue = () => true;
      isFalse = () => false;
      noop = () => { };

      Stopwatch sw = new Stopwatch();
      thread = new Thread(() => {

        while (threadAlive) {
          int maxSleepMSec = (int)((1f / fps) * 1000f);
          if (!IsReady) {
            Thread.Sleep(maxSleepMSec);
            continue;
          }

          sw.Start();

          stateAsync.deltaTime = stateAsync.scale / fps;
          foreach (Task task in taskList) {
            if (!task.active()) continue;

            if (task.useAsync()) {
              task.asyncUpdate(stateAsync, true);
            }
          }

          sw.Stop();
          stateAsync.frameElapsed = sw.Elapsed.Milliseconds;
          sw.Reset();

          stateAsync.elapsed += stateAsync.frameElapsed;
          int sleepTime = Mathf.Max(0, maxSleepMSec - stateAsync.frameElapsed);

          Thread.Sleep(sleepTime);
          stateAsync.elapsed += sleepTime;
          stateAsync.count++;
        }
      });

      thread.IsBackground = true;
      thread.Start();
    }

    void Update() {
      stateAsync.scale = Time.timeScale;

      stateSync.scale = Time.timeScale;
      stateSync.deltaTime = Time.deltaTime;

      foreach (Task task in taskList) {
        if (!task.active()) continue;

        task.preUpdate();
        if (!task.useAsync()) {
          task.asyncUpdate(stateSync, false);
        }
        task.postUpdate();
      }

      stateSync.count++;
    }

    void OnDestroy() {
      threadAlive = false;
    }

    public int AddTask(Action<State> action) {
      int id = taskList.Count;

      taskList.Add(new Task() {
        id = id,
        preUpdate = noop,
        asyncUpdate = (state, isAsync) => action(state),
        postUpdate = noop,
        useAsync = isTrue,
        active = isTrue
      });

      return id;
    }

    public int AddTask(
      Action preUpdate,
      UpdateHandler asyncUpdate,
      Action postUpdate,
      Func<bool> useAsync,
      Func<bool> active
    ) {
      int id = taskList.Count;

      taskList.Add(new Task() {
        id = id,
        preUpdate = preUpdate,
        asyncUpdate = asyncUpdate,
        postUpdate = postUpdate,
        useAsync = useAsync,
        active = active
      });

      return id;
    }

    public void RemoveTask(int id) {
      taskList[id].active = isFalse;
    }


    public void StartUpdate() {
      IsReady = true;
    }

    public void StopUpdate() {
      IsReady = false;
    }

  }

}
