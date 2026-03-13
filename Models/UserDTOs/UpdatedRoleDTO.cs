using MongoDB.Bson;

namespace QLDEducationalWeatherDataAPI.Models.UserDTOs
{
    /// <summary>
    /// Data transfer object that represents the Updated User role.
    /// </summary>
    public class UpdatedRoleDTO
    {
        /// <summary>
        /// Gets or Sets the unique identifier for the Updated user role.
        /// </summary>
        public ObjectId _id { get; set; }
        /// <summary>
        /// Gets the string representtion of the unique identifier for the API Users.
        /// </summary>
        public string ObjId => _id.ToString();
        /// <summary>
        /// Gets or Sets the User role associated with the API Users .
        /// </summary>
        public string Role {  get; set; }
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
