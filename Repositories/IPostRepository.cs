using blog_API.Models;

namespace blog_API.Repositories;

public interface IPostRepository
{
    // IEnumerable<Post> GetAllPosts();
    IEnumerable<object> GetAllPosts();
    IEnumerable<object> GetUserPosts(int userId);
    Post? GetPostById(int postId);
    Post CreatePost(Post newPost);
    Post? UpdatePost(Post newPost);
    void DeletePostById(int postId);

}