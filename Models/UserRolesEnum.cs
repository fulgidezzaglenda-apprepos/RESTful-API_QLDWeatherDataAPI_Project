namespace QLDEducationalWeatherDataAPI.Models
{
    /// <summary>
    /// Enum that represents different user roles associated with the level of access.
    /// </summary>
    public enum UserRoles
    {
        /// <summary>
        /// Represents the role of a Teacher along with the numeric level access.
        /// </summary>
        TEACHER = 20,
        /// <summary>
        /// Represents the role for an Administrator along with the numeric level access.
        /// </summary>
        SENSOR = 10,
        /// <summary>
        /// Represents the role of a Student along with the numeric level access.
        /// </summary>
        STUDENT = 0
    }
}
