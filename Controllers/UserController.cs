using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using QLDEducationalWeatherDataAPI.AttributeTags;
using QLDEducationalWeatherDataAPI.Models;
using QLDEducationalWeatherDataAPI.Models.Repositories;
using QLDEducationalWeatherDataAPI.Models.UserDTOs;
using System.Data;

namespace QLDEducationalWeatherDataAPI.Controllers
{
    /// <summary>
    /// Controller for managing user related operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// Represents the user repository used for accessing user data.
        /// </summary>
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="userRepository"> The user repository for accessing user data. </param>
        public UserController(IUserRepository userRepository)
        {
            // Assigning the provided use repository to the local variable.
            _userRepository = userRepository;
        }

        /// <summary>
        /// This is to check if the provided API Key is authenticated with the specified required role.
        /// </summary>
        /// <param name="apiKey"> This is the API Key to authenticate. </param>
        /// <param name="requiredRole"> This is the required user role for the authentication. </param>
        /// <returns>
        /// Returns true is the API Key is authenticated with the specified role, otherwise make it false.
        /// </returns>
        private bool IsAuthenticated(string apiKey, UserRoles requiredRole)
        {
            // Authenticating the user with the provided API Key and required user role.
            // If the authentication fails, return to false.
            if (_userRepository.AuthenticateUser(apiKey, requiredRole) == null)
            {
                return false;
            }
            // Updates the last login timestamp for the authenticated uesr.
            _userRepository.UpdateLastLogin(apiKey);
            // Then, return true if the authentication is successful.
            return true;
        }

        /// <summary>
        /// This method is a delete operation that delete a user account by its identifier.
        /// </summary>
        /// <param name="id"> This is the identifier of the user account to be deleted. </param>
        /// <returns>
        /// Returns an OK response if the deletion is successful.
        /// Returns a BadRequest response if the provided 'Id' is null or empty, or if the deletion fails.
        /// Or, returns a UnAuthorized response if the user is not authorized to perfrom the action.
        /// </returns>
        //DELETE: api/<UserController>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string),StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string),StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(string),StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string),StatusCodes.Status500InternalServerError)]
        [HttpDelete("DeleteById")]
        [ApiKey(UserRoles.TEACHER)]
        public ActionResult Delete(string id)
        {
            // Checks if the provided 'id' is null or empty.
            if (String.IsNullOrWhiteSpace(id))
            {
                // If it is, returns a BadRequest response indicating that a valid 'id' is required to perform this action.
                return BadRequest("A valid id is required to perform this operation!");
            }
            // Attempts to delete the user account by its identifier.
            var result = _userRepository.Delete(id);
            // Checks if the delete operation was not successful.
            if (!result.WasSuccessful)
            {
                // If not, returns a BadRequest indicating failed to delete user account.
                return BadRequest("Failed to delete user account!");
            }
            else
            {
                // If it is successfull, returns an OK response indicating the success deletion of the specific user account.
                return Ok(result);
            }
        }

