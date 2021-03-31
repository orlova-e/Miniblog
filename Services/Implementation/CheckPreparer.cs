using Domain.Entities;
using Domain.Entities.Enums;
using Repo.Interfaces;
using Services.FoundValues;
using Services.Implementation.Search;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Implementation
{
    public class CheckPreparer : ICheckPreparer
    {
        private readonly string queueList;
        private readonly ExtendedRole role;
        private readonly User user;
        private IRepository Repository { get; }
        private IUserService UserService { get; }
        private IArticleService ArticleService { get; }
        private ICommentsService CommentsService { get; }

        public CheckPreparer(CheckPreparerBuilder builder,
            User user,
            string queueList)
        {
            Repository = builder.Repository;
            UserService = builder.UserService;
            ArticleService = builder.ArticleService;
            CommentsService = builder.CommentsService;
            role = user.Role as ExtendedRole;
            this.user = user;
            this.queueList = queueList;
        }

        public async Task AcceptAsync(Guid id)
        {
            if (!HasAccess())
                return;
            Entity entity = await Repository.Entities.GetByIdAsync(id).ConfigureAwait(false);
            entity.Accepted = true;
            entity.VerifiedMatches = string.Empty;
            await Repository.Entities.UpdateAsync(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            if (!HasAccess())
                return;

            Task deleteTask = (queueList switch
            {
                "articles" or "pages" => ArticleService.DeleteArticleAsync(id),
                "users" => UserService.DeleteUserAsync(id),
                "comments" => CommentsService.DeleteCommentAsync(role, id),
                "tags" => Repository.Tags.DeleteAsync(id),
                "topics" => Repository.Topics.DeleteAsync(id),
                "series" => Repository.Series.DeleteAsync(id),
                _ => throw new NotImplementedException()
            });

            await deleteTask;
        }

        public async Task ChangeRole(Guid userId, RoleType type)
        {
            if (!HasAccess())
                return;

            User user = await Repository.Users.GetByIdAsync(userId);
            Role role = Repository.Roles.Find(r => r.Type == type).FirstOrDefault();
            user.Role = role is not null ? role : user.Role;
            await Repository.Users.UpdateAsync(user);
        }

        public IEnumerable<Entity> EnumerationOf(Func<Entity, bool> predicate)
        {
            IEnumerable<Entity> entities = queueList switch
            {
                "articles" => Repository.Articles.Find(predicate)?
                    .Where(a => a.EntryType is EntryType.Article),
                "pages" => Repository.Articles.Find(predicate)?
                    .Where(p => p.EntryType is EntryType.Page),
                "users" => Repository.Users.Find(predicate),
                "comments" => Repository.Comments.Find(predicate),
                "tags" => Repository.Tags.Find(predicate),
                "topics" => Repository.Topics.Find(predicate),
                "series" => Repository.Series.Find(predicate),
                _ => throw new NotImplementedException()
            };
            entities = Prepare(entities);
            return entities;
        }

        public async Task<IEnumerable<Entity>> SearchAsync(string query)
        {
            if (!HasAccess())
                throw new InvalidOperationException();

            bool accurateSearch = true;
            IEnumerable<FoundObject> found = await (queueList switch
            {
                "articles" or "pages" => new AggregateSearch<Article>(Repository).FindAsync(query),
                "users" => new AggregateSearch<User>(Repository).FindAsync(query),
                "comments" => new AggregateSearch<Comment>(Repository).FindAsync(query),
                "tags" => new AggregateSearch<Tag>(Repository).FindAsync(query, accurateSearch),
                "topics" => new AggregateSearch<Topic>(Repository).FindAsync(query, accurateSearch),
                "series" => new AggregateSearch<Series>(Repository).FindAsync(query, accurateSearch),
                { } => throw new NotImplementedException(),
                _ => null
            }).ConfigureAwait(false);

            if (queueList is "articles")
            {
                found = found.Where(f => f.Entity is Article article && article.EntryType == EntryType.Article);
            }
            else if (queueList is "pages")
            {
                found = found.Where(f => f.Entity is Article article && article.EntryType == EntryType.Page);
            }

            IEnumerable<Entity> entities = found.Select(f => f.Entity);
            entities = Prepare(entities);

            return entities;
        }

        private IEnumerable<Entity> Prepare(IEnumerable<Entity> entities)
        {
            if (queueList is "users" && role.Type is not RoleType.Administrator)
            {
                entities = entities?.Where(e => (e as User).Role.Type != RoleType.Administrator);

                var preparedEntities = entities?.ToList();
                int? index = preparedEntities?.FindIndex(e => e.Id == user.Id);
                if (index is not null && index is not -1)
                    preparedEntities.RemoveAt((int)index);
                entities = preparedEntities;
            }

            return entities;
        }

        public Entity GetNext(Guid id, Guid[] existElementsIds)
        {
            existElementsIds = existElementsIds.Append(id).ToArray();

            if (role.Type is not RoleType.Administrator)
                existElementsIds = existElementsIds.Append(user.Id).ToArray();

            bool predicate(Entity e) => !existElementsIds.Contains(e.Id) && e.Accepted != true;
            IEnumerable<Entity> entities = EnumerationOf(predicate);
            entities = Prepare(entities);

            return entities?.FirstOrDefault();
        }

        public bool HasAccess(string queueList = null)
        {
            queueList = queueList is null ? this.queueList : queueList;

            if (role is null)
                return false;

            bool hasAccess = queueList switch
            {
                "articles" => role.CheckArticles,
                "pages" => role.Type is RoleType.Administrator,
                "topics" => role.CheckTopics,
                "tags" => role.CheckTags,
                "comments" => role.CheckComments,
                _ => true,
            };

            return hasAccess;
        }
    }
}
