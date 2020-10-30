using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Newtonsoft.Json;

namespace SafeVaultBetta.Types
{
    public class UserCred
    {
        public const int GettingOldTime = 5184000; // 60 days

        public string CredReference { get; set; }
        public string Password { get; set; }
        public int SetTimeStamp { get; set; }

        [JsonIgnore]
        public int Id { get; set; }

        [JsonIgnore]
        public bool IsHidden { get; set; }
        [JsonIgnore]
        public Visibility UNameVisibility
        {
            get
            {
                if (IsHidden) return Visibility.Hidden;
                else return Visibility.Visible;
            }
            private set { }
        }

        [JsonIgnore]
        public int CurrentTimeStamp { get; set; }
        [JsonIgnore]
        public Visibility IsOutdated
        {
            get
            {
                if (CurrentTimeStamp - SetTimeStamp < GettingOldTime) return Visibility.Visible;
                else return Visibility.Hidden;
            }
            private set { }
        }
    }
}
