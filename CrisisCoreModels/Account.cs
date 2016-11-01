namespace CrisisCoreModels
{
    /// <summary>
    ///  This describes a user account for logging into the system and access permissions.
    /// </summary>
    public class Account
    {
        public string AccountId { get; set; }
        public string Password { get; set; }
        public int AccessLevel { get; set; }
    }
}