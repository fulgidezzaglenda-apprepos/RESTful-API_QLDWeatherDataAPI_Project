using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using QLDEducationalWeatherDataAPI.Models.Filters;
using QLDEducationalWeatherDataAPI.Models.UserDTOs;
using QLDEducationalWeatherDataAPI.Services;
using System.Text.RegularExpressions;

namespace QLDEducationalWeatherDataAPI.Models.Repositories
{
    /// <summary>
    /// Represents a concrete implementation of the IUserRepository interface for managing user data.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        /// <summary>
        /// The MongoDB collection containing user data.
        /// </summary>
        private readonly IMongoCollection<ApiUsers> _users;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="builder"> MongoDB connection builder. </param>
        public UserRepository(MongoConnectionBuilder builder)
        {
            _users = builder.GetDatabase().GetCollection<ApiUsers>("ApiUsers");
        }

        /// <summary>
        /// Authenticates a user based on the provided API Key and required roles.
        /// </summary>
        /// <param name="apiKey"> The API Key of the user to authenticate. </param>
        /// <param name="requiredRole"> The required user roles for authentication. </param>
        /// <returns>
        /// Returns the authenticated user if successful, othetwise null.
        /// </returns>
        public ApiUsers AuthenticateUser(string apiKey, params UserRoles[] requiredRole)
        {
            var filter = Builders<ApiUsers>.Filter.Eq(u => u.ApiKey, apiKey);
            filter &= Builders<ApiUsers>.Filter.Eq(u => u.IsActive, true);
            var user = _users.Find(filter).FirstOrDefault();

            if (user == null || IsAllowedRole(user.Role, requiredRole) == false)
            {
                return null;
            }
            return user;
        }

