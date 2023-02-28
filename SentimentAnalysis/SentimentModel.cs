using System;
using System.IO;
using System.Reflection;
using Microsoft.ML;

namespace SentimentAnalysis
{
    public class SentimentModel
    {
        private static Lazy<PredictionEngine<SentimentInput, SentimentAnalysis>> _predictionEngine = new(CreatePredictionEngine);

        // For more info on consuming ML.NET models, visit https://aka.ms/mlnet-consume
        // Method for consuming model in your app
        public static SentimentAnalysis Predict(SentimentInput input)
        {
            SentimentAnalysis result = _predictionEngine.Value.Predict(input);
            return result;
        }

        public static PredictionEngine<SentimentInput, SentimentAnalysis> CreatePredictionEngine()
        {
            // Create new MLContext
            MLContext mlContext = new MLContext();

            // Load model & create prediction engine
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyDirectory = Path.GetDirectoryName(assembly.Location);
            var modelPath = Path.Combine(assemblyDirectory!, "MLModel.zip");
            var mlModel = mlContext.Model.Load(modelPath, out _);
            var predEngine = mlContext.Model.CreatePredictionEngine<SentimentInput, SentimentAnalysis>(mlModel);

            return predEngine;
        }
    }
}