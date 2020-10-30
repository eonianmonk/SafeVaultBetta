using System;
using SafeVaultBetta.Types;

namespace SafeVaultBetta.Events
{
    public class UserCredAddedEvent : EventArgs
    {
        public UserCredAddedEvent(UserCred cred)
        {
            this.Creds = cred;
        }

        public UserCred Creds {get;set;}
    }
}
