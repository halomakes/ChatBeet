using Microsoft.ML.Data;

namespace SampleClassification.Model
{
    public class SentimentInput
    {
        [ColumnName("col0"), LoadColumn(0)]
        public string Message { get; set; }


        [ColumnName("col1"), LoadColumn(1)]
        public string Col1 { get; set; }
    }
}