        /// <summary>
        /// This method is a delete operation that delete multiple user account by a specified user role within the given DateTime range.
        /// </summary>
        /// <param name="firstLogin"> This is the first login date for the users account use for filtering. </param>
        /// <param name="lastLogin"> This is the last login date for the users account use for filtering. </param>
        /// <param name="userRole"> This is the user role specified for deletion. </param>
        /// <returns>
        /// Returns an OK response if the deletion is successful.
        /// If not, returns a BadRequest response if any validation fails or if the deletion fails.
        /// </returns>
        //DELETE: api/<UserController>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DateTime),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [HttpDelete("DeleteMany")]
        [ApiKey(UserRoles.TEACHER)]
        public ActionResult DeleteMany(DateTime firstLogin, DateTime lastLogin, string userRole)
        {
            // Checks if the user role is null or empty.
            if (String.IsNullOrWhiteSpace(userRole))
            {
                // If it is, returns a BadRequest response indicating that a valid user role is required to perform this operation.
                return BadRequest($"A valid role is required to perform this operation!:");
            }
            // Checks if the provided dates are the default DateTime value.
            if (firstLogin == default(DateTime) || lastLogin == default(DateTime))
            {
                // If it is not, returns a BadRequest response indicating a valid date is required to perform this operation.
                return BadRequest($"A valid date is required to perform this operation: {firstLogin.Date} and {lastLogin.Date}");
            }
            // List to store details of deleted users.
            List<UsersLogDTO> updateDeletedUsers = new List<UsersLogDTO>();
            // This is where to retrieves users to delete based on specified criteria.
            var usersToDelete = _userRepository.GetDeletedUsersByRoleAndDate(firstLogin, lastLogin, userRole);
            // Checks if no users are found based on the specified criteria.
            if (usersToDelete == null)
            {
                // It it is, returns a BadRequest response indicating that no users are found with user role between the given DateTime range.
                return BadRequest($"No users found with user role '{userRole}' between date created '{firstLogin}' and '{lastLogin}'.");
            }

            // This loops iterates through each of user account to be deleted.
            foreach (var userDTO in usersToDelete)
            {
                // Parsing the user role string to an enum value, then checks if it is true or false.
                if (Enum.TryParse(userRole, out UserRoles roleName) == false)
                {
                    // If it is false, returns a BadRequest response indicating Invalid user role provided.
                    return BadRequest($"Invalid user role provided for user: {userRole}");
                }
                // This is to create a DTO object for the deleted user accounts.
                var deletedUsers = new UsersLogDTO
                {
                    _id = userDTO._id,
                    Role = userRole,
                    FirstLogin = userDTO.LastAccess,
                    LastLogin = userDTO.LastAccess,
                };
                // Updating the repository upon the deletion operation of user accounts.
                var result = _userRepository.UpdateDeletedUsers(deletedUsers);
                // Checks if the update operation was not successful.
                if (!result.WasSuccessful)
                {
                    // If its not, returns a BadRequest response indicating failed to delete user accounts.
                    return BadRequest($"Failed to delete users!");
                }
                // Adding all deleted user accounts details to the list.
                updateDeletedUsers.Add(deletedUsers);
            }
            // Returns an OK response indicating successful deletion of user accounts.
            return Ok($"Users account have been successfully deleted! Total users account deleted: {updateDeletedUsers.Count}");
        }

        /// <summary>
        /// This method is create operation that create a single new user account.
        /// </summary>
        /// <param name="userDTO"> This is the DTO containing new user information. </param>
        /// <returns>
        /// Returns an OK response with the API Key if the creation is successful.
        /// Otherwise, returns a BadRequest response indicating if any validation fails or if the user account alreadt exists.
        /// </returns>
        //POST: api/<UserController>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [HttpPost("Create")]
        [ApiKey(UserRoles.TEACHER)]
        public ActionResult CreateUser(UserDetailsDTO userDTO)
        {
            // Parsing the provided user role string to enum value, then checks if its true or false.
            if (Enum.TryParse(userDTO.Role.ToUpper(), out UserRoles roleName) == false)
            {
                // If its false, returns a BadRequest response indicating invalid user role provided.
                return BadRequest($"Invalid user role provided for user: {userDTO.Role}");
            }
            // This is to create a new user account object.
            var user = new ApiUsers
            {
                UserName = userDTO.UserName,
                EmailAddress = userDTO.EmailAddress,
                Role = roleName.ToString(),
                IsActive = true
            };
            // Attempts to create the user account.
            var result = _userRepository.CreateUser(user);
            // Checks if the user account creation failed.
            if (result == false)
            {
                // If it is, returns a BadRequest response indicating that user account already exists for this email.
                return BadRequest($"User account already exists for this email!: {userDTO.EmailAddress}");
            }
            // Otherwise, returns and OK response with the API Key if the creation is successful.
            return Ok($"A user account has been successfully created! Your API Key is: {user.ApiKey}");
        }
        
