
 
using System;

namespace FrameWork
{
    public class IDGenerator
    {
        // Génère un ID unique pour chaque objet
        public static string GenerateID()
        {
            return Guid.NewGuid().ToString();
        }
    }
}