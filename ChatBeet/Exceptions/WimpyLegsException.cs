namespace ChatBeet.Exceptions;

public class WimpyLegsException : Exception
{
    public DateTime NextJump { get; }

    public WimpyLegsException(DateTime nextJump)
    {
        NextJump = nextJump;
    }
}

public class StumbleException : Exception
{
}