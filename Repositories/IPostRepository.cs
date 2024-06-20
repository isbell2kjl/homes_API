using blog_API.Models;

namespace blog_API.Repositories;

public interface IPostRepository
{
    // IEnumerable<Post> GetAllPosts();
    IEnumerable<object> GetAllPosts();
    IEnumerable<object> GetUserPosts(int userId);
    Task<IEnumerable<Post>> Search(string name);

    Post GetPostById(int postID);

    // IEnumerable<object>? GetPostById(int postId);
    Post CreatePost(Post newPost);
    Post? UpdatePost(Post newPost);
    void DeletePostById(int postId);

}