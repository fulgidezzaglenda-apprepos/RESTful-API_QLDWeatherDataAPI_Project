using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace QLDEducationalWeatherDataAPI.Models
{
    /// <summary>
    /// Represents a class for managing the API Users.
    /// </summary>
    public class ApiUsers
    {
        /// <summary>
        /// Gets or Sets the unique identifier for the API users.
        /// </summary>
        [JsonIgnore]
        [BsonId]
        public ObjectId _id {  get; set; }
        /// <summary>
        /// Gets the string representation of the unique identifier for the API Users.
        /// </summary>
        public string ObjId => _id.ToString();
        /// <summary>
        /// Gets or Sets the UserName associated with the API Users.
        /// </summary>
        public string UserName {  get; set; }
        /// <summary>
        /// Gets or Sets the Email Address associated with the API Users.
        /// </summary>
        public string EmailAddress { get; set; }
        /// <summary>
        /// Gets or Sets the User Role associated for the API Users.
        /// </summary>
        public string Role {  get; set; }
        /// <summary>
        /// Gets or Sets a value that indcates wheter the API Users is Active.
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Gets or Sets timestamp representine the last access time of the API Users.
        /// </summary>
        [BsonElement("Last Access")]
        public DateTime LastAccess {  get; set; }
        /// <summary>
        /// Gets or Set the expiration date for the API Users.
        /// </summary>
        [BsonElement("Expiry Date")]
        public DateTime ExpiryDate { get; set; }
        /// <summary>
        /// Gets or Sets the API Key value assign for the API Users.
        /// </summary>
        public string ApiKey {  get; set; }
        /// <summary>
        /// Gets or Sets the DateTime that indicate the creation time of the API Userss.
        /// </summary>
        [BsonElement("Date Created")]
        public DateTime DateCreated { get; set; }
    }
}
