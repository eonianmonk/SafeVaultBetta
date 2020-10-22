using System;
using SafeVaultAlpha.Types;

namespace SafeVaultAlpha.Events
{
    public class UserCreatedEventArgs : EventArgs
    {
        public UserCreatedEventArgs(User user)
        {
            this.User = user;
        }

        public User User { get; set; }        
    }
}