        /// <summary>
        /// Checks if the user's role is allowed based on the requiered roles.
        /// </summary>
        /// <param name="userRole"> The user role of the user. </param>
        /// <param name="requiredRole"> The required user roles for authentication. </param>
        /// <returns>
        /// Returns true is the user's role is allowed, otherwise false.
        /// </returns>
        private bool IsAllowedRole(string userRole, UserRoles[] requiredRole)
        {
            if (Enum.TryParse(userRole, out UserRoles roleName) == false)
            {
                return false;
            }

            foreach (var role in requiredRole)
            {
                int userRoleNumber = (int)roleName;
                int requiredRoleNumber = (int)role;

                if (userRoleNumber == requiredRoleNumber)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Creates a new user if the email address is not already registered.
        /// </summary>
        /// <param name="user"> The user that needs to create. </param>
        /// <returns> Returns true if the user is created successful, otherwise false. </returns>
        // -- This is now working base in the given requirements in the scenario. -- Glenda --
        public bool CreateUser(ApiUsers user)
        {
            var filter = Builders<ApiUsers>.Filter.Eq(u => u.EmailAddress, user.EmailAddress);
            var existinguser = _users.Find(filter).FirstOrDefault();

            if (existinguser != null)
            {
                return false;
            }

            user.DateCreated = DateTime.Now;
            user.ApiKey = Guid.NewGuid().ToString();
            user.LastAccess = DateTime.UtcNow;
            user.ExpiryDate = DateTime.UtcNow.AddMonths(3);
            _users.InsertOne(user);
            return true;
        }

        /// <summary>
        /// Creates new users if their email addresses are not already registered on the same date.
        /// </summary>
        /// <param name="userList"> The list of users that needs to be created.</param>
        /// <returns>
        /// Returns true if all users are created successfully, otherwise false.
        /// </returns>
        public bool CreateUsers(ApiUsers userList)
        {
            DateTime currentDate = DateTime.UtcNow;
            var filter = Builders<ApiUsers>.Filter.And(
                         Builders<ApiUsers>.Filter.Eq(u => u.EmailAddress, userList.EmailAddress),
                         Builders<ApiUsers>.Filter.Eq(u => u.DateCreated, currentDate)
                         );
            var extinguisher = _users.Find(filter).FirstOrDefault();

            if (extinguisher != null)
            {
                return false;
            }

            userList.DateCreated = DateTime.Now;
            userList.ApiKey = Guid.NewGuid().ToString();
            userList.LastAccess = DateTime.UtcNow;
            userList.ExpiryDate = DateTime.UtcNow.AddMonths(3);
            _users.InsertOne(userList);
            return true;
        }

        /// <summary>
        /// Deletes a user account by its unique identifier.
        /// </summary>
        /// <param name="id"> This is the unique identifier of the user account to be delete. </param>
        /// <returns>
        /// Returns a response that contains information about the delete operation.
        /// </returns>
        // -- This is now working base in the given requirements in the scenario. -- Glenda --
        public ResponseDTO<ApiUsers> Delete(string id)
        {
            try
            {
                ObjectId ObjId = ObjectId.Parse(id);
                var filter = Builders<ApiUsers>.Filter.Eq(u => u._id, ObjId);
                var result = _users.DeleteOne(filter);
                if (result.DeletedCount > 0)
                {
                    return new ResponseDTO<ApiUsers>
                    {
                        Message = "A user account has been successfully deleted!",
                        WasSuccessful = true,
                        RecordsAffected = Convert.ToInt32(result.DeletedCount)
                    };
                }
                else
                {
                    return new ResponseDTO<ApiUsers>
                    {
                        Message = "No user has been deleted. Please check details and try again!",
                        WasSuccessful = false,
                        RecordsAffected = 0
                    };
                }
            }
            catch (Exception)
            {
                return new ResponseDTO<ApiUsers>
                {
                    Message = "An error occurred while deleting user/s account. Please check all details are correct then try again!",
                    WasSuccessful = false,
                    RecordsAffected = 0
                };
            }
        }

        /// <summary>
        /// Retrieves the deleted user account based on the user role and date range of last access
        /// </summary>
        /// <param name="firstLogin"> The date range to be use for filtering first login access.</param>
        /// <param name="lastLogin"> The date range to be use for filtering the last login access. </param>
        /// <param name="userRole"> User role that is required to retrieve user account. </param>
        /// <returns>
        /// Return a list of deleted user accounts matching the specified criteria.
        /// </returns>
        public List<ApiUsers> GetDeletedUsersByRoleAndDate(DateTime firstLogin, DateTime lastLogin, string userRole)
        {
            var filter = Builders<ApiUsers>.Filter.And(
                         Builders<ApiUsers>.Filter.Eq(r => r.Role, userRole),
                         Builders<ApiUsers>.Filter.Gte(u => u.LastAccess, firstLogin),
                         Builders<ApiUsers>.Filter.Lte(u => u.LastAccess, lastLogin));
            return _users.Find(filter).ToList();
        }

        /// <summary>
        /// Updates the user data with the user role of deleted user.
        /// </summary>
        /// <param name="deletedUsers"> Contains about the information of the deleted user accounts.</param>
        /// <returns>
        /// Returns a response that contains information about the delete operation.
        /// </returns>
        public ResponseDTO<ApiUsers> UpdateDeletedUsers(UsersLogDTO deletedUsers)
        {
            ObjectId Objid = ObjectId.Parse(deletedUsers.ObjId);
            var filter = Builders<ApiUsers>.Filter.And(
                         Builders<ApiUsers>.Filter.Eq(u => u._id, Objid),
                         Builders<ApiUsers>.Filter.Eq(u => u.Role, deletedUsers.Role));
            var result = _users.DeleteOne(filter);

            if (result.DeletedCount > 0)
            {
                return new ResponseDTO<ApiUsers>
                {
                    Message = "User role has been updated successfully!",
                    WasSuccessful = true,
                    RecordsAffected = Convert.ToInt32(result.DeletedCount)
                };
            }
            else
            {
                return new ResponseDTO<ApiUsers>
                {
                    Message = "No user role has been updated. Please check details and try again!",
                    WasSuccessful = false,
                    RecordsAffected = 0
                };
            }
        }

        /// <summary>
        /// Deleted multiple user accounts based on the specified criteria.
        /// </summary>
        /// <param name="deletedUsers"> The list of user accounts that needs to be deleted. </param>
        /// <returns>
        /// Returns a response that contains information about the delete opereation.
        /// </returns>
        // -- This is now working base in the given requirements in the scenario. -- Glenda --
        // -- NOTE: THIS IS FOR TESTING BASED ON THE GIVEN PROJECT SCENARIO REQUIREMENTS.
        public ResponseDTO<ApiUsers> DeleteMany(ApiUsers deletedUsers)
        {
            try
            {
                DateTime firstLogin = DateTime.UtcNow;
                DateTime lastLogin = DateTime.UtcNow;
                var filter = Builders<ApiUsers>.Filter.Eq(u => u.Role, deletedUsers.Role);
                             Builders<ApiUsers>.Update.Set(u => u.Role, deletedUsers.Role);
                             Builders<ApiUsers>.Update.Set(u => u.LastAccess, deletedUsers.LastAccess);
                var result = _users.DeleteMany(filter);
                if (result.DeletedCount > 0)
                {
                    return new ResponseDTO<ApiUsers>
                    {
                        Message = "User/s account has been successfully deleted!",
                        WasSuccessful = true,
                        RecordsAffected = Convert.ToInt32(result.DeletedCount)
                    };
                }
                else
                {
                    return new ResponseDTO<ApiUsers>
                    {
                        Message = "No user/s account matching the criteria have been found or deleted!",
                        WasSuccessful = false,
                        RecordsAffected = 0
                    };
                }
            }
            catch (Exception)
            {
                return new ResponseDTO<ApiUsers>
                {
                    Message = "An error occurred while deleting user/s account. Please check all details are correct then try again!",
                    WasSuccessful = false,
                    RecordsAffected = 0
                };
            }
        }

        /// <summary>
        /// Updates the user role and date created of a user.
        /// </summary>
        /// <param name="roleUpdated"> Information of the updated user role and date created. </param>
        /// <returns>
        /// Returns true if the user is updated successfully, otherwise false.
        /// </returns>
        public bool Update(ApiUsers roleUpdated)
        {
            try
            {
                DateTime startDate = DateTime.UtcNow;
                DateTime endDate = DateTime.UtcNow;
                ObjectId ObjId = ObjectId.Parse(roleUpdated.ObjId);
                var filter = Builders<ApiUsers>.Filter.And(
                             Builders<ApiUsers>.Filter.Eq(u => u.ObjId, roleUpdated.ObjId),
                             Builders<ApiUsers>.Filter.Eq(u => u.Role, roleUpdated.Role));
                var update = Builders<ApiUsers>.Update.Set(u => u.Role, roleUpdated.Role);
                             Builders<ApiUsers>.Update.Set(u => u.DateCreated, roleUpdated.DateCreated);
                var existingUsers = _users.Find(filter).ToList();

                if (existingUsers == null)
                {
                    return false;
                }

                var user = new ApiUsers();
                user.Role = roleUpdated.Role;
                user.DateCreated = DateTime.Now;
                user.ApiKey = Guid.NewGuid().ToString();
                user.LastAccess = DateTime.UtcNow;
                user.ExpiryDate = DateTime.UtcNow.AddMonths(3);

                _users.ReplaceOne(u => u.ObjId == user.ObjId, user);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occured: {ex: Please try again later!}");
                return false;
            }
        }

        /// <summary>
        /// Retrieves all user accounts.
        /// </summary>
        /// <returns> Returns list of all user accounts. </returns>
        public List<ApiUsers> GetAllUsers()
        {
            var builder = Builders<ApiUsers>.Filter;
            var filter = builder.Empty;
            return _users.Find(filter).ToList();
        }

        /// <summary>
        /// Retrieves user accounts with the specified old user role and ID.
        /// </summary>
        /// <param name="id"> The unique identifier of the user accounts need to retrieve. </param>
        /// <param name="oldRole"> The old user role of the user accounts to be retrieve. </param>
        /// <returns> Returns list of user accounts matching the specified criteria. </returns>
        public List<ApiUsers> GetOldUserRoles(string id, string oldRole)
        {
            ObjectId ObjId = ObjectId.Parse(id);
            var filter = Builders<ApiUsers>.Filter.And(
                         Builders<ApiUsers>.Filter.Eq(u => u._id, ObjId),
                         Builders<ApiUsers>.Filter.Eq(r => r.Role, oldRole));
            return _users.Find(filter).ToList();
        }

        /// <summary>
        /// Updates the role of a user.
        /// </summary>
        /// <param name="roleUpdated"> The updated user role information. </param>
        /// <returns>
        /// Returns a response that contains information about the update operation.
        /// </returns>
        public ResponseDTO<ApiUsers> RoleUpdated(UpdatedRoleDTO roleUpdated)
        {
            ObjectId Objid = ObjectId.Parse(roleUpdated.ObjId);
            var filter = Builders<ApiUsers>.Filter.Eq(u => u._id, Objid);
            var update = Builders<ApiUsers>.Update.Set(u => u.Role, roleUpdated.Role);
            var result = _users.UpdateOne(filter, update);

            if (result.ModifiedCount > 0)
            {
                return new ResponseDTO<ApiUsers>
                {
                    Message = "User role has been updated successfully!",
                    WasSuccessful = true,
                    RecordsAffected = Convert.ToInt32(result.ModifiedCount)
                };
            }
            else
            {
                return new ResponseDTO<ApiUsers>
                {
                    Message = "No user role has been updated. Please check details and try again!",
                    WasSuccessful = false,
                    RecordsAffected = 0
                };
            }
        }

        /// <summary>
        /// Retrieves users account based on their user role and creation date within a specified date range.
        /// </summary>
        /// <param name="startDate"> Start date of the date range for filtering user data. </param>
        /// <param name="endDate"> End date of the date range for filtering user data. </param>
        /// <param name="oldRole"> Old user role of the users to be retrieve. </param>
        /// <returns>
        /// Returns a list of users account matching the specified criteria.
        /// </returns>
        public List<ApiUsers> GetUserByRoleAndDate(DateTime startDate, DateTime endDate, string oldRole)
        {
            var filter = Builders<ApiUsers>.Filter.And(
                         Builders<ApiUsers>.Filter.Eq(r => r.Role, oldRole),
                         Builders<ApiUsers>.Filter.Gte(u => u.DateCreated, startDate),
                         Builders<ApiUsers>.Filter.Lte(u => u.DateCreated, endDate));
            return _users.Find(filter).ToList();
        }

        /// <summary>
        /// Updates the role of a user.
        /// </summary>
        /// <param name="updatedRole"> Information of the updated role. </param>
        /// <returns>
        /// Returns a response that contains information about the update operation.
        /// </returns>
        public ResponseDTO<ApiUsers> UpdateUser(UpdatedRoleDTO updatedRole)
        {
            ObjectId Objid = ObjectId.Parse(updatedRole.ObjId);
            var filter = Builders<ApiUsers>.Filter.Eq(u => u._id, Objid);
            var update = Builders<ApiUsers>.Update.Set(u => u.Role, updatedRole.Role);
            var result = _users.UpdateOne(filter, update);

            if (result.ModifiedCount > 0)
            {
                return new ResponseDTO<ApiUsers>
                {
                    Message = "User role has been updated successfully!",
                    WasSuccessful = true,
                    RecordsAffected = Convert.ToInt32(result.ModifiedCount)
                };
            }
            else
            {
                return new ResponseDTO<ApiUsers>
                {
                    Message = "No user role has been updated. Please check details and try again!",
                    WasSuccessful = false,
                    RecordsAffected = 0
                };
            }
        }

        /// <summary>
        /// Updates the role and creation of multiple user accounts.
        /// </summary>
        /// <param name="updatedRole"> Information of updated role and creation date for user accounts.</param>
        /// <returns>
        /// Returns true is the update operation is successful, otherwise false.
        /// </returns>
        public bool UpdateMany(ApiUsers updatedRole)
        {
            try
            {
                DateTime startDate = DateTime.UtcNow;
                DateTime endDate = DateTime.UtcNow;
                var filter = Builders<ApiUsers>.Filter.Eq(u => u.Role, updatedRole.Role);
                var update = Builders<ApiUsers>.Update.Set(u => u.Role, updatedRole.Role);
                             Builders<ApiUsers>.Update.Set(u => u.DateCreated, updatedRole.DateCreated);
                var existingUsers = _users.Find(filter).ToList();

                if (existingUsers.Count == 0)
                {
                    return false;
                }

                foreach (var user in existingUsers)
                {
                    user.Role = updatedRole.Role;
                    user.DateCreated = DateTime.Now;
                    user.ApiKey = Guid.NewGuid().ToString();
                    user.LastAccess = DateTime.UtcNow;
                    user.ExpiryDate = DateTime.UtcNow.AddMonths(3);
                    _users.ReplaceOne(u => u.ObjId == user.ObjId, user);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occured: {ex: Please try again later!}");
                return false;
            }
        }

        /// <summary>
        /// Updates the last access and expiry date of a user indentifier by the API Key.
        /// </summary>
        /// <param name="apiKey"> The API Key of the user. </param>
        public void UpdateLastLogin(string apiKey)
        {
            var currentDate = DateTime.UtcNow;
            var filter = Builders<ApiUsers>.Filter.Eq(a => a.ApiKey, apiKey);
            var update = Builders<ApiUsers>.Update.Set(u => u.LastAccess, currentDate)
                                                   .Set(u => u.ExpiryDate, currentDate.AddMonths(3));
            _users.UpdateOne(filter, update);
        }
    }
}