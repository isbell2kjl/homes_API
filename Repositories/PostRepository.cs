using homes_API.Migrations;
using homes_API.Models;
using Microsoft.EntityFrameworkCore;

namespace homes_API.Repositories;

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
                p.PhotoURL,
                p.Title,
                p.Visible,
                p.Archive,
                p.UserId_fk,
                p.User!.UserName,
                p.User!.ProjId_fk,
                p.Comments
            })
                .ToList();
    }


    //Return only the posts by a specific project based on the ProjId_fk property of the User model.
    public IEnumerable<object> GetProjectPosts(int projectId)
    {
        var posts = _context.Posts!
            .Include(post => post.User)
            .Where(post => post.User!.ProjId_fk == projectId)
            .Select(post => new
            {
                post.PostId,
                post.Content,
                post.Posted,
                post.PhotoURL,
                post.Title,
                post.Visible,
                post.Archive,
                post.UserId_fk,
                post.User!.UserName,
                post.User.ProjId_fk,
                post.Comments
            })
            .ToList();

        return posts;
        
    }

    public Post GetPostById(int postId)
    {
        return _context.Posts!.FirstOrDefault(p => p.PostId == postId)!;
        
    }

    //search idea from https://www.pragimtech.com/blog/blazor/search-in-asp.net-core-rest-api/
    public async Task<IEnumerable<Post>> Search(string name)
    {
        IQueryable<Post> query = _context.Posts!;

        if (!string.IsNullOrEmpty(name))
        {
            query = query!.Where(p => p.Title!.Contains(name)
                        || p.Content!.Contains(name));
        }

        return await query.ToListAsync();

    }

    public Post? UpdatePost(Post newPost)
    {
        var originalPost = _context.Posts!.Find(newPost.PostId);
        if (originalPost != null)
        {
            originalPost.Title = newPost.Title;
            originalPost.Content = newPost.Content;
            originalPost.PhotoURL = newPost.PhotoURL;
            originalPost.Visible = newPost.Visible;
            originalPost.Archive = newPost.Archive;
            originalPost.Posted = DateTime.Now;
            _context.SaveChanges();
        }
        return originalPost;
    }
}