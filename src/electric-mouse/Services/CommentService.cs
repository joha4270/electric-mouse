using electric_mouse.Data;
using electric_mouse.Models;
using electric_mouse.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace electric_mouse.Services
{

    public class CommentService : ICommentService
    {

        ApplicationDbContext _dbContext;
        public CommentService(ApplicationDbContext context)
        {
            _dbContext = context;
        }

        public void AddComment(ApplicationUser user, int routeID, int originalCommentID, string commentContent)
        {
            _dbContext.Comments.Add(new Comment
            {
                RouteID = routeID,
                OriginalPostID = originalCommentID,
                User = user,
                Date = DateTime.Now,
                Content = commentContent
            });
            _dbContext.SaveChanges();
        }

        public void DeleteComment(int commentID)
        {
            Comment comment = _dbContext.Comments.First(c => c.CommentID == commentID);
            comment.Deleted = true;
            comment.DeletionDate = DateTime.Now;
            _dbContext.SaveChanges();
        }

        public List<Comment> GetAllComments() => _dbContext.Comments.ToList();
    }
}
