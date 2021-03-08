using Domain.Entities;
using System;

namespace Services.VisibleValues
{
    public class VisibleCommentValues : VisibleObjectValues
    {
        public string Username { get; set; }
        public string Text { get; set; }

        public static explicit operator VisibleCommentValues(Comment comment)
            => new VisibleCommentValues
            {
                Id = comment.Id,
                Username = comment.Author?.Username,
                Text = comment.Text
            };

        public override int Rate(string propertyName) => propertyName switch
        {
            nameof(Username) => 3,
            nameof(Text) => 2,
            _ => throw new NotImplementedException()
        };
    }
}
