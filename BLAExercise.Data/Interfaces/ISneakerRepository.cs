﻿using BLAExercise.Data.Models;

namespace BLAExercise.Data.Interfaces;

public interface ISneakerRepository : IGenericRepository<Sneaker>
{
    /// <summary>
    /// Retrieves a list of sneakers associated with a specific user by their user ID.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose sneakers are to be retrieved.</param>
    /// <returns>A task that represents the asynchronous operation, returning a list of sneakers.</returns>
    Task<List<Sneaker>> GetSneakersByUserIdAsync(int userId);

    /// <summary>
    /// Retrieves a list of sneakers associated with a specific user by their email address.
    /// </summary>
    /// <param name="email">The email address of the user whose sneakers are to be retrieved.</param>
    /// <returns>A task that represents the asynchronous operation, returning a list of sneakers.</returns>
    Task<List<Sneaker>> GetSneakersByUserEmailAsync(string email);
}
