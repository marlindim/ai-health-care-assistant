


namespace Infrastructure.AIqueryResponse
{
    public static class CheckResponse
    {
        public record SymptomCheckRequest(string Symptoms, Guid? PatientId = null);

        public record SymptomCheckResponse(
            IEnumerable<string> PossibleConditions,
            string Advice,
            double Confidence
        );
    }
}

#region Old Code
//namespace Infrastructure.AIqueryResponse
//{
//    public class Promt
//    {
//        public record SymptomCheckRequest(string Symptoms, Guid? PatientId = null);
//        public record SymptomCheckResponse(IEnumerable<string> PossibleConditions, string Advice, double Confidence);
//    }
//}
#endregion