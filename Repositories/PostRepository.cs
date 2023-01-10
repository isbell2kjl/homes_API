using blog_API.Migrations;
using blog_API.Models;
using Microsoft.EntityFrameworkCore;

namespace blog_API.Repositories;

public class PostRepository : IPostRepository
{
    private readonly PostDbContext _context;

    public PostRepository(PostDbContext context)
    {
        _context = context;
    }

    public Post CreatePost(Post newPost)
    {
        _context.Posts!.Add(newPost);
        _context.SaveChanges();
        return newPost;
    }

    public void DeletePostById(int postId)
    {
        var post = _context.Posts!.Find(postId);
        if (post != null)
        {
            _context.Posts.Remove(post);
            _context.SaveChanges();
        }
    }

    //return all posts from all users.
    public IEnumerable<object> GetAllPosts()
    {
        return _context.Posts!
        .Include(p => p.User).
            Select(p => new
            {
                p.PostId,
                p.Content,
                p.Posted,
                p.UserId_fk,
                p.User!.UserName
            })
                .ToList();
    }

    //Return only the posts by a specific user based on the UserId_fk property of the Post model.
    public IEnumerable<object> GetUserPosts(int userId)
    {
        return _context.Posts!
        .Include(user => user.User).
            Select(p => new
            {
                p.PostId,
                p.Content,
                p.Posted,
                p.UserId_fk,
                p.User!.UserName
            })
            .Where(post => post.UserId_fk == userId)
                .ToList();
    }

    public Post? GetPostById(int postId)
    {
        return _context.Posts!.SingleOrDefault(p => p.PostId == postId);
    }

    public Post? UpdatePost(Post newPost)
    {
        var originalPost = _context.Posts!.Find(newPost.PostId);
        if (originalPost != null)
        {
            // originalPost.Title = newPost.Title;
            originalPost.Content = newPost.Content;
            // originalPost.Posted = newPost.Posted;
            _context.SaveChanges();
        }
        return originalPost;
    }
}