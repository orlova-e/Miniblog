using Domain.Entities;
using Domain.Entities.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.Options
{
    public class Verifiable
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Value { get; set; }
        public string Author { get; set; }
        public string Avatar { get; set; }
        public string Link { get; set; }
        public string Matches { get; set; }

        public static implicit operator Verifiable(Entity entity)
        {
            Verifiable verifiable = entity switch
            {
                User user => new Verifiable
                {
                    Id = user.Id,
                    Author = user.Username,
                    Value = user.Description,
                    Avatar = user.Avatar switch
                    {
                        byte[] avatar => Convert.ToBase64String(avatar),
                        null => null
                    },
                    Link = "users/account/" + user.Username,
                    Matches = user.VerifiedMatches
                },
                Article article => new Verifiable
                {
                    Id = article.Id,
                    Author = article.User?.Username,
                    Value = article.Header,
                    Matches = article.VerifiedMatches,
                    Link = "articles/article/title?" + article.Link
                },
                Comment comment => new Verifiable
                {
                    Id = comment.Id,
                    Author = comment.Author?.Username,
                    Value = comment.Text,
                    Avatar = comment.Author?.Avatar switch
                    {
                        byte[] avatar => Convert.ToBase64String(avatar),
                        null => null
                    },
                    Matches = comment.VerifiedMatches,
                    Link = "articles/article/title?" + comment.Article.Link
                },
                Topic topic => new Verifiable
                {
                    Id = topic.Id,
                    Value = topic.Name,
                    Matches = topic.VerifiedMatches
                },
                Tag tag => new Verifiable
                {
                    Id = tag.Id,
                    Value = tag.Name,
                    Matches = tag.VerifiedMatches
                },
                Series series => new Verifiable
                {
                    Id = series.Id,
                    Value = series.Name,
                    Author = series.User?.Username,
                    Link = series.Link,
                    Matches = series.VerifiedMatches
                },
                { } => throw new NotImplementedException(),
                null => null
            };
            return verifiable;
        }
    }

    public class VerifiableUser : Verifiable
    {
        public string Role { get; set; }
        public static implicit operator VerifiableUser(User user)
        {
            Verifiable verifiable = user;
            VerifiableUser verifiableUser = new VerifiableUser
            {
                Id = verifiable.Id,
                Author = verifiable.Author,
                Value = verifiable.Value,
                Avatar = verifiable.Avatar,
                Link = verifiable.Link,
                Matches = verifiable.Matches,
                Role = Enum.GetName(typeof(RoleType), user.Role.Type)
            };
            return verifiableUser;
        }
    }
}
