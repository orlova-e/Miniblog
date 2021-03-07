using Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ICommentsService
    {
        Task<Comment> AddCommentAsync(string username, string title, string text, Guid? parentId = null);
        Task<Comment> UpdateCommentAsync(string username, string text, Guid commentId);
        Task DeleteCommentAsync(string username, Guid commentId);
        Task DeleteCommentAsync(ExtendedRole extendedRole, Guid commentId);
        Task<bool> LikeComment(string username, Guid commentId);
    }
}
