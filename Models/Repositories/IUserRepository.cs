using MongoDB.Bson;
using QLDEducationalWeatherDataAPI.Models.UserDTOs;

namespace QLDEducationalWeatherDataAPI.Models.Repositories
{
    /// <summary>
    /// Represents a repository interface for managing user data.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Authenticates a user based on the provided API Key and requiered roles.
        /// </summary>
        /// <param name="apiKey"> The API Key of the user to authenticate. </param>
        /// <param name="requiredRole"> The required user roles for authentication. </param>
        /// <returns>
        /// Returns the authenticated user if successful, otherwise null.
        /// </returns>
        ApiUsers AuthenticateUser(string apiKey, params UserRoles[] requiredRole);
        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user"> The user to create. </param>
        /// <returns> Returns true if the user creation is successful, otherwise false. </returns>
        bool CreateUser(ApiUsers user);
        /// <summary>
        /// Creates multiple new users.
        /// </summary>
        /// <param name="user"> The list of users to create. </param>
        /// <returns>
        /// Returns true if all users are created successfully, otherwise false. 
        /// </returns>
        bool CreateUsers(ApiUsers userList);
        /// <summary>
        /// Updates the last login timestamp for the user identifier by the API Key.
        /// </summary>
        /// <param name="apiKey"> The API Key of the user. </param>
        void UpdateLastLogin(string apiKey);
        /// <summary>
        /// Deletes a single user account with the specified identifier.
        /// </summary>
        /// <param name="user"> The unique identifier of the user to be deleted. </param>
        /// <returns>
        /// Returns a response that contains information about the delete operation.
        /// </returns>
        ResponseDTO<ApiUsers> Delete(string id);
        /// <summary>
        /// Deletes multiple users.
        /// </summary>
        /// <param name="deletedUsers"> The list of users to be deleted. </param>
        /// <returns>
        /// Returns a response that contains information about the delete operation.
        /// </returns>
        ResponseDTO<ApiUsers> DeleteMany(ApiUsers deletedUsers);
        /// <summary>
        /// Retrieves deleted users with the specified user role and within the specified date range.
        /// </summary>
        /// <param name="firstLogin"> This is the first login date for filtering deleted users. </param>
        /// <param name="lastLogin"> This is the last login date for filtering the deleted users. </param>
        /// <param name="userRole"> The user role of the deleted users to filter. </param>
        /// <returns>
        /// Returns a list of deleted users matching the specified criteria.
        /// </returns>
        List<ApiUsers> GetDeletedUsersByRoleAndDate(DateTime firstLogin, DateTime lastLogin, string userRole);
        /// <summary>
        /// This is to updates the deletion status of users.
        /// </summary>
        /// <param name="deletedUsers"> This is the DTO that contains information about the users to update after deletion. </param>
        /// <returns>
        /// Returns a response that contains information about the update operation after deletion.
        /// </returns>
        ResponseDTO<ApiUsers> UpdateDeletedUsers(UsersLogDTO deletedUsers);
        /// <summary>
        /// This is to updates the user role of multiple users.
        /// </summary>
        /// <param name="updatedRole"> The DTO that contains information about the users and their updated roles. </param>
        /// <returns>
        /// Returns true if the update operation is succesful, otherwise false.
        /// </returns>
        bool UpdateMany(ApiUsers updatedRole);
        /// <summary>
        /// Retrieves list of users with the specified role and withing the specified date range.
        /// </summary>
        /// <param name="startDate"> The start date for filtering users. </param>
        /// <param name="endDate"> The end date for filtering users. </param>
        /// <param name="oldRole"> The old user role of the users to be filter. </param>
        /// <returns>
        /// Returns a list of users matching the specified criteria.
        /// </returns>
        List<ApiUsers> GetUserByRoleAndDate (DateTime startDate, DateTime endDate, string oldRole);
        /// <summary>
        /// Updates user role of a user.
        /// </summary>
        /// <param name="updatedRole"> The DTO that contains information about the user and the updated role. </param>
        /// <returns>
        /// Returns a response that contains information about the update operation.
        /// </returns>
        ResponseDTO<ApiUsers> UpdateUser (UpdatedRoleDTO updatedRole);
        /// <summary>
        /// Retrieves list of all users.
        /// </summary>
        /// <returns> Returns list of all users. </returns>
        List<ApiUsers> GetAllUsers();
        /// <summary>
        /// Retrieves users with the specfied old user role and identifier.
        /// </summary>
        /// <param name="id"> The unique identifier of the user. </param>
        /// <param name="oldRole"> Old uer role of the user. </param>
        /// <returns>
        /// Returns a list of users matching the specified criteria.
        /// </returns>
        List<ApiUsers> GetOldUserRoles (string id, string oldRole);
        /// <summary>
        /// Updates the user role of a user.
        /// </summary>
        /// <param name="roleUpdated"> The DTO that contains information about the user and the updated role. </param>
        /// <returns>
        /// Returns true is the update operation is successful, otherwise false.
        /// </returns>
        bool Update(ApiUsers roleUpdated);
        /// <summary>
        /// Updates the user role of a user.
        /// </summary>
        /// <param name="roleUpdated"> The DTO that contains information about the user and the updated role.</param>
        /// <returns>
        /// Returns a response that contains information about the update operation.
        /// </returns>
        ResponseDTO<ApiUsers> RoleUpdated(UpdatedRoleDTO roleUpdated);
    }
}
