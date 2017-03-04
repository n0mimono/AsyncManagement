# AsyncManagement

Async-update demo.

# Demo 0: async-count-up

```c#
    int count = 0;
    AsyncObject asyncObj = new AsyncObject();
    asyncObj.OnPreUpdate += () => {

    };
    asyncObj.OnAsyncUpdate += (state, isAsync) => {
      count++;
    };
    asyncObj.OnPostUpdate += () => {
      text.text = count.ToString();
    };
    asyncObj.SetActive(isActive);
    asyncObj.SetAsync(isAsync);
```

# Demo 1: async-lod-handling

```c#
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
```
