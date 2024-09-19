using homes_API.Models;

namespace homes_API.Repositories;

public interface IPostRepository
{
    // IEnumerable<Post> GetAllPosts();
    IEnumerable<object> GetAllPosts();
    // IEnumerable<object> GetVisiblePosts();
    IEnumerable<object> GetUserPosts(int userId);
    Task<IEnumerable<Post>> Search(string name);
    // Task<IEnumerable> GetActivePosts(int archive);
    Post GetPostById(int postID);
    // IEnumerable<object>? GetPostById(int postId);
    Post CreatePost(Post newPost);
    Post? UpdatePost(Post newPost);
    void DeletePostById(int postId);

}