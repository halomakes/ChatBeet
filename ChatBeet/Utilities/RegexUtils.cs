namespace ChatBeet.Utilities
{
    public static class RegexUtils
    {
        public static string Nick = @"[A-z_\-\[\]\\^{}|`][A-z0-9_\-\[\]\\^{}|`]+";

        public static string Uri = @"(?:http:\/\/|https:\/\/)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)";
    }
}
