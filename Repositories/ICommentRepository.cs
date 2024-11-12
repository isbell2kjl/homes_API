using homes_API.Models;

namespace homes_API.Repositories;

public interface ICommentRepository
{
    IEnumerable<object> GetAllComments();
    IEnumerable<object> GetPostComments(int PostId);
    Comment? GetCommentById(int ComId);
    Comment CreateComment(Comment newComment);
    Comment? UpdateComment(int comId, CommentUpdate editComment);
    void DeleteCommentById(int ComId);

} 