namespace ChatBeet.Models
{
    public class FunTranslation
    {
        public SuccessInfo Success { get; set; }
        public TranslationContent Contents { get; set; }
        public class SuccessInfo
        {
            public int Total { get; set; }
        }

        public class TranslationContent
        {
            public string Translated { get; set; }
            public string Text { get; set; }
            public string Translation { get; set; }
        }
    }


}
