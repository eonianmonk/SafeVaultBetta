using SafeVaultAlpha.Cryptography;
using SafeVaultBetta.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SafeVaultAlpha.Types
{
    public class User
    {
        public string Username { get; set; }
        public byte[] Password { get; set; }
        public int FileMinimalLength { get; set; }

        public User() { }

        public User(Stream fs, string username)
        {
            int offset = FileConsts.FileSign.Length;
            this.Username = username;
            fs.Read(this.Password, offset, StaticHash.SHA1HashLength);

            byte[] temp = null;
            fs.Read(temp, offset+StaticHash.SHA1HashLength, 4);
            this.FileMinimalLength = Convert.ToInt32(temp);
        }
    }
}
