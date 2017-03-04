using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using AsyncManagement;

public class Demo0 : MonoBehaviour {

  [Serializable]
  public class Core {
    public bool isActive;
    public bool isAsync;
    public Text text;
  }
  public Core core0;
  public Core core1;
  public Core core2;

  void Start() {
    CountUpText(core0);
    CountUpText(core1);
    CountUpText(core2);

    GameObject.FindObjectOfType<AsyncUpdateManager>().StartUpdate();
  }

  private void CountUpText(Core core) {
    int count = 0;
    AsyncObject asyncObj = new AsyncObject();
    asyncObj.OnPreUpdate += () => {

    };
    asyncObj.OnAsyncUpdate += (state, isAsync) => {
      count++;
    };
    asyncObj.OnPostUpdate += () => {
      core.text.text = count.ToString();
    };
    asyncObj.SetActive(core.isActive);
    asyncObj.SetAsync(core.isAsync);
  }

}
