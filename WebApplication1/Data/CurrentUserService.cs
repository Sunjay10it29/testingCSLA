using System;
using System.Security.Claims;

namespace WebApplication1.Data
{
  public class CurrentUserService
  {
    private ClaimsPrincipal _currentUser;

    public event EventHandler<CurrentUserChangedEventArgs> CurrentUserChanged;

    public CurrentUserService()
    {
      CurrentUser = new ClaimsPrincipal(new ClaimsIdentity());
    }

    public ClaimsPrincipal CurrentUser
    {
      get
      {
        return _currentUser;
      }
      set
      {
        _currentUser = value;
        CurrentUserChanged?.Invoke(this, new CurrentUserChangedEventArgs() { NewUser = value });
      }
    }

    public class CurrentUserChangedEventArgs : EventArgs
    {
      public ClaimsPrincipal NewUser { get; set; }
    }
  }
}
