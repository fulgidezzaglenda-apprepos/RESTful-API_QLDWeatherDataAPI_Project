namespace QLDEducationalWeatherDataAPI.Models.UserDTOs
{
    /// <summary>
    /// Data transer object for the API users.
    /// </summary>
    public class UserDTO
    {
        /// <summary>
        /// Gets or Sets the User Name associated with the API Users.
        /// </summary>
        public string UserName { get; set; } = string.Empty;
        /// <summary>
        /// Gets or Sets the Email Address associated with the API Users.
        /// </summary>
        public string EmailAddress { get; set; } = string.Empty;
        /// <summary>
        /// Gets or Sets the User role associated with the API Users.
        /// </summary>
        public string Role { get; set; } = string.Empty;
        /// <summary>
        /// Gets or Sets the timestamp that represent the creation time of the API Users.
        /// </summary>
        public DateTime DateCreated { get; set; }
        /// <summary>
        /// Gets or Sets the timestamp that represent the first date/time of creation of the API Users.
        /// </summary>
        public DateTime DateCreatedStart { get; set; }
        /// <summary>
        /// Gets or Sets the timestamp that represent the last date/time of creation of the API Users.
        /// </summary>
        public DateTime DateCreatedEnd { get; set; }
    }
}
