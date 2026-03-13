namespace QLDEducationalWeatherDataAPI.Models.UserDTOs
{
    /// <summary>
    /// Represents a generic response DTO that contains data of type T.
    /// </summary>
    /// <typeparam name="T"> The type of data contained in the response DTO.</typeparam>
    public class ResponseDTO<T>
    {
        /// <summary>
        /// Gets or Sets the message associated with the response.
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// Gets or Sets the value that indicates whether the operation was successful.
        /// </summary>
        public bool WasSuccessful { get; set; } = true;
        /// <summary>
        /// Gets or Sets the T value associated with the response which can be of type T.
        /// </summary>
        public T? Value { get; set; }
        /// <summary>
        /// Gets or Sets the number of records affected by the operation.
        /// </summary>
        public int RecordsAffected { get; set; }
    }
}
