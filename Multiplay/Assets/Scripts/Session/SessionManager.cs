using UnityEngine;
using Mali.Utils;
using Unity.Services.Core;
using Unity.Services.Authentication;

public class SessionManager : Singleton<SessionManager>
{
    
    async void Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
}
