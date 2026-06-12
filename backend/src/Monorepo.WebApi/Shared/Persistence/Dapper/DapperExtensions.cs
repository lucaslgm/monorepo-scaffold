using Dapper;

namespace Monorepo.WebApi.Shared.Persistence.Dapper;

public static class DapperExtensions
{
    /// <param name="value">The argument value.</param>
    extension(string value)
    {
        /// <summary>
        /// Builds a DbString argument as a VARCHAR.
        /// </summary>
        /// <param name="length">The argument length.</param>
        /// <returns>The configured DbString.</returns>
        public DbString ToVarChar(int length)
        {
            return new DbString
            {
                Value = value,
                Length = length,
                IsAnsi = true,
                IsFixedLength = false
            };
        }

        /// <summary>
        /// Builds a DbString argument as a CHAR.
        /// </summary>
        /// <param name="length">The argument length.</param>
        /// <returns>The configured DbString.</returns>
        public DbString ToChar(int length)
        {
            return new DbString
            {
                Value = value,
                Length = length,
                IsAnsi = true,
                IsFixedLength = true
            };
        }
    }

    /// <param name="value">The argument value.</param>
    extension(char value)
    {
        /// <summary>
        /// Builds a DbString argument as a VARCHAR.
        /// </summary>
        /// <returns>The configured DbString.</returns>
        public DbString ToVarChar()
        {
            return new DbString
            {
                Value = value.ToString(),
                Length = 1,
                IsAnsi = true,
                IsFixedLength = false
            };
        }

        /// <summary>
        /// Builds a DbString argument as a CHAR.
        /// </summary>
        /// <returns>The configured DbString.</returns>
        public DbString ToChar()
        {
            return new DbString
            {
                Value = value.ToString(),
                Length = 1,
                IsAnsi = true,
                IsFixedLength = true
            };
        }
    }

    public static DbString ToDateTime(this DateTime value)
    {
        var dateString = value.ToString("yyyy-MM-dd HH:mm:ss.fff");

        return new DbString
        {
            Value = dateString,
            Length = dateString.Length,
            IsAnsi = true,
            IsFixedLength = false
        };
    }
}
