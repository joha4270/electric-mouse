using electric_mouse.Models;
using System.Collections.Generic;

namespace electric_mouse.Services.Interfaces
{
    public interface ICommentService
    {
        void AddComment(ApplicationUser user, int routeID, int originalCommentID, string commentContent);
        void DeleteComment(int commentID);
        List<Comment> GetAllComments();
    }
}