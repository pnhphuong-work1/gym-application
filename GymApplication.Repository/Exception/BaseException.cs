namespace GymApplication.Repository.Exception;

public class BaseException : System.Exception
{
    protected BaseException(string title, string message)
        : base(message) =>
        Title = title;
    public string Title { get; }
}