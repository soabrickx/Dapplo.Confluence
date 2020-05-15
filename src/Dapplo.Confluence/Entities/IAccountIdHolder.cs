namespace Dapplo.Confluence.Entities
{
    /// <summary>
    /// Interface which allows us to have an account ID to work with.
    /// </summary>
    public interface IAccountIdHolder
    {
        /// <summary>
        /// The account ID of the user, which uniquely identifies the user across all Atlassian products.
        /// </summary>
        string AccountId { get; }
    }
}
