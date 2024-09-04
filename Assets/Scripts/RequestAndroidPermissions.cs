using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif


public class RequestAndroidPermissions : MonoBehaviour
{

    void Start()
    {
#if PLATFORM_ANDROID

        StartCoroutine(RequestPermissions());


    }
#endif

    private IEnumerator RequestPermissions()
    {
#if PLATFORM_ANDROID
        List<bool> requiredPermissions = new List<bool> { false, false, false };
        List<bool> requestedPermissions = new List<bool> { false, false, false };

        List<Action> actions = new List<Action>
        {
            new Action(() =>
            {
                requiredPermissions[0] = Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite);
                if(!requiredPermissions[0] && !requestedPermissions[0])
                {
                    Permission.RequestUserPermission(Permission.ExternalStorageWrite);
                    requestedPermissions[0] = true;
                    return;
                }
            }),
            new Action(() =>
            {
                requiredPermissions[1] = Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead);
                if(!requiredPermissions[1] && !requestedPermissions[1])
                {
                    Permission.RequestUserPermission(Permission.ExternalStorageRead);
                    requestedPermissions[1] = true;
                    return;
                }
            }),
            new Action(() =>
            {
                requiredPermissions[2] = Permission.HasUserAuthorizedPermission("android.settings.MANAGE_APP_ALL_FILES_ACCESS_PERMISSION");
                if(!requiredPermissions[2] && !requestedPermissions[2])
                {
                    Permission.RequestUserPermission("android.settings.MANAGE_APP_ALL_FILES_ACCESS_PERMISSION");
                    requestedPermissions[2] = true;
                    return;
                }
            })
        };

        for (int i = 0; i < requiredPermissions.Count; i++)
        {
            actions[i].Invoke();

            yield return new WaitUntil(() => requiredPermissions[i] == true);
        }
#endif
    }
}