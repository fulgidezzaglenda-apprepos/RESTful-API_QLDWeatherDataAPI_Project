using MongoDB.Bson;

namespace QLDEducationalWeatherDataAPI.Models.UserDTOs
{
    /// <summary>
    /// Data transfer object that represents user log information.
    /// </summary>
    public class UsersLogDTO
    {
        /// <summary>
        /// Gets or Sets the unique identifier of the Users log.
        /// </summary>
        public ObjectId _id { get; set; }
        /// <summary>
        /// Gets the string representation of the unique identifier for the API Users.
        /// </summary>
        public string ObjId => _id.ToString();
        /// <summary>
        /// Gets or Sets the User role associated with the API Users.
        /// </summary>
        public string Role { get; set; } = string.Empty;
        /// <summary>
        /// Gets or Sets the timestamp representing the last access time of the API Users.
        /// </summary>
        public DateTime LastAccess {  get; set; }
        /// <summary>
        /// Gets or Sets the timestamp representing the first loging for the API users.
        /// </summary>
        public DateTime FirstLogin { get; set; }
        /// <summary>
        /// Gets or Sets the timestamp representing the last login for the API users.
        /// </summary>
        public DateTime LastLogin { get; set; }
    }
}
