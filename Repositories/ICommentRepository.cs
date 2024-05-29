using blog_API.Models;

namespace blog_API.Repositories;

public interface ICommentRepository
{
    IEnumerable<object> GetAllComments();
    IEnumerable<object> GetPostComments(int PostId);
    Comment? GetCommentById(int ComId);
    Comment CreateComment(Comment newComment);
    Comment? UpdateComment(Comment newComment);
    void DeleteCommentById(int ComId);

}