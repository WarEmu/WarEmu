using System.Collections;

namespace FrameWork
{
    public interface ICache
    {
        // Collection des Clef du cache
        ICollection Keys { get; }

        // Récupère un objet de la collection
        object this[object key] { get; set; }
    }
}