using HsH2BrainEditor.Models;
using LiteDB;
using System;

namespace HsH2BrainEditor.Services
{
    public class UserService
    {
        public UserModel Login(string username, string password)
        {
            var sanitizedUsername = username.ToLower().Trim();
            using (var db = new LiteDatabase("hsh2go.db"))
            {
                var col = db.GetCollection<UserModel>("users");
                var potentialUser = col.FindOne(c => c.Username == sanitizedUsername);
                if (potentialUser == null) return null;

                if (CryptoHelper.Crypto.VerifyHashedPassword(potentialUser.Password, password))
                    return potentialUser;

                return null;
            }
        }

        public string SelectUser(string username)
        {
            return SelectUserObject(username)?.Username; 
        }

        public UserModel SelectUserObject(string username)
        {
            var sanitizedUsername = username.ToLower().Trim();
            using (var db = new LiteDatabase("hsh2go.db"))
            {
                var col = db.GetCollection<UserModel>("users");
                var potentialUser = col.FindOne(c => c.Username == sanitizedUsername);
                if (potentialUser == null) return null;

                return potentialUser;
            }
        }

        public bool Register(string username, string password)
        {
            var sanitizedUsername = username.ToLower().Trim();
            using (var db = new LiteDatabase("hsh2go.db"))
            {
                var col = db.GetCollection<UserModel>("users");
                var potentialUser = col.FindOne(c => c.Username == sanitizedUsername);
                if (potentialUser != null) return false;

                col.Insert(new UserModel
                {
                    Id = Guid.NewGuid(),
                    Username = sanitizedUsername,
                    Password = CryptoHelper.Crypto.HashPassword(password)
                });

                return true;
            }
        }
    }
}
