using UnityEngine;

namespace Core.Extensions
{
    public static class UnityExtensions
    {
        /// <summary>
        /// Returns the object itself if it exists, null otherwise.
        /// </summary>
        /// <remarks>
        /// This method helps differentiate between a null reference and a destroyed Unity object.
        /// Unity's "== null" check can incorrectly return true for destroyed objects, leading to 
        /// misleading behaviour. The OrNull method uses Unity's "null check", and if the object 
        /// has been marked for destruction, it ensures an actual null reference is returned,
        /// aiding in correctly chaining operations and preventing NullReferenceExceptions.
        /// </remarks>
        public static T OrNull<T>(this T obj) where T : Object => obj ? obj : null;
    }
}

