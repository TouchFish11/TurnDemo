using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// µÇÂ¼Êı¾İ
/// </summary>
[Serializable]
public struct LoginData
{
    // ÕËºÅ
    public string account;
    // ÃÜÂë
    public string password;

    public LoginData(string account, string password)
    {
        this.account = account;
        this.password = password;
    }
}
