using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace QASite.Data
{
    public class QuestionsRepository
    {
        private readonly string _connectionString;

        public QuestionsRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddQuestion(Question question, List<string> tags)
        {
            using var ctx = new QASiteContext(_connectionString);
            ctx.Questions.Add(question);
            ctx.SaveChanges();
            foreach (string tag in tags)
            {
                Tag t = GetTag(tag);
                int tagId;
                if (t == null)
                {
                    tagId = AddTag(tag);
                }
                else
                {
                    tagId = t.Id;
                }
                ctx.QuestionsTags.Add(new QuestionsTags
                {
                    QuestionId = question.Id,
                    TagId = tagId
                });
            }

            ctx.SaveChanges();
        }

        public Question Get(int id)
        {
            using var context = new QASiteContext(_connectionString);
            return context.Questions
                .Include(q => q.User)
                .ThenInclude(q => q.LikedQuestions)
                .Include(q => q.Answers)
                .ThenInclude(a => a.User)
                .Include(q => q.Likes)
                .Include(u => u.QuestionsTags)
                .ThenInclude(qt => qt.Tag)
                .FirstOrDefault(q => q.Id == id);
        }

        public List<Question> Get()
        {
            using var context = new QASiteContext(_connectionString);
            return context.Questions
                .Include(q => q.Answers)
                .Include(q => q.Likes)
                .Include(q => q.QuestionsTags)
                .ThenInclude(q => q.Tag)
                .OrderByDescending(q => q.Date)
                .ToList();
        }

        public void AddAnswer(Answer answer)
        {
            using var context = new QASiteContext(_connectionString);
            context.Answers.Add(answer);
            context.SaveChanges();
        }

        public void AddQuestionLike(int questionId, int userId)
        {
            using var context = new QASiteContext(_connectionString);
            var like = new QuestionLike
            {
                QuestionId = questionId,
                UserId = userId
            };
            context.QuestionLikes.Add(like);
            context.SaveChanges();
        }

        public int GetQuestionLikes(int questionId)
        {
            using var context = new QASiteContext(_connectionString);
            return context.QuestionLikes.Count(q => q.QuestionId == questionId);
        }

        private Tag GetTag(string name)
        {
            using var ctx = new QASiteContext(_connectionString);
            return ctx.Tags.FirstOrDefault(t => t.Name == name);
        }

        private int AddTag(string name)
        {
            using var ctx = new QASiteContext(_connectionString);
            var tag = new Tag { Name = name };
            ctx.Tags.Add(tag);
            ctx.SaveChanges();
            return tag.Id;
        }

    }
}