        /// <summary>
        /// This method is a creation operation that creates multiple new user accounts with a specified user role.
        /// </summary>
        /// <param name="usersDTOList"> This is list of DTOs that contains user accounts information. </param>
        /// <returns>
        /// Returns an OK response with the total count of user accounts created if the creation is successful.
        /// Otherwise, returns a BadRequest response if any validation fails or if any of the user accounts already exists.
        /// </returns>
        //POST: api/<UserController>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [HttpPost("CreateMany")]
        [ApiKey(UserRoles.TEACHER)]
        public ActionResult CreateUsers(List<UserDetailsDTO> usersDTOList)
        {
            // This is the list where the created user accounts will be stored.
            List<ApiUsers> createdUsers = new List<ApiUsers>();
            // This loops iterates through each of userDTO in the provided list.
            foreach (var userDTO in usersDTOList)
            {
                // Parsing the provided user role string to enum value, then checks if its true or false.
                if (Enum.TryParse(userDTO.Role.ToUpper(), out UserRoles roleName) == false)
                {
                    // If its false, returns a BadRequest response indicating invalid user role provided.
                    return BadRequest($"Invalid user role provided for user: {userDTO.Role}");
                }

                // Setting the creation date for the user accounts.
                DateTime currentDate = DateTime.UtcNow;
                //userDTO.DateCreated = currentDate;
                // Creating a new users object.
                var users = new ApiUsers
                {
                    UserName = userDTO.UserName,
                    EmailAddress = userDTO.EmailAddress,
                    Role = roleName.ToString(),
                    IsActive = true

                };
                // Attempts to create the new user accounts.
                var result = _userRepository.CreateUsers(users);
                // Checks if the user accounts creation failed.
                if (result == false)
                {
                    // If it is, returns a BadRequest response indicating that user accounts already exists for this email.
                    return BadRequest($"User/s account already exists for this email: {userDTO.EmailAddress}");
                }
                // Adding the created user accounts to the list.
                createdUsers.Add(users);
            }
            // Then, returns an OK response indicating that the creaion is successful with the total count of user accounts created.
            return Ok($"Users account has been successfully created! API Key provided: Total users created: {createdUsers.Count}");
        }

        /// <summary>
        /// This method is an update operation that updates user role for single user account that is identified by the provided 'id'.
        /// </summary>
        /// <param name="id"> This is the identifier for the user account that need to update the user role. </param>
        /// <param name="oldRole"> This is the old user role of the user account that need to be update. </param>
        /// <param name="newRole"> The new user role to be assign to the user accout.</param>
        /// <returns>
        /// Returns an OK repsonse indicating if the update is successful.
        /// Otherwise, returns a BadRequest response if any of the validation fails or if the update opertaion fails.
        /// </returns>
        //PATCH: api/<UserController>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [HttpPatch("UpdateOne")]
        [ApiKey(UserRoles.TEACHER)]
        public ActionResult Update(string id, string oldRole, string newRole)
        {
            // Checks if the provided 'id' is null or empty.
            if (String.IsNullOrWhiteSpace(id))
            {
                // If it is, returns a BadRequest response, indicating that a valid 'id' is required to perform this action.
                return BadRequest($"A valid id is required to perform this action: {id}");
            }
            // Checks if the provided new and old user role is null or empty.
            if (String.IsNullOrWhiteSpace(newRole) || String.IsNullOrWhiteSpace(oldRole))
            {
                // If it is, returns a BadRequest response indicating that a valid user role is required to perform this operation.
                return BadRequest($"A valid role is required to perform this operation!: {newRole} and {oldRole}");
            }

            // Retrieves the user account that needs to be update based on the provided 'id' and 'old role'.
            var userToUpdate = _userRepository.GetOldUserRoles(id, oldRole);
            // Checks if there is no user account found with the old role.
            if (userToUpdate == null)
            {
                // If it is, returns a BadRequest response indicating that no user account found with the old role.
                return BadRequest($"No user found with old role '{oldRole}'.");
            }
            // Checks in the userDTO 
            var userDTO = new UserDTO();
            // Parsing the new user role string to an enum value, then checks if it is true or false.
            if (Enum.TryParse(newRole, out UserRoles roleName) == false)
            {
                // If its false, returns a BadRequest response indicating invalid user role provided.
                return BadRequest($"Invalid user role provided for user: {newRole}");
            }
            // Creates a DTO object for updating user role.
            var roleUpdated = new UpdatedRoleDTO
            {
                _id = ObjectId.Parse(id),
                 Role = newRole,
            };
            // Updates the user role in our repository.
            var result = _userRepository.RoleUpdated(roleUpdated);
            // Checks if the update operation was not successful.
            if (!result.WasSuccessful)
            {
                // It its not, returns a BadRequest response indicating failed to update user role.
                return BadRequest($"Failed to update user role for user: {userDTO.UserName}");
            }
            // Then, returns an OK response indicating successful updated if the update operation is successful.
            return Ok($"User roles have been successfully updated for {userToUpdate.Count} users");
        }

