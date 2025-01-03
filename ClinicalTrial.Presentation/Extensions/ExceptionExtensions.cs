namespace ClinicalTrial.Presentation.Extensions
{
    public static class ExceptionExtensions
    {
        public static object GetApiExceptionMessage(this Exception ex)
        {
            return new { ex.GetInnermostException().Message, StackTrace = ex.GetInnermostException().StackTrace?.ToString() };
        }

        private static Exception GetInnermostException(this Exception ex)
        {
            while (ex.InnerException != null) ex = ex.InnerException;
            return ex;
        }
    }
}
