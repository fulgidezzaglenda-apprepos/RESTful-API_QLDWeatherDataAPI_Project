using QLDEducationalWeatherDataAPI.Models.WeatherDTOs;

namespace QLDEducationalWeatherDataAPI.Models.Repositories
{
    /// <summary>
    /// Represents a generic repository interface for CRUD operation.
    /// </summary>
    /// <typeparam name="T"> The type of entity managed by the repository. </typeparam>
    public interface IGenericRepository<T>
    {
        /// <summary>
        /// Retrieves all weather data.
        /// </summary>
        /// <returns> An enumerable collection of weather data. </returns>
        IEnumerable <WeatherData> GetAll();
        /// <summary>
        /// Retrieves an entity by its unique identifier.
        /// </summary>
        /// <param name="id"> The unique identifier of the entity. </param>
        /// <returns> Returns the entity with the specified identifier. </returns>
        T GetById(string id);
        /// <summary>
        /// Creates a new entity.
        /// </summary>
        /// <param name="item"> This is the entity to be created. </param>
        void Create(T item);
        /// <summary>
        /// This is to update an entity with the specified identifier and device name.
        /// </summary>
        /// <param name="id"> The unique identifier of the entity. </param>
        /// <param name="DeviceName"> The device name associated with the entity. </param>
        /// <param name="item"> This is the updated entity data. </param>
        /// <returns>
        /// Returns a response that contains information about the update operation.
        /// </returns>
        ResponseDTO<T> Update(string id, string DeviceName, T item);
        /// <summary>
        /// Deletes an entity with the specified identifier.
        /// </summary>
        /// <param name="id"> The unique identifier of the entity to delete. </param>
        /// <returns>
        /// Returns a response that contains information about the delete operation.
        /// </returns>
        ResponseDTO<T> Delete(string id);
    }
}
