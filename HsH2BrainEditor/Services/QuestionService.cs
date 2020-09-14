using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using HsH2Brain.Shared.Models;

namespace HsH2BrainEditor.Services
{
    public class QuestionService
    {
        private Guid UserGuid { get; }

        public QuestionService(Guid userGuid)
        {
            UserGuid = userGuid;
        }

        public List<QuestionSetModel> Load()
        {
            if (UserGuid == Guid.Empty) return null;

            using var db = new LiteDatabase("hsh2go.db");
            var col = db.GetCollection<QuestionSetModel>();
            return col.Find(c => c.AuthorId == UserGuid).ToList();
        }

        public QuestionSetModel Load(Guid quizId)
        {
            if (UserGuid == Guid.Empty || quizId == Guid.Empty) return null;

            using var db = new LiteDatabase("hsh2go.db");
            var col = db.GetCollection<QuestionSetModel>();
            return col.Find(c => c.Id == quizId && c.AuthorId == UserGuid).FirstOrDefault();
        }

        public Guid Insert(string title)
        {
            using var db = new LiteDatabase("hsh2go.db");
            var newGuid = Guid.NewGuid();
            var col = db.GetCollection<QuestionSetModel>();
            col.Insert(new QuestionSetModel
            {
                Id = newGuid,
                AuthorId = UserGuid,
                Title = title,
                Questions = new List<QuestionModel>()
            });

            return newGuid;
        }

        public void InsertQuestion(Guid quizId, QuestionModel newQuestion)
        {
            using var db = new LiteDatabase("hsh2go.db");
            var col = db.GetCollection<QuestionSetModel>();
            var entry = col.Find(c => c.Id == quizId && c.AuthorId == UserGuid).ToList().FirstOrDefault();
            if (entry == null) return;

            newQuestion.Id = Guid.NewGuid();
            newQuestion.QuestionType = EQuestionType.SimpleText;
            entry.Questions.Add(newQuestion);
            col.Update(entry);
        }

        public void UpdateTitle(Guid quizId, string title)
        {
            using var db = new LiteDatabase("hsh2go.db");
            var col = db.GetCollection<QuestionSetModel>();
            var entry = col.Find(c => c.Id == quizId && c.AuthorId == UserGuid).FirstOrDefault();
            if (entry == null) return;

            entry.Title = title;
            col.Update(entry);
        }

        public void Delete(Guid questionId, Guid quizId)
        {
            using var db = new LiteDatabase("hsh2go.db");
            var col = db.GetCollection<QuestionSetModel>();
            var entry = col.Find(c => c.Id == quizId && c.AuthorId == UserGuid).FirstOrDefault();
            if (entry == null) return;

            var todel = entry.Questions.SingleOrDefault(c => c.Id == questionId);
            if (todel != null)
                entry.Questions.Remove(todel);

            col.Update(entry);
        }
    }
}
