using blog_API.Migrations;
using blog_API.Models;
using Microsoft.EntityFrameworkCore;

namespace blog_API.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly PostDbContext _context;

    public CommentRepository(PostDbContext context)
    {
        _context = context;
    }

    public Comment CreateComment(Comment newComment)
    {
        _context.Comments!.Add(newComment);
        _context.SaveChanges();
        return newComment;
    }

    public void DeleteCommentById(int comId)
    {
        var comment = _context.Comments!.Find(comId);
        if (comment != null)
        {
            _context.Comments.Remove(comment);
            _context.SaveChanges();
        }
    }

    //return all comments from all posts.
    public IEnumerable<object> GetAllComments()
    {
        return _context.Comments!
        .Include(c => c.Post).
            Select(c => new
            {
                c.ComId,
                c.Task,
                c.Text,
                c.ComDate,
                c.UsrId_fk,
                c.User!.UserName,
                c.PostId_fk
            })
                .ToList();
    }

    //Return only the comments by a specific post based on the PostId_fk property of the Comment model.
    public IEnumerable<object> GetPostComments(int postId)
    {
        return _context.Comments!
        .Include(post => post.Post).
            Select(c => new
            {
                c.ComId,
                c.Task,
                c.Text,
                c.ComDate,
                c.UsrId_fk,
                c.User!.UserName,
                c.PostId_fk
            })
            .Where(comment => comment.PostId_fk == postId)
                .ToList();
    }

    public Comment? GetCommentById(int comId)
    {
        return _context.Comments!.SingleOrDefault(c => c.ComId == comId);
    }

    public Comment? UpdateComment(Comment newComment)
    {
        var originalComment = _context.Comments!.Find(newComment.ComId);
        if (originalComment != null)
        {
            originalComment.Task = newComment.Task;
            originalComment.Text = newComment.Text;
            originalComment.ComDate = DateTime.Now;
            _context.SaveChanges();
        }
        return originalComment;
    }
}