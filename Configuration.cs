namespace Blog;

public static class Configuration
{
    //token  - JWT - Json Web Token
    public static string JWTKey  = "3Ap41dnZ4kOyN7jJvxVM0Q3Ap41dnZ4kOyN7jJvxVM0Q3Ap41dnZ4kOyN7jJvxVM0Q==";
    public static string ApiKeyName  = "api_key";
    public static string ApiKey  = "curso_api_aspnet06/diego_telis";
    public static SmtpConfiguration Smtp = new ();

    public class SmtpConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }


}
