namespace WebApplication1.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message)
        {
        }  
    }
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        { 
        }  
    } 
    public class AccessDeniedException : Exception
    {
        public AccessDeniedException(string message) : base(message)
        { 
        }  
    }

}
