using QLDEducationalWeatherDataAPI.Models.Filters;

namespace QLDEducationalWeatherDataAPI.Models.UserDTOs
{
    /// <summary>
    /// Data transfe object that represents the user details.
    /// </summary>
    public class UserDetailsDTO
    {
        /// <summary>
        /// Gets or Sets the filter applied to the user log data.
        /// </summary>
        //public UserFilter Filter { get; set; }
        /// <summary>
        /// Gets or Sets the UserName associated with the API Users.
        /// </summary>
        public string? UserName { get; set; }
        /// <summary>
        /// Gets or Sets the Email Address associated with the API Users.
        /// </summary>
        public string? EmailAddress { get; set; }
        /// <summary>
        /// Gets or Sets the User role associated with the API Users.
        /// </summary>
        public string? Role { get; set; }
    }
}
