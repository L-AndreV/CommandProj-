namespace Contracts
{
    public class AuthRequest : BaseContract
    {
        public enum OperationType
        {
            Register,
            Login
        }
        public AuthRequest() => RequestType = nameof(AuthRequest);
        public OperationType Operation { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
    }
}
