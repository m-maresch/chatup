namespace Common
{
    public class UserSignInResponseDto
    {
        public bool Valid { get; set; }
        public int UserID { get; set; }
        public string UriAuthorizationServer { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
    }
}