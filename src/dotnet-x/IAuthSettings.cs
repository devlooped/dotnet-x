namespace Devlooped
{
    public interface IAuthSettings
    {
        string AccessToken { get; set; }
        string AccessTokenSecret { get; set; }
        string ConsumerKey { get; set; }
        string ConsumerSecret { get; set; }
    }
}