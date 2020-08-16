using HsH2Brain.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HsH2BrainEditor.Services
{
    public class QuestionService
    {
        public List<QuestionSetModel> Load(Guid userId)
        {
            if (userId == Guid.Empty) return null;

            using (var db = new LiteDatabase("hsh2go.db"))
            {
                var col = db.GetCollection<QuestionSetModel>();
                return col.Find(c => c.AuthorId == userId).ToList();
            }
        }

        public QuestionSetModel Load(Guid userId, Guid quizId)
        {
            if (userId == Guid.Empty || quizId == Guid.Empty) return null;

            using (var db = new LiteDatabase("hsh2go.db"))
            {
                var col = db.GetCollection<QuestionSetModel>();
                return col.FindOne(c => c.Id == quizId && c.AuthorId == userId);
            }
        }

        public Guid Insert(string title, Guid userId)
        {
            using (var db = new LiteDatabase("hsh2go.db"))
            {
                var newGuid = Guid.NewGuid();
                var col = db.GetCollection<QuestionSetModel>();
                col.Insert(new QuestionSetModel
                {
                    Id = newGuid,
                    AuthorId = userId,
                    Title = title,
                    Questions = new List<QuestionModel>()
                });

                return newGuid;
            }
        }
    }
}
