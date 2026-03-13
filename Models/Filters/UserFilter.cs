namespace QLDEducationalWeatherDataAPI.Models.Filters
{
    /// <summary>
    /// Represents a filter for querying user data.
    /// </summary>
    public class UserFilter
    {
        /// <summary>
        /// Gets or Sets the UserName match for filtering users.
        /// </summary>
        public string? UserNameMatch { get; set; }
        /// <summary>
        /// Gets or Sets the EmailAddress match for filtering users.
        /// </summary>
        public string? EmailAddressMatch { get; set; }
        /// <summary>
        /// Gets or Sets the Role match for filtering users.
        /// </summary>
        public string? RoleMatch { get; set; }
        /// <summary>
        /// Gets or Sets the Date Before which users were created. 
        /// </summary>
        public DateTime? CreatedBefore { get; set; }
        /// <summary>
        /// Gets or Sets the Date After which users were created.
        /// </summary>
        public DateTime? CreatedAfter { get; set; }
    }
}
