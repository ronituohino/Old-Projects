using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class PlatformDetection : Singleton<PlatformDetection>
{
    public bool onMobile;
    public bool inEditor = false;

    void Awake(){
        if(Application.isMobilePlatform){
            onMobile = true;
        } else {
            onMobile = false;
        }

#if UNITY_EDITOR
        inEditor = true;
#endif
    }

#if UNITY_EDITOR
    void Update(){
        if (EditorApplication.isRemoteConnected) {
            onMobile = true;
        }
    }
#endif
}