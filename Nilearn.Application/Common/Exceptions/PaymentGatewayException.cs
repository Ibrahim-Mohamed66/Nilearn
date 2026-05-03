namespace Nilearn.Application.Common.Exceptions
{
    // PaymentGatewayException.cs
    public sealed class PaymentGatewayException : Exception
    {
        public int? StatusCode { get; }

        public PaymentGatewayException(string message, int? statusCode = null)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public PaymentGatewayException(string message, Exception inner)
            : base(message, inner) { }
    }
}
