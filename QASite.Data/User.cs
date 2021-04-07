using System.Collections.Generic;

namespace QASite.Data
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public List<Question> Questions { get; set; }
        public List<Answer> Answers { get; set; }
        public List<QuestionLike> LikedQuestions { get; set; }
    }
}