using System;
using LiteDB;

namespace HsH2Brain.Shared.Models
{
    public class SettingsModel
    {
        [BsonId] public Guid Id { get; set; }
        public bool DarkMode { get; set; }
    }
}
