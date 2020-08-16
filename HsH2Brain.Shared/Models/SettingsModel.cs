using LiteDB;
using System;

namespace HsH2Brain.Models
{
    public class SettingsModel
    {
        [BsonId] public Guid Id { get; set; }
        public bool DarkMode { get; set; }
    }
}