        /// <summary>
        /// This method is an update operation that updates user roles of multiple users within a specified DateTime range.
        /// </summary>
        /// <param name="startDate"> This is the start date for the users account use for filtering. </param>
        /// <param name="endDate"> This is the end date for the users account use for filtering. </param>
        /// <param name="oldRole"> The old user role of the users to be update. </param>
        /// <param name="newRole"> The new user role for the users to be assign after the update. </param>
        /// <returns>
        /// Returns an OK response indicating if the update operation is successful.
        /// Otherwise, returns a BadRequest response indicating if any validation fails or if the update operation fails.
        /// </returns>
        //PUT: api/<UserController>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [HttpPut("UpdateMany")]
        [ApiKey(UserRoles.TEACHER)]
        public ActionResult UpdateMany(DateTime startDate, DateTime endDate, string oldRole, string newRole)
        {
            // Checks if the provided old or new user role is null or empty.
            if (String.IsNullOrWhiteSpace(newRole) || String.IsNullOrWhiteSpace(oldRole))
            {
                // If it is, returns a BadRequest response indicating a valid user role is required to perform this operation.
                return BadRequest($"A valid role is required to perform this operation!: {newRole} and {oldRole}");
            }
            // Checks if the provided start or end date is the default DateTime value.
            if (startDate == default(DateTime) || endDate == default(DateTime))
            {
                // If its not, returns a BadRequest response indicating a valid date is required to perform this operation.
                return BadRequest($"A valid date is required to perform this operation: {startDate.Date} and {endDate.Date}");
            }
            // This is the list where the updated user accounts with new user roles will be stored.
            List<UpdatedRoleDTO> updatedUsersRole = new List<UpdatedRoleDTO>();
            // Retrieves user accounts that needs to be update based on specified criteria.
            var usersToUpdate = _userRepository.GetUserByRoleAndDate(startDate, endDate, oldRole);
            // Checks is there's no user accounts are found based on the specified criteria.
            if (usersToUpdate == null || usersToUpdate.Count == 0)
            {
                // If it is, returns a BadRequest response indicating there's no user accouts found with the old user role between the given DateTime range.
                return BadRequest($"No users found with old role '{oldRole}' between date created '{startDate}' and '{endDate}'.");
            }
            // This loops iterates through each user accounts to update their user role.
            foreach (var userDTO in usersToUpdate)
            {
                // Parsing the new user role string to an enum value, then checks if it is true or false.
                if (Enum.TryParse(newRole, out UserRoles roleName) == false)
                {
                    // It its false, returns a BadRequest response indicating invalid user role provided.
                    return BadRequest($"Invalid user role provided for user: {newRole}");
                }
                // Createa a DTO object for updating user account with new user role.
                var updatedRole = new UpdatedRoleDTO
                {
                    _id = userDTO._id,
                    Role = newRole,
                    DateCreatedStart = userDTO.DateCreated,
                    DateCreatedEnd = userDTO.DateCreated,
                };
                // Updates the user role in our repository.
                var result = _userRepository.UpdateUser(updatedRole);
                // Checks if the update operation is successfull.
                if (!result.WasSuccessful)
                {
                    // If its not, returns a BadRequest response indicating failed to update user role for users.
                    return BadRequest($"Failed to update user role for user: {userDTO.UserName}");
                }
                // Adding the updated user role details to the list.
                updatedUsersRole.Add(updatedRole);
            }
            // Then, returns an OK response indicating successful update of user roles.
            return Ok($"User roles have been successfully updated for {updatedUsersRole.Count} users");
        }
    }
}
